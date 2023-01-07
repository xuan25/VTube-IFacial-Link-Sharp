namespace VTube.DataModel
{
    public class AuthenticationResponse : ResponseBase
    {
        public class DataSection : ErrorInfoBase
        {
            public bool Authenticated { get; set; }
            public string Reason { get; set; }
        }

        public DataSection Data { get; set; }
    }
}
