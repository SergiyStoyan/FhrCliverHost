using System.Collections.Generic;
using System.Reflection;

namespace Cliver.FhrManager.Properties
{
    
    
    // This class allows you to handle specific events on the settings class:
    //  The SettingChanging event is raised before a setting's value is changed.
    //  The PropertyChanged event is raised after a setting's value is changed.
    //  The SettingsLoaded event is raised after the setting values are loaded.
    //  The SettingsSaving event is raised before the setting values are saved.
    internal sealed partial class Settings {
        
        public Settings() {
            this.SettingsLoaded += Settings_SettingsLoaded;
        }

        void Settings_SettingsLoaded(object sender, System.Configuration.SettingsLoadedEventArgs e)
        {
            Cliver.CrawlerHost.Settings.SettingsLoadedEventHandler(this);
        }
    }
}
