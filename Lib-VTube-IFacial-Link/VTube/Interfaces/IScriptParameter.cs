namespace VTube.Interfaces
{
    public interface IScriptParameter : IParameter, IScriptMeta
    {
        public string RecentError { get; set; }
        public double RecentValue { get; set; }

        public event EventHandler ScriptUpdated;
    }
}
