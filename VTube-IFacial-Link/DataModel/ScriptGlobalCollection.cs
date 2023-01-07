using System.Collections.ObjectModel;
using VTube.Interfaces;

namespace VTube_IFacial_Link.DataModel
{
    public class ScriptGlobalCollection<T> : ObservableCollection<T>, IScriptGlobalColleciton<T> where T : IScriptGlobal { }

}
