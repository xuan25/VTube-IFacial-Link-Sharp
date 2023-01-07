namespace VTube.DataModel
{
    public class ParameterCreationResponse : ResponseBase
    {
        public class DataSection : ErrorInfoBase
        {
            public string ParameterName { get; set; }
        }

        public DataSection Data { get; set; }
    }
}
