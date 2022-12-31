namespace VTube.DataModel
{
    class ParameterCreationResponse : ResponseBase
    {
        public class Data : ErrorInfoBase
        {
            public string parameterName { get; set; }
        }

        public Data data { get; set; }
    }
}
