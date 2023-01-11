using IFacial;
using Microsoft.UI.Dispatching;
using System.ComponentModel;

namespace VTube_IFacial_Link.DataModels
{
    public class CapturedDataModel : INotifyPropertyChanged
    {
        private CapturedData _data;

        public CapturedData Data
        {
            set
            {
                _data = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Data)));
            }
            get
            {
                return _data;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public CapturedDataModel(CapturedData data)
        {
            Data = data;
        }

        public void NotifyDataChanged(DispatcherQueue dispatcherQueue = null)
        {
            if (dispatcherQueue != null && !dispatcherQueue.HasThreadAccess)
            {
                dispatcherQueue.TryEnqueue(() =>
                {
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Data)));
                });
            }
            else
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Data)));
            }
        }
    }
}
