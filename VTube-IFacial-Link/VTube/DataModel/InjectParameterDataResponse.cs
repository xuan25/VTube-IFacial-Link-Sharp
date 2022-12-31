namespace VTube.DataModel
{
    class InjectParameterDataResponse : ResponseBase
    {
        public class Data : ErrorInfoBase
        {
        }

        public Data data { get; set; }
    }
}
