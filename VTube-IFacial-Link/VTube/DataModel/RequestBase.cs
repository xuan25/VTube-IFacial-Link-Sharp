namespace VTube.DataModel
{
    class RequestBase
    {
        public string apiName { get; set; } = "VTubeStudioPublicAPI";
        public string apiVersion { get; set; } = "1.0";
        public string requestID { get; set; } = "VTubeIFacialLink";
        public string messageType { get; set; }

        public RequestBase(string messageType)
        {
            this.messageType = messageType;
        }
    }
}
