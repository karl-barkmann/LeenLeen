using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace Demo.Windows.View
{
    internal class StateToBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int state = (int) value;

            switch (state)
            {
                case  1:
                    return CreateBrush(Colors.Green);
                case 2:
                    return CreateBrush(Colors.Yellow);
                case 3:
                    return CreateBrush(Colors.OrangeRed);
            }

            return new SolidColorBrush(Colors.White);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        private LinearGradientBrush CreateBrush(Color color)
        {
            var brush = new LinearGradientBrush();
            brush.MappingMode= BrushMappingMode.RelativeToBoundingBox;;
            brush.GradientStops.Add(new GradientStop(color,1));
            brush.GradientStops.Add(new GradientStop(Colors.White, 0));
            brush.StartPoint = new Point(0.5, 0);
            brush.EndPoint = new Point(0.5, 1);
            return brush;
        }
    }
}
