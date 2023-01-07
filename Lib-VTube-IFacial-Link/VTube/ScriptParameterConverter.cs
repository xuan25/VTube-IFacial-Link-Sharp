using IFacial;
using MoonSharp.Interpreter;
using System.Collections.Specialized;
using VTube.DataModel;
using VTube.Interfaces;

namespace VTube
{
    public class ScriptParameterConverter : IDisposable, IParameterConverter
    {

        public IScriptGlobalColleciton<IScriptGlobal> ScriptGlobals { get; private set; }

        public IScriptParameterColleciton<IScriptParameter> ScriptParameters { get; private set; }

        public IParameterColleciton<IParameter> Parameters
        {
            get
            {
                return ScriptParameters;
            }
        }

        private Dictionary<IScriptParameter, DynValue> CallMap { get; set; }

        Script ScriptSession { get; set; }

        public ScriptParameterConverter(IScriptParameterColleciton<IScriptParameter> parameters, IScriptGlobalColleciton<IScriptGlobal> globals)
        {
            ScriptParameters = parameters;
            ScriptGlobals = globals;
            foreach (IScriptParameter param in ScriptParameters)
            {
                param.ScriptUpdated += Param_ScriptUpdated;
            }
            foreach (IScriptGlobal global in ScriptGlobals)
            {
                global.ValueUpdated += Global_ValueUpdated;
            }
            ScriptParameters.CollectionChanged += ScriptParameters_CollectionChanged;
            ScriptGlobals.CollectionChanged += ScriptGlobals_CollectionChanged;
            InitScriptSession();
        }

        private void ScriptParameters_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add || e.Action == NotifyCollectionChangedAction.Replace)
            {
                foreach (IScriptParameter param in e.NewItems)
                {
                    param.ScriptUpdated += Param_ScriptUpdated;
                    try
                    {
                        CallMap.Add(param, ScriptSession.LoadString(param.Script.Replace('\r', '\n')));
                    }
                    catch (Exception ex)
                    {
                        CallMap.Remove(param);
                        param.RecentError = ex.Message;
                        // throw new Exception($"Script Error ({param.Name}): {ex.Message}", ex);
                    }
                }
            }
            if (e.Action == NotifyCollectionChangedAction.Remove || e.Action == NotifyCollectionChangedAction.Replace)
            {
                foreach (IScriptParameter param in e.OldItems)
                {
                    param.ScriptUpdated -= Param_ScriptUpdated;
                    CallMap.Remove(param);
                    // Note: Unable to release resource in Lua
                }
            }
            if (e.Action == NotifyCollectionChangedAction.Reset)
            {
                InitScriptSession();
            }
        }

        private void ScriptGlobals_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            if(e.Action == NotifyCollectionChangedAction.Add || e.Action == NotifyCollectionChangedAction.Replace)
            {
                foreach(IScriptGlobal global in e.NewItems)
                {
                    global.ValueUpdated += Global_ValueUpdated;
                    ScriptSession.Globals[global.Name] = global.Value;
                }
            }
            if (e.Action == NotifyCollectionChangedAction.Remove || e.Action == NotifyCollectionChangedAction.Replace)
            {
                foreach (IScriptGlobal global in e.OldItems)
                {
                    global.ValueUpdated -= Global_ValueUpdated;
                    ScriptSession.Globals.Remove(global.Name);
                }
            }
            if (e.Action == NotifyCollectionChangedAction.Reset)
            {
                InitScriptSession();
            }
        }

        private void Global_ValueUpdated(object? sender, EventArgs e)
        {
            IScriptGlobal global = (IScriptGlobal)sender;
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

            foreach (IScriptGlobal global in ScriptGlobals)
            {
                scriptSession.Globals[global.Name] = global.Value;
            }

            Dictionary<IScriptParameter, DynValue> callMap = new Dictionary<IScriptParameter, DynValue>();
            foreach (IScriptParameter param in ScriptParameters)
            {
                try
                {
                    callMap.Add(param, scriptSession.LoadString(param.Script.Replace('\r', '\n')));
                }
                catch (Exception ex)
                {
                    callMap.Remove(param);
                    param.RecentError = ex.Message;
                    // throw new Exception($"Script Error ({param.Name}): {ex.Message}", ex);
                }
            }

            ScriptSession = scriptSession;
            CallMap = callMap;
        }

        public List<InjectParameterDataRequest.DataSection.ParameterValue> Convert(CapturedData capturedData)
        {
            ScriptSession.Globals["data"] = capturedData;

            List<InjectParameterDataRequest.DataSection.ParameterValue> parameterValues = new List<InjectParameterDataRequest.DataSection.ParameterValue>();
            lock (ScriptParameters)
            {
                foreach (IScriptParameter param in ScriptParameters)
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
                        // throw new Exception($"Script Error ({param.Name}): {ex.Message}", ex);
                    }
                }
            }

            return parameterValues;
        }

        public void Dispose()
        {
            ScriptParameters.CollectionChanged -= ScriptParameters_CollectionChanged;
            ScriptGlobals.CollectionChanged -= ScriptGlobals_CollectionChanged;
        }
    }
}
