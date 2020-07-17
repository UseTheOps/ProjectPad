using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace ProjectPad.Business
{
    public abstract class ViewModelBase : INotifyPropertyChanged
    {
        protected bool DisablePropertyChangeEvent { get; set; } = false;

        protected void OnPropertyChanged(string prop)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
