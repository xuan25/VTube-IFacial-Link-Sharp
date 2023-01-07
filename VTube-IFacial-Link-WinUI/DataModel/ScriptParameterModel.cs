using Microsoft.UI.Xaml;
using System;
using System.ComponentModel;
using VTube;
using VTube.Interfaces;

namespace VTube_IFacial_Link.DataModel
{
    public class ScriptParameterModel : DependencyObject, IScriptParameter, INotifyPropertyChanged
    {
        public event EventHandler ScriptUpdated;

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

        private string _script;
        public string Script
        {
            get
            {
                return _script;
            }
            set
            {
                _script = value;
                ScriptUpdated?.Invoke(this, EventArgs.Empty);
                ScriptUpdated?.Invoke(this, EventArgs.Empty);
                DispatcherQueue.TryEnqueue(Microsoft.UI.Dispatching.DispatcherQueuePriority.Normal, () =>
                {
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Script)));
                });
            }
        }

        private string _recentError;
        public string RecentError
        {
            get
            {
                return _recentError;
            }
            set
            {
                _recentError = value;
                DispatcherQueue.TryEnqueue(Microsoft.UI.Dispatching.DispatcherQueuePriority.Normal, () =>
                {
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(RecentError)));
                });
            }
        }

        public double _recentValue;
        public double RecentValue
        {
            get
            {
                return _recentValue;
            }
            set
            {
                _recentValue = value;
                DispatcherQueue.TryEnqueue(Microsoft.UI.Dispatching.DispatcherQueuePriority.Normal, () =>
                {
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(RecentValue)));
                });
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
