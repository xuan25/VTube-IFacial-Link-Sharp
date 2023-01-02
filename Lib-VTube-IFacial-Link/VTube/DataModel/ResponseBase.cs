namespace VTube.DataModel
{
    class ResponseBase
    {
        public string ApiName { get; set; }
        public string ApiVersion { get; set; }
        public long Timestamp { get; set; }
        public string MessageType { get; set; }
        public string RequestID { get; set; }

        public class ErrorInfoBase
        {
            public int ErrorID { get; set; }
            public string Message { get; set; }
        }
    }
}
