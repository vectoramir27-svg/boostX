using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace BoostX.Models
{
    public class SystemStatItem : INotifyPropertyChanged
    {
        private string _value = string.Empty;

        public string Title { get; set; } = string.Empty;

        public string Value
        {
            get => _value;
            set
            {
                if (_value != value)
                {
                    _value = value;
                    OnPropertyChanged();
                }
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? name = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}