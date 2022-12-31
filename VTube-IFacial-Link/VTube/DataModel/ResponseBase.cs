namespace VTube.DataModel
{
    class ResponseBase
    {
        public string apiName { get; set; }
        public string apiVersion { get; set; }
        public long timestamp { get; set; }
        public string messageType { get; set; }
        public string requestID { get; set; }

        public class ErrorInfoBase
        {
            public int errorID { get; set; }
            public string message { get; set; }
        }
    }
}
