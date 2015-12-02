using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;
using System.IO;
using System.Text.RegularExpressions;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Cliver.Bot;

namespace Cliver.CrawlerHost
{
    public class Settings
    {
        const string KEY = "SETTINGS";

      static  public void Save(string scope, Dictionary<string, object> setting_names2value)
        {
            DbApi db = DbApi.Create();
            DbSettings dbs = new DbSettings(db);
            dbs.Save(scope, KEY, setting_names2value);
        }

       static public void SettingsLoadedEventHandler(System.Configuration.ApplicationSettingsBase settings)
        {
            Assembly calling_assembly = Assembly.GetCallingAssembly();

            DbApi db = DbApi.Create();
            DbSettings dbs = new DbSettings(db);
            string scope = calling_assembly.GetName().Name;
            Dictionary<string, object> setting_names2value = dbs.Get<Dictionary<string, object>>(scope, KEY);

            //Configuration c = ConfigurationManager.OpenExeConfiguration(calling_assembly.CodeBase);
            //AppSettingsSection ass = c.AppSettings;
            FieldInfo fi = settings.GetType().GetField("defaultInstance", BindingFlags.NonPublic | BindingFlags.Static);
            foreach (string name in setting_names2value.Keys)
            {
                object value = setting_names2value[name];
                try
                {
                    PropertyInfo pi = fi.FieldType.GetProperty(name);
                    if (pi == null)
                        Log.Warning("Setting '" + scope + "::" + name + "' could not be set as it does not exists.");
                    if (pi.PropertyType == typeof(int) && value is string)
                        value = int.Parse((string)value);
                    ((global::System.Configuration.ApplicationSettingsBase)fi.GetValue(null))[name] = value;
                    continue;
                }
                catch { }
                LogMessage.Error("Could not set '" + scope + "::" + name + "' to " + value.ToString());
            }
        }
    }
}

