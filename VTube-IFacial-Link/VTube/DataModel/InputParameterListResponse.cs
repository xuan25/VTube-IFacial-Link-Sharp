namespace VTube.DataModel
{
    class InputParameterListResponse : ResponseBase
    {
        public class DataSection : ErrorInfoBase
        {
            public class Parameter
            {
                public string Name { get; set; }
                public string AddedBy { get; set; }
                public float Value { get; set; }
                public float Min { get; set; }
                public float Max { get; set; }
                public float DefaultValue { get; set; }
            }

            public bool ModelLoaded { get; set; }
            public string ModelName { get; set; }
            public List<Parameter> CustomParameters { get; set; }
            public List<Parameter> DefaultParameters { get; set; }
        }

        public DataSection Data { get; set; }
    }
}
