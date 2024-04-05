using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Navigation;

namespace WpfApp.Models
{
    internal class Supply : INotifyPropertyChanged
    {
        public int Id { get; private set; }
        public string Name { get; set; }

        private int _count;

        private decimal _cost;
        private decimal _sum;

        private static int _Id = 1;

        private static List<int> _list = new List<int>();

        public event PropertyChangedEventHandler PropertyChanged;

        public Supply()
        {
            if (_list.Count != 0) Id = getIdFromList();
            else Id = _Id++;

            Name = string.Empty;
            _count = 0;
            _cost = 0;
            _sum = 0;
        }

        public Supply(string Name, int Count, decimal Cost)
        {
            if (_list.Count != 0) Id = getIdFromList();
            else Id = _Id++;

            this.Name = Name;
            this.Count = Count;
            this.Cost = Cost;
            _sum = _cost * _count;
        }

        public decimal Sum 
        {
            get =>_sum;
            set 
            {
                _sum = value;
                OnPropertyChanged(nameof(Sum));
            }
        }

        public int Count 
        {
            get => _count;
            set 
            {
                _count = value;
                Sum = _count * _cost;
                OnPropertyChanged(nameof(Count));
            }
        }

        public decimal Cost 
        {
            get => _cost;
            set 
            {
                _cost = value;
                Sum = _cost * _count;
                OnPropertyChanged(nameof(Cost));
            }
        }

        public static void peekId(int id) 
        {
            if(!_list.Contains(id)) _list.Add(id);
            _list.Sort();
        }

        private int getIdFromList() 
        {
            int id = _list.FirstOrDefault();
            _list.Remove(id);
            return id;
        }

        protected virtual void OnPropertyChanged(string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
