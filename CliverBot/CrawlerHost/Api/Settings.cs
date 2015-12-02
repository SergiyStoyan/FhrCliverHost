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
        const string KEY = "__SETTINGS__";

        static public void Save(string scope, Dictionary<string, object> setting_names2value)
        {
            DbSettings dss = new DbSettings(DbApi.Create());
            dss.Save(scope, KEY, setting_names2value);
        }

        static public void SettingsLoadedEventHandler(System.Configuration.ApplicationSettingsBase settings)
        {
            Assembly calling_assembly = Assembly.GetCallingAssembly();

            DbSettings dss = new DbSettings(DbApi.Create());
            string scope = calling_assembly.GetName().Name;
            Dictionary<string, object> setting_names2value = dss.Get<Dictionary<string, object>>(scope, KEY);
            if (setting_names2value == null)
                setting_names2value = new Dictionary<string, object>();

            FieldInfo fi = settings.GetType().GetField("defaultInstance", BindingFlags.NonPublic | BindingFlags.Static);
            Dictionary<string, object> setting_names2value2 = new Dictionary<string, object>();
            PropertyInfo[] pis = fi.FieldType.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            foreach (PropertyInfo pi in pis)
            {
                object value;
                if (setting_names2value.TryGetValue(pi.Name, out value))
                {
                    if (pi.PropertyType == typeof(int) && value is string)
                        value = int.Parse((string)value);
                    //((global::System.Configuration.ApplicationSettingsBase)fi.GetValue(null))[pi.Name] = value;
                    pi.SetValue(settings, value);
                }
                else
                    value = pi.GetValue(settings);
                setting_names2value2[pi.Name] = value;
            }
            dss.Save(scope, KEY, setting_names2value2);
        }
    }
}