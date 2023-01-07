using System.Collections.Specialized;

namespace VTube.Interfaces
{
    public interface IParameterColleciton<out T> : IEnumerable<T>, INotifyCollectionChanged where T : IParameter { }
}
