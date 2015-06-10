using Math.Lib;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MathTool
{
    public class MathViewModel : INotifyPropertyChanged
    {
        public int Number { get; set; }

        int _Result = 0;
        public int Result 
        {
            get
            {
                return _Result;
            }
            set
            {
                _Result = value;
                RaisePropertyChanged("Result");
            } 
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void RaisePropertyChanged(string propertyName)
        {
            // take a copy to prevent thread issues
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private ICommand _fibButtonCommand;
        public ICommand FibCommand
        {
            get
            {
                return _fibButtonCommand;
            }
            set
            {
                _fibButtonCommand = value;
            }
        }

        public void Fib(object obj)
        {
            if (this.Number > 40)
            {
                this.Result = 999999999;
                return;
            }
            //++this.Result;
            this.Result = _imath.Fib(this.Number);
        }

        private IMath _imath = null;

        public MathViewModel()
        {
            _fibButtonCommand = new RelayCommand(Fib);
            _imath = new MathChannelAdapter();
        }
    }
}
