using static VTube.DataModel.InjectParameterDataRequest.DataSection;

namespace VTube.DataModel
{
    class InjectParameterDataRequest : RequestBase
    {
        public class DataSection
        {
            public class ParameterValue
            {
                public string Id { get; set; }
                public double Weight { get; set; }
                public double Value { get; set; }

                public ParameterValue(string id, double weight, double value)
                {
                    Id = id;
                    Weight = weight;
                    Value = value;
                }

                public ParameterValue(Params id, double weight, double value)
                {
                    Id = Enum.GetName(typeof(Params), id);
                    Weight = weight;
                    Value = value;
                }
            }

            public bool FaceFound { get; set; }
            public string Mode { get; set; } = "set";
            public List<ParameterValue> ParameterValues { get; set; }

            public DataSection(bool faceFound, string mode, List<ParameterValue> parameterValues)
            {
                FaceFound = faceFound;
                Mode = mode;
                ParameterValues = parameterValues;
            }
        }

        public DataSection Data { get; set; }

        public InjectParameterDataRequest(bool faceFound, string mode, List<ParameterValue> parameterValues) : base("InjectParameterDataRequest")
        {
            Data = new DataSection(faceFound, mode, parameterValues);
        }
    }
}
