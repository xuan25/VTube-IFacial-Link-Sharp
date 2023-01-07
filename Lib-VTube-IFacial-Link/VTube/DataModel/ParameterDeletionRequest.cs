namespace VTube.DataModel
{
    public class ParameterDeletionRequest : RequestBase
    {
        public class DataSection
        {
            public string ParameterName { get; set; }
            
            public DataSection(string parameterName)
            {
                ParameterName = parameterName;
            }
        }

        public DataSection Data { get; set; }

        public ParameterDeletionRequest(string parameterName) : base("ParameterDeletionRequest")
        {
            Data = new DataSection(parameterName);
        }
    }
}
