using Avalonia.Data.Converters;
using MigraDoc.DocumentObjectModel;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shiai_Helper.UI
{
    public class OrientationTranslator : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var or = (Orientation)value;
            switch(or)
            {
                case Orientation.Portrait:
                    return "Hochformat";
                case Orientation.Landscape:
                    return "Querformat";
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {            
            return null;
        }
    }
}
