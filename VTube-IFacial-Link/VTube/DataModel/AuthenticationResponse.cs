namespace VTube.DataModel
{
    class AuthenticationResponse : ResponseBase
    {
        public class Data : ErrorInfoBase
        {
            public bool authenticated { get; set; }
            public string reason { get; set; }
        }

        public Data data { get; set; }
    }
}
