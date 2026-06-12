using System;
using System.ComponentModel;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace Practice
{
    public class Product : INotifyPropertyChanged
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public string Icon { get; set; } = "";
        public decimal Price { get; set; }
        public string Category { get; set; } = "";

        private int _stock;
        public int Stock
        {
            get => _stock;
            set { _stock = value; OnPropertyChanged(nameof(Stock)); }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string name) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }

    public class CartItem : INotifyPropertyChanged
    {
        public Product Product { get; set; } = null!;

        private int _quantity;
        public int Quantity
        {
            get => _quantity;
            set
            {
                _quantity = value;
                OnPropertyChanged(nameof(Quantity));
                OnPropertyChanged(nameof(LineTotal));
            }
        }

        public string Name => Product.Name;
        public decimal Price => Product.Price;
        public decimal LineTotal => Product.Price * Quantity;

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string name) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }

    public class SaleRecord
    {
        public string Number { get; set; } = "";
        public string DateTime { get; set; } = "";
        public int ItemsCount { get; set; }
        public string Total { get; set; } = "";
        public string Paid { get; set; } = "";
        public string Change { get; set; } = "";
        public string Cashier { get; set; } = "";
    }

    public class StockBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int stock = value is int i ? i : 0;
            bool low = stock <= 5;
            string? mode = parameter as string;

            if (mode == "Background")
                return low
                    ? new SolidColorBrush(Color.FromRgb(0x42, 0x20, 0x06))
                    : new SolidColorBrush(Color.FromRgb(0x0F, 0x3D, 0x30));

            return low
                ? new SolidColorBrush(Color.FromRgb(0xFB, 0xBF, 0x24))
                : new SolidColorBrush(Color.FromRgb(0x34, 0xD3, 0x99));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
                    => throw new NotImplementedException();
    }
}