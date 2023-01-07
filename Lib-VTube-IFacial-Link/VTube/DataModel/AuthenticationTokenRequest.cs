namespace VTube.DataModel
{
    public class AuthenticationTokenRequest : RequestBase
    {
        public class DataSection
        {
            public string PluginName { get; set; } = Constants.PLUGIN_NAME;
            public string PluginDeveloper { get; set; } = Constants.PLUGIN_DEVELOPER;
            public string PluginIcon { get; set; } = Constants.PLUGIN_ICON;
        }

        public DataSection Data { get; set; } = new DataSection();

        public AuthenticationTokenRequest() : base("AuthenticationTokenRequest") { }
    }
}
