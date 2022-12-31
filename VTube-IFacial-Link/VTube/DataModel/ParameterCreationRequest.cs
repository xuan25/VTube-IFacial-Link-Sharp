namespace VTube.DataModel
{
    class ParameterCreationRequest : RequestBase
    {
        public class DataSection
        {
            public string ParameterName { get; set; }
            public string Explanation { get; set; }
            public float Min { get; set; }
            public float Max { get; set; }
            public float DefaultValue { get; set; }

            public DataSection(string parameterName, string explanation, float min, float max, float defaultValue)
            {
                ParameterName = parameterName;
                Explanation = explanation;
                Min = min;
                Max = max;
                DefaultValue = defaultValue;
            }
        }

        public DataSection Data { get; set; }

        public ParameterCreationRequest(string parameterName, string explanation, float min, float max, float defaultValue) : base("ParameterCreationRequest")
        {
            Data = new DataSection(parameterName, explanation, min, max, defaultValue);
        }
    }
}
