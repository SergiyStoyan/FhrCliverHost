using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Web;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Reflection;

namespace Cliver.ProductIdentifier
{
    public class Companies
    {
        internal Companies(Engine engine)
        {
            this.engine = engine;
        }
        readonly Engine engine;

        internal Company Get(Product product)
        {
            return Get(product.DbProduct.CompanyId);
        }

        public Company Get(int company_id)
        {
            Company c = null;
            if (!company_ids2Company.TryGetValue(company_id, out c))
            {
                Fhr.ProductOffice.Models.Company dc = engine.Db.Companies.Where(x => x.Id == company_id).FirstOrDefault();
                if (dc == null)
                    throw new Exception("No Company for Id=" + company_id);
                c = new Company(engine, dc);
                company_ids2Company[company_id] = c;
                engine.Products.Ininitalize(dc.Products);

                if (engine.AutoDataAnalysing && c.IsDataAnalysisRequired())
                {
                    engine.PerformDataAnalysis(company_id);
                    c = new Company(engine, dc);
                    company_ids2Company[company_id] = c;
                    engine.Products.Ininitalize(dc.Products);
                }
            }
            return c;
        }
        Dictionary<int, Company> company_ids2Company = new Dictionary<int, Company>();

        internal Dictionary<int, Company>.ValueCollection All
        {
            get
            {
                return company_ids2Company.Values;
            }
        }
    }
}

