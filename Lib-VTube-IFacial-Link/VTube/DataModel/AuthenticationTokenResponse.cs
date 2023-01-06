namespace VTube.DataModel
{
    public class AuthenticationTokenResponse : ResponseBase
    {
        public class DataSection : ErrorInfoBase
        {
            public string AuthenticationToken { get; set; }
        }

        public DataSection Data { get; set; }
    }
}
