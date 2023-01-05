using IFacial;
using MoonSharp.Interpreter;
using System.Text.Json;
using VTube.DataModel;
using static VTube.VTubeClient;

namespace VTube
{
    public class ParameterConverter
    {
        public interface IGlobal
        {
            public string Name { get; set; }
            public double Value { get; set; }

            public event EventHandler ValueUpdated;
        }

        public interface IParameter
        {
            public string Name { get; set; }
            public string Script { get; set; }
            public string RecentError { get; set; }
            public double RecentValue { get; set; }

            public event EventHandler ScriptUpdated;
        }

        public IEnumerable<IGlobal> Globals { get; set; }
    //{
    //    { "FACE_POSITION_X_RATIO", 100 },
    //    { "FACE_POSITION_Y_RATIO", 100 },
    //    { "FACE_POSITION_Z_RATIO", 20 },

    //    { "FACE_ANGLE_X_RATIO", 1 },
    //    { "FACE_ANGLE_Y_RATIO", 1 },
    //    { "FACE_ANGLE_Z_RATIO", 1 },

    //    { "MOUTH_SMILE_RATIO", 2 },
    //    { "MOUTH_OPEN_RATIO", 1.2 },

    //    { "BROWS_RATIO", 2 },

    //    { "TONGUE_OUT_RATIO", 0.4 },

    //    { "EYE_OPEN_RATIO", 1.25 },
    //    { "EYE_ROTATION_RATIO", 1.5 },

    //    { "CHEEK_PUFF_RATIO", 2 },
    //    { "FACE_ANGRY_RATIO", 0.3 },

    //    { "BROW_LEFT_Y_RATIO", 2 },
    //    { "BROW_RIGHT_Y_RATIO", 2 },

    //    { "MOUTH_X_RATIO", 2 }
    //};

    public IEnumerable<IParameter> Params { get; set; }

        private Dictionary<IParameter, DynValue> CallMap { get; set; }

        Script ScriptSession { get; set; }
        public ParameterConverter(IEnumerable<IParameter> parameters, IEnumerable<IGlobal> globals)
        {
            Params = parameters;
            Globals = globals;
            foreach (IParameter param in Params)
            {
                param.ScriptUpdated += Param_ScriptUpdated;
            }
            foreach (IGlobal global in Globals)
            {
                global.ValueUpdated += Global_ValueUpdated;
            }
            InitScriptSession();
        }

        private void Global_ValueUpdated(object? sender, EventArgs e)
        {
            IGlobal global = (IGlobal)sender;
            ScriptSession.Globals[global.Name] = global.Value;
        }

        private void Param_ScriptUpdated(object? sender, EventArgs e)
        {
            InitScriptSession();
        }

        public void InitScriptSession()
        {
            UserData.RegisterType<CapturedData>();
            Script scriptSession = new Script();

            foreach (IGlobal global in Globals)
            {
                scriptSession.Globals[global.Name] = global.Value;
            }

            Dictionary<IParameter, DynValue> callMap = new Dictionary<IParameter, DynValue>();
            foreach (IParameter param in Params)
            {
                try
                {
                    callMap.Add(param, scriptSession.LoadString(param.Script.Replace('\r', '\n')));
                }
                catch (Exception ex)
                {
                    callMap.Remove(param);
                    param.RecentError = ex.Message;
                    //throw new Exception($"Script Error ({param.Name}): {ex.Message}", ex);
                }
            }

            ScriptSession = scriptSession;
            CallMap = callMap;
        }

        internal List<InjectParameterDataRequest.DataSection.ParameterValue> Convert(CapturedData capturedData)
        {
            ScriptSession.Globals.Remove("capturedData");
            ScriptSession.Globals["capturedData"] = capturedData;

            List<InjectParameterDataRequest.DataSection.ParameterValue> parameterValues = new List<InjectParameterDataRequest.DataSection.ParameterValue>();
            foreach (IParameter param in Params)
            {
                try
                {
                    if (CallMap.ContainsKey(param))
                    {
                        DynValue res = ScriptSession.Call(CallMap[param]);
                        double val = res.Number;
                        param.RecentValue = val;
                        param.RecentError = null;
                        parameterValues.Add(new InjectParameterDataRequest.DataSection.ParameterValue(param.Name, 1, val));
                    }
                    else
                    {
                        if(param.RecentError == null)
                        {
                            param.RecentError = "Syntax Error";
                        }
                    }
                }
                catch (Exception ex)
                {
                    param.RecentError = ex.Message;
                    //throw new Exception($"Script Error ({param.Name}): {ex.Message}", ex);
                }
            }

            return parameterValues;
        }
    }
}
