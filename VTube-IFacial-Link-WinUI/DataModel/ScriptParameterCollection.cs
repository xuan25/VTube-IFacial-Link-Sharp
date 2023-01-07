using System.Collections.ObjectModel;
using VTube.Interfaces;

namespace VTube_IFacial_Link.DataModel
{
    public class ScriptParameterCollection<T> : ObservableCollection<T>, IScriptParameterColleciton<T> where T : IScriptParameter { }

}
