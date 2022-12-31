namespace VTube.DataModel
{
    class InputParameterListResponse : ResponseBase
    {
        public class Data : ErrorInfoBase
        {
            public class Parameter
            {
                public string name { get; set; }
                public string addedBy { get; set; }
                public float value { get; set; }
                public float min { get; set; }
                public float max { get; set; }
                public float defaultValue { get; set; }
            }

            public bool modelLoaded { get; set; }
            public string modelName { get; set; }
            public List<Parameter> customParameters { get; set; }
            public List<Parameter> defaultParameters { get; set; }
        }

        public Data data { get; set; }
    }
}
