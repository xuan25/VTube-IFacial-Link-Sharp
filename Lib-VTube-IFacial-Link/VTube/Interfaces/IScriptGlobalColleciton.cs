using System.Collections.Specialized;

namespace VTube.Interfaces
{
    public interface IScriptGlobalColleciton<out T> : IEnumerable<T>, INotifyCollectionChanged where T : IScriptGlobal { }
}
