namespace VTube.Interfaces
{
    public interface IScriptGlobal : IScriptMeta
    {
        public string Name { get; set; }
        public double Value { get; set; }

        public event EventHandler ValueUpdated;
    }
}
