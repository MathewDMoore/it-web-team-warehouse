using System.Configuration;
using ApplicationSource.Interfaces;

namespace ApplicationSource
{
    public class SettingsWrapper:ISettings
    {
        public string GetServerLocation {
            get { return ConfigurationManager.AppSettings["ServerLocation"]; }
        }
    }
}