using System.Collections.ObjectModel;
using VTube.Interfaces;

namespace VTube_IFacial_Link.DataModels
{
    public class ScriptMetaCollection<T> : ObservableCollection<T>, IScriptMetaColleciton<T> where T : IScriptMeta { }

}
