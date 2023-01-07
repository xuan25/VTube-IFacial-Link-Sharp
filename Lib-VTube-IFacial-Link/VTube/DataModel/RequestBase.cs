namespace VTube.DataModel
{
    public class RequestBase
    {
        public string ApiName { get; set; } = "VTubeStudioPublicAPI";
        public string ApiVersion { get; set; } = "1.0";
        public string RequestID { get; set; } = "VTubeIFacialLink";
        public string MessageType { get; set; }

        public RequestBase(string messageType)
        {
            MessageType = messageType;
        }
    }
}
