using static VTube.DataModel.InjectParameterDataRequest.Data;

namespace VTube.DataModel
{
    class InjectParameterDataRequest : RequestBase
    {
        public class Data
        {
            public class ParameterValue
            {
                public string id { get; set; }
                public double weight { get; set; }
                public double value { get; set; }

                public ParameterValue(string id, double weight, double value)
                {
                    this.id = id;
                    this.weight = weight;
                    this.value = value;
                }

                public ParameterValue(Params id, double weight, double value)
                {
                    this.id = Enum.GetName(typeof(Params), id);
                    this.weight = weight;
                    this.value = value;
                }
            }

            public bool faceFound { get; set; }
            public string mode { get; set; } = "set";
            public List<ParameterValue> parameterValues { get; set; }

            public Data(bool faceFound, string mode, List<ParameterValue> parameterValues)
            {
                this.faceFound = faceFound;
                this.mode = mode;
                this.parameterValues = parameterValues;
            }
        }

        public Data data { get; set; }

        public InjectParameterDataRequest(bool faceFound, string mode, List<ParameterValue> parameterValues) : base("InjectParameterDataRequest")
        {
            data = new Data(faceFound, mode, parameterValues);
        }
    }
}
