using IFacial;
using System.ComponentModel;

namespace VTube_IFacial_Link.DataModel
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

        public void NotifyDataChanged()
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Data)));
        }
    }
}
