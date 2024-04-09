using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using WpfApp.Models;
using WpfApp.Properties;

namespace WpfApp.ViewModels
{
    internal class ViewModel : BaseViewModel
    {
        public ObservableCollection<Supply> _Items { get; set; } = new ObservableCollection<Supply>();
        public DelegateCommand addItemCommand { get; private set; } 
        public DelegateCommand deleteItemCommand { get; private set; } 
        public DelegateCommand showFormForCreatinReport { get; private set; }
        public string dateTime { get; private set; }

        private int _orderId;
        public int OrderId 
        {
            get => _orderId;
            set 
            {
                _orderId = value;   
                OnPropertyChanged(nameof(OrderId));
            }
        }


        private decimal _totalSum = 0;
        public decimal TotalSum
        {
            get => _totalSum;
            set
            {
                if (_totalSum != value)
                {
                    _totalSum = value;
                    OnPropertyChanged(nameof(TotalSum));
                }
            }
        }

        private string _supplierName = string.Empty;
        public string SupplierName 
        {
            get => _supplierName;
            set 
            {
                _supplierName = value; 
                OnPropertyChanged(nameof(SupplierName));
            }
        }

        private string _buyerName = string.Empty;
        public string BuyerName 
        {
            get => _buyerName;
            set 
            {
                _buyerName = value;
                OnPropertyChanged(nameof(BuyerName));
            }
        }

        Supply _selectedSupply;
        public Supply SelectedSupply 
        {
            get => _selectedSupply;
            set 
            {
                _selectedSupply = value;
                OnPropertyChanged(nameof(SelectedSupply));
            }
        }

        public ViewModel() 
        {
            updateOrderId();
            addItemCommand = new DelegateCommand((obj) => addItem());
            deleteItemCommand = new DelegateCommand((obj) => deleteItem());
            showFormForCreatinReport = new DelegateCommand(obj => showNewWindow());
            dateTime = DateTime.Now.ToString("dd-MM-yyyy");   
        }

        private void addItem() 
        {
            Supply supply = new Supply();
            supply.PropertyChanged += recalculateTotalSum;
            _Items.Add(supply);
        }

        private void deleteItem() 
        {
            Supply.peekId(_selectedSupply.Id);
            _Items.Remove(_selectedSupply);
            recalculateTotalSum(null, null);
        }

        private async void showNewWindow()
        {
            var displayRootRegistry = (Application.Current as App).displayRootRegistry;
            ViewModelReport viewModelReport = new ViewModelReport(_orderId, _supplierName, _buyerName, _Items);
            await displayRootRegistry.ShowModalPresentation(viewModelReport);
            updateOrderId();
        }

        private void updateOrderId() 
        {
            OrderId = Convert.ToInt32(Settings.Default.Id);
        }

        private void recalculateTotalSum(object sender, PropertyChangedEventArgs e)
        {
            decimal total = 0;
            foreach (Supply item in _Items)
            {
                total += item.Sum;
            }
            TotalSum = total;
        }
    }

    public class IntToOrderIdConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int fontSize)
            {
                return fontSize.ToString();
            }
            return null;

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            TextBox textBox = (TextBox)value;
            return System.Convert.ToInt32(textBox.Text);
        }
    }
}
