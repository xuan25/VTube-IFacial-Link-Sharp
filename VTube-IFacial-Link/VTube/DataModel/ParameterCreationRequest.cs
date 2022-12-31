namespace VTube.DataModel
{
    class ParameterCreationRequest : RequestBase
    {
        public class Data
        {
            public string parameterName { get; set; }
            public string explanation { get; set; }
            public float min { get; set; }
            public float max { get; set; }
            public float defaultValue { get; set; }

            public Data(string parameterName, string explanation, float min, float max, float defaultValue)
            {
                this.parameterName = parameterName;
                this.explanation = explanation;
                this.min = min;
                this.max = max;
                this.defaultValue = defaultValue;
            }
        }

        public Data data { get; set; }

        public ParameterCreationRequest(string parameterName, string explanation, float min, float max, float defaultValue) : base("ParameterCreationRequest")
        {
            this.data = new Data(parameterName, explanation, min, max, defaultValue);
        }
    }
}
