using System.Collections.ObjectModel;
using VTube.Interfaces;

namespace VTube_IFacial_Link.DataModels
{
    public class ScriptParameterCollection<T> : ObservableCollection<T>, IScriptParameterColleciton<T> where T : IScriptParameter { }

}
