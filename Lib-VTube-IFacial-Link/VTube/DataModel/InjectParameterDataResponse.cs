namespace VTube.DataModel
{
    class InjectParameterDataResponse : ResponseBase
    {
        public class DataSection : ErrorInfoBase
        {
        }

        public DataSection Data { get; set; }
    }
}
