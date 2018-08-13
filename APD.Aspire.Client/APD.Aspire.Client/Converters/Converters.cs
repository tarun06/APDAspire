using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace APD.Aspire.Client
{
    public class ListToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
           
            if(value is ObservableCollection<string> collection)
                return string.Join(" , ", collection);
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var coll = value.ToString().Replace(" ", string.Empty).Split(',');
            var collection =new  ObservableCollection<string>(coll) ;
            return collection;
        }
    }
}
