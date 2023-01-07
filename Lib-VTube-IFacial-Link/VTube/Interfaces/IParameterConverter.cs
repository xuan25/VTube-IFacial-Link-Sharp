using IFacial;
using VTube.DataModel;

namespace VTube.Interfaces
{
    public interface IParameterConverter : IDisposable
    {
        public IParameterColleciton<IParameter> Parameters { get; }
        public List<InjectParameterDataRequest.DataSection.ParameterValue> Convert(CapturedData capturedData);
    }
}
