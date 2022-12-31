namespace VTube.DataModel
{
    class AuthenticationTokenRequest : RequestBase
    {
        public class Data
        {
            public string pluginName { get; set; } = Constants.PLUGIN_NAME;
            public string pluginDeveloper { get; set; } = Constants.PLUGIN_DEVELOPER;
            public string pluginIcon { get; set; } = Constants.PLUGIN_ICON;
        }

        public Data data { get; set; } = new Data();

        public AuthenticationTokenRequest() : base("AuthenticationTokenRequest") { }
    }
}
