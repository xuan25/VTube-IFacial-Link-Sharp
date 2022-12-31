namespace VTube.DataModel
{
    class AuthenticationRequest : RequestBase
    {
        public class DataSection
        {
            public string PluginName { get; set; } = Constants.PLUGIN_NAME;
            public string PluginDeveloper { get; set; } = Constants.PLUGIN_DEVELOPER;
            public string AuthenticationToken { get; set; }

            public DataSection(string authenticationToken)
            {
                AuthenticationToken = authenticationToken;
            }
        }

        public DataSection Data { get; set; }

        public AuthenticationRequest(string authenticationToken) : base("AuthenticationRequest")
        {
            Data = new DataSection(authenticationToken);
        }
    }
}
