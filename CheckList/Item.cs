using System.ComponentModel;

namespace CheckList
{
    public class Item : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public ulong Id { get; set; }
        public string Description { get; set; }
        private Status _state;
        private bool _isCompleted;


        public Status State
        {
            get { return _state; }
            set
            {
                _state = value;
                if (_state == Status.Completed)
                {
                    _isCompleted = true;
                    if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("IsCompleted"));
                }
                if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("State"));
                
            }

        }

        public bool IsCompleted
        {
            get { return _isCompleted; }
            set
            {
                _isCompleted = value;
                if (_isCompleted)
                {
                    _state = Status.Completed;
                    if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("State"));
                }
                if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("IsCompleted"));
            }
        }


    }
}
