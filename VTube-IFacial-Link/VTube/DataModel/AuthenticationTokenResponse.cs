namespace VTube.DataModel
{
    class AuthenticationTokenResponse : ResponseBase
    {
        public class Data : ErrorInfoBase
        {
            public string authenticationToken { get; set; }
        }

        public Data data { get; set; }
    }
}
