namespace VTube.DataModel
{
    class AuthenticationRequest : RequestBase
    {
        public class Data
        {
            public string pluginName { get; set; } = Constants.PLUGIN_NAME;
            public string pluginDeveloper { get; set; } = Constants.PLUGIN_DEVELOPER;
            public string authenticationToken { get; set; }

            public Data(string authenticationToken)
            {
                this.authenticationToken = authenticationToken;
            }
        }

        public Data data { get; set; }

        public AuthenticationRequest(string authenticationToken) : base("AuthenticationRequest")
        {
            this.data = new Data(authenticationToken);
        }
    }
}
