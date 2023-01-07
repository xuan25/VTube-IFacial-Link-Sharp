using Microsoft.UI.Xaml;
using System;
using System.ComponentModel;
using VTube;
using VTube.Interfaces;

namespace VTube_IFacial_Link.DataModel
{
    public class ScriptGlobalModel : DependencyObject, IScriptGlobal, INotifyPropertyChanged
    {
        public event EventHandler ValueUpdated;

        private string _name;
        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value;
                DispatcherQueue.TryEnqueue(Microsoft.UI.Dispatching.DispatcherQueuePriority.Normal, () =>
                {
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Name)));
                });
            }
        }

        public double _Value;
        public double Value
        {
            get
            {
                return _Value;
            }
            set
            {
                _Value = value;
                ValueUpdated?.Invoke(this, EventArgs.Empty);
                DispatcherQueue.TryEnqueue(Microsoft.UI.Dispatching.DispatcherQueuePriority.Normal, () =>
                {
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Value)));
                });
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
