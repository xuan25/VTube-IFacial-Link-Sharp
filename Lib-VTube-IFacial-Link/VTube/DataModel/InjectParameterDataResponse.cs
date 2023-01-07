namespace VTube.DataModel
{
    public class InjectParameterDataResponse : ResponseBase
    {
        public class DataSection : ErrorInfoBase
        {
        }

        public DataSection Data { get; set; }
    }
}
