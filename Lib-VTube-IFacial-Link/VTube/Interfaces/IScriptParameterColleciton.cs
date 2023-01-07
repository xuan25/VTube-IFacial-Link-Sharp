using System.Collections.Specialized;

namespace VTube.Interfaces
{
    public interface IScriptParameterColleciton<out T> : IEnumerable<T>, INotifyCollectionChanged, IParameterColleciton<T> where T : IScriptParameter { }
}
