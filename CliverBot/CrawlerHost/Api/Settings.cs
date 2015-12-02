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
        const string KEY_PREFIX = "__SETTINGS__";

        static public void Save(string scope, string key, Dictionary<string, object> setting_names2value)
        {
            Cliver.Bot.DbSettings.Save(DbApi.Create(), scope, key, setting_names2value);
        }

        static public void LoadFromDatabase(Assembly assembly, System.Configuration.ApplicationSettingsBase settings)
        {
            Assembly entry_assembly = Assembly.GetEntryAssembly();
            string scope = entry_assembly.GetName().Name;
            string key = KEY_PREFIX + assembly.GetName().Name;
            DbApi di = DbApi.Create();
            Dictionary<string, object> setting_names2value = Cliver.Bot.DbSettings.Get<Dictionary<string, object>>(di, scope, key);
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
            Cliver.Bot.DbSettings.Save(di, scope, key, setting_names2value2);
        }

        //public static void Load()
        //{
        //    AppDomain.CurrentDomain.AssemblyLoad += CurrentDomain_AssemblyLoad;
        //    foreach (Assembly a in AppDomain.CurrentDomain.GetAssemblies())
        //        set_Settings_SettingsLoaded(a);
        //}

        //static void CurrentDomain_AssemblyLoad(object sender, AssemblyLoadEventArgs args)
        //{
        //    set_Settings_SettingsLoaded(args.LoadedAssembly);
        //}

        //static void set_Settings_SettingsLoaded(Assembly a)
        //{
        //    string name = a.FullName;
        //    if (!Regex.IsMatch(name, "Fhr|Cliver"))
        //        return;
        //   foreach(Type t in a.GetTypes().Where(t => t.BaseType == typeof(global::System.Configuration.ApplicationSettingsBase)))
        //   {
        //       EventInfo ei = t.GetEvent("SettingsLoaded");
        //       MethodInfo mi = typeof(Settings).GetMethod("Settings_SettingsLoaded", BindingFlags.NonPublic | BindingFlags.Static);
        //       Delegate d = Delegate.CreateDelegate(ei.EventHandlerType, mi);               
        //       ei.AddEventHandler(null, d);
        //   }
        //}
                
        //static void Settings_SettingsLoaded(object sender, System.Configuration.SettingsLoadedEventArgs e)
        //{
        //    //Cliver.CrawlerHost.Settings.SettingsLoadedEventHandler(e.);
        //}
    }
}