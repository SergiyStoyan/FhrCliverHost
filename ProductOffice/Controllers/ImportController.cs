using System;
using System.Text;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Cliver.ProductOffice.Models;
using Cliver.FhrApi.ProductOffice.Models;
using System.IO;
using System.Data.Entity.Validation;

namespace Cliver.ProductOffice.Controllers
{
    [Authorize]
    public class ImportController : Controller
    {
        private DbApi db = new DbApi();
        
        List<object> MapSelect
        {
            get
            {
                var cs = db.ImportMaps.Select(r => new { Value = r.Id, Name = r.Company.Name + ": " + r.Name + " (" + r.Currency.Symbol + ")" }).ToList<object>();
                return cs;
            }
        }

        public ActionResult Index()
        {
            ViewBag.MapId = new SelectList(MapSelect, "Value", "Name", null);
            return View();
        }
        
//        public void progress(int value)
//        {
//            if(value > 99)
//                Redirect("Import");                
//            if (next_progress_time > DateTime.Now)
//                return;
//            if (!progress_header_sent)
//            {
//                Response.Write(@"
//  <link rel='stylesheet' href='//code.jquery.com/ui/1.11.4/themes/smoothness/jquery-ui.css'>
//  <script src='//code.jquery.com/jquery-1.10.2.js'></script>
//  <script src='//code.jquery.com/ui/1.11.4/jquery-ui.js'></script>
//
//<div id='ProgressBar'></div>");
//                progress_header_sent = true;
//            }
//            Response.Write(@"<div id='ProgressBar'></div>
//<script>
//  $(function() {
//    $('#progressbar').progressbar({
//      value: " + value.ToString() + @"
//    });
//  });
//</script>");
//            Response.Flush();
//            next_progress_time = DateTime.Now.AddSeconds(3);
//        }
//        DateTime next_progress_time = DateTime.Now;
//        bool progress_header_sent = false;

        public void progress(int value)
        {
            Response.Write(value.ToString());
            Response.Flush();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(Import import)
        {
            ViewBag.MapId = new SelectList(MapSelect, "Value", "Name", import.MapId);

            if (!ModelState.IsValid)
            {
                if (Request.IsAjaxRequest())
                    return PartialView(import);
                return View(import);
            }

            DirectoryInfo di = new DirectoryInfo(Path.GetTempPath() + "import_product_files");
            if (!di.Exists)
                di.Create();
            foreach (FileInfo fi in di.GetFiles())
                fi.Delete();
            string file = di.FullName + "\\" + import.File.FileName;
            import.File.SaveAs(file);

            try
            {
                process_file(file, import);
            }
            catch (DbEntityValidationException e)
            {
                List<string> ms = new List<string>();
                foreach (var eve in e.EntityValidationErrors)
                {
                    ms.Add(eve.Entry.Entity.GetType() + " failed validation\n");
                    foreach (var error in eve.ValidationErrors)
                        ms.Add(" - " + error.PropertyName + ":" + error.ErrorMessage);
                }
                Errors.Insert("Entity Validation Failed", string.Join("<br>", ms));
            }
            catch (Exception e)
            {
                Errors.Insert(Errors.Expand(e));
            }
            return View(import);
        }

        bool process_file(string file, Import import, Action<int> progress=null)
        {
            DateTime start_time = DateTime.Now;

            Excel.IExcelDataReader edr;
            FileStream stream = System.IO.File.Open(file, FileMode.Open, FileAccess.Read);
            if (file.EndsWith(".xlsx", StringComparison.InvariantCultureIgnoreCase))
                edr = Excel.ExcelReaderFactory.CreateOpenXmlReader(stream);
            else
                edr = Excel.ExcelReaderFactory.CreateBinaryReader(stream);
            //edr = Excel.ExcelReaderFactory.CreateOpenXmlReader(stream);
            System.Data.DataSet ds = edr.AsDataSet();
            edr.Close();
            
            FhrApi.ProductOffice.Models.ImportMap import_map = db.ImportMaps.Where(r => r.Id == import.MapId).First();
            //if (import_map.C_CompanyProductIdI == null)
            //    throw new Exception("C_CompanyProductIdI in map #" + import_map.Id + " is not specified.");
            //if (import_map.C_NameI == null)
            //    throw new Exception("C_NameI in map #" + import_map.Id + " is not specified.");

            db.Configuration.AutoDetectChangesEnabled = false;
            db.Configuration.ValidateOnSaveEnabled = false;

            int sheet_count = ds.Tables.Count;
            int invalid_product_count = 0;
            int new_product_count = 0;
            int updated_product_count = 0;
            int processed_product_count = 0;
            int invalid_price_count = 0;
            int new_price_count = 0;
            int updated_price_count = 0;
            int processed_price_count = 0;
            int row_number = -1;
            List<string> error_messages = new List<string>();
            foreach (System.Data.DataTable dt in ds.Tables)
            {
                if (import_map.SkipFirstRow)
                    dt.Rows.RemoveAt(0);
                foreach (System.Data.DataRow row in dt.Rows)
                {
                    row_number++;
                    FhrApi.ProductOffice.Models.Product product = new FhrApi.ProductOffice.Models.Product();
                    product.CompanyId = import_map.CompanyId;

                    product.ExternalId = row[import_map.C_CompanyProductIdI].ToString().Trim();
                    if (string.IsNullOrWhiteSpace(product.ExternalId))
                    {
                        bool line_is_empty = true;
                        for (int i = 0; i < 5; i++)
                            if (!string.IsNullOrWhiteSpace(row[i].ToString()))
                            {
                                line_is_empty = false;
                                break;
                            }
                        if (!line_is_empty)
                        {
                            string error = "Cell (" + import_map.C_PriceI + ", " + row_number + ") ExternalId is empty.";
                            if (import.CheckNotImport)
                            {
                                invalid_product_count++;
                                error_messages.Add(error);
                            }
                            else
                                throw new Exception(error);
                        }
                        continue;
                    }

                    product.ModifyTime = null;
                    product.UpdateTime = import.UpdateTime;

                    product.Name = row[import_map.C_NameI].ToString().Trim();
                    if (product.Name == null)
                    {
                        string error = "Cell (" + import_map.C_PriceI + ", " + row_number + ") Name is empty.";
                        if (import.CheckNotImport)
                        {
                            invalid_product_count++;
                            error_messages.Add(error);
                        }
                        else
                            throw new Exception(error);
                    }

                    product.Source = "file:" + import.File.FileName;

                    if (import_map.C_SkuI != null)
                        product.Sku = row[(int)import_map.C_SkuI].ToString().Trim();

                    if (import_map.C_CategoryI != null)
                        product.Category = row[(int)import_map.C_CategoryI].ToString().Trim();

                    if (import_map.C_DescriptionI != null)
                        product.Description = row[(int)import_map.C_DescriptionI].ToString().Trim();

                    if (!import.CheckNotImport)
                    {
                        FhrApi.ProductOffice.Models.Product p = db.Products.Where(r => r.CompanyId == product.CompanyId && r.ExternalId == product.ExternalId).FirstOrDefault();
                        if (p == null)
                        {
                            product.CreateTime = import.UpdateTime;
                            product.LinkId = null;
                            db.Products.Add(product);
                            new_product_count += db.SaveChanges();
                        }
                        else
                        {
                            product.Id = p.Id;
                            if (product.Description != null)
                                p.Description = product.Description;
                            if (product.ImageUrls != null)
                                p.ImageUrls = product.ImageUrls;
                            if (product.ModifyTime != null)
                                p.ModifyTime = product.ModifyTime;
                            p.UpdateTime = product.UpdateTime;
                            if (product.Name != null)
                                p.Name = product.Name;
                            if (product.Sku != null)
                                p.Sku = product.Sku;
                            if (product.Source != null)
                                p.Source = product.Source;
                            if (product.Url != null)
                                p.Url = product.Url;
                            //db.Entry(product).State = EntityState.Modified;
                            updated_product_count += db.SaveChanges();
                        }
                    }
                    processed_product_count++;

                    if (progress != null)
                        progress(processed_product_count);

                    if (import_map.C_PriceI != null)
                    {
                        Price price = new Price();
                        price.CurrencyId = import_map.CurrencyId;
                        price.ProductId = product.Id;
                        price.Time = (DateTime)product.UpdateTime;
                        decimal v;
                        if (!decimal.TryParse(row[(int)import_map.C_PriceI].ToString().Trim(), out v))
                        {
                            string error = "Cell (" + import_map.C_PriceI + ", " + row_number + ") Price cannot be parsed:" + row[(int)import_map.C_PriceI].ToString().Trim();
                            if (import.CheckNotImport)
                            {
                                invalid_price_count++;
                                error_messages.Add(error);
                            }
                            else
                                throw new Exception(error);
                        }

                        price.Value = v;
                        if (!import.CheckNotImport)
                        {
                            Price p = db.Prices.Where(r => r.ProductId == price.ProductId && r.Time == price.Time).FirstOrDefault();
                            if (p == null)
                            {
                                db.Prices.Add(price);
                                new_price_count += db.SaveChanges();
                            }
                            else
                            {
                                price.Id = p.Id;
                                db.Entry(p).CurrentValues.SetValues(price);
                                updated_price_count += db.SaveChanges();
                            }
                        }
                        processed_price_count++;
                    }

                    DbApi.RenewContext(ref db);
                }
            }

            if (import.CheckNotImport)
            {
                Messages.Add("CHECK RESULT", "File " + import.File.FileName + " has been checked with map " + db.ImportMaps.Where(r => r.Id == import.MapId).First().Name);
                Messages.Add("CHECK RESULT", "Spreadsheets: " + sheet_count);
                Messages.Add("CHECK RESULT", "Found products: " + processed_product_count);
                Messages.Add("CHECK RESULT", "Invalid products: " + invalid_product_count);
                Messages.Add("CHECK RESULT", "Invalid prices: " + invalid_price_count);
                Messages.Add("CHECK RESULT", "Process time: " + String.Format("{0:0.00}", (DateTime.Now - start_time).TotalSeconds) + " secs");
                add_error_messages("CHECK RESULT", error_messages);
            }
            else
            {
                Messages.Add("IMPORT RESULT", "File " + import.File.FileName + " has been imported with map " + db.ImportMaps.Where(r => r.Id == import.MapId).First().Name);
                Messages.Add("IMPORT RESULT", "Spreadsheets: " + sheet_count);
                Messages.Add("IMPORT RESULT", "Processed products: " + processed_product_count);
                Messages.Add("IMPORT RESULT", "Invalid products: " + invalid_product_count);
                Messages.Add("IMPORT RESULT", "Added products: " + new_product_count);
                Messages.Add("IMPORT RESULT", "Changed products: " + updated_product_count);
                Messages.Add("IMPORT RESULT", "Invalid prices: " + invalid_price_count);
                Messages.Add("IMPORT RESULT", "Added prices: " + new_price_count);
                Messages.Add("IMPORT RESULT", "Changed prices: " + updated_price_count);
                Messages.Add("CHECK RESULT", "Process time: " + String.Format("{0:0.00}", (DateTime.Now - start_time).TotalSeconds) + " secs");
                add_error_messages("IMPORT RESULT", error_messages);
            }

            return true;
        }

        void add_error_messages(string title, List<string> error_messages)
        {
            if (error_messages.Count < 1)
            {
                Messages.Add(title, "No error found.");
                return;
            }
            int max_count = 30;
            int count = error_messages.Count;
            string notification = null;
            if (count > max_count)
            {
                count = max_count;
                notification = "(Only first " + count + " errors displayed)<br>";
            }
            Messages.Add(title, "<p style='color:#c03;'>ERRORS:<br>" + notification + string.Join("<br/>", error_messages.Take(count)) + "</p>");
        }
    }
}