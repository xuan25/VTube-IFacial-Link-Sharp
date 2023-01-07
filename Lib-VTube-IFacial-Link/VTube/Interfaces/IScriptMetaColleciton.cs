using System.Collections.Specialized;

namespace VTube.Interfaces
{
    public interface IScriptMetaColleciton<out T> : IEnumerable<T>, INotifyCollectionChanged where T : IScriptMeta { }
}
