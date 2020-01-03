using System;
using System.Globalization;
using System.Windows.Data;

namespace GTI.WFMS.Models.Common
{

    /// <summary>
    /// 토글스위치 매핑컨버터 - alsokyun
    /// </summary>
    public class ToggleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ("Y".Equals(value))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool val = (bool)value;
            if (val)
                return "Y";
            else
                return "N";

        }
    }

    /// <summary>
    /// 토글스위치 매핑컨버터 (1/0)- alsokyun
    /// </summary>
    public class Toggle2Converter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ("1".Equals(value))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool val = (bool)value;
            if (val)
                return "1";
            else
                return "0";

        }
    }

    /// <summary>
    /// Rownumber 컨버터 - alsokyun
    /// </summary>
    public class RownumConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (int)value + 1;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }


    /// <summary>
    /// String2ImgConverter 컨버터 - alsokyun
    /// </summary>
    public class Str2ImgConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string filepath = value as string;
            return new Uri(BizUtil.GetDataFolder("style_img", filepath), UriKind.Absolute);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }


    /// <summary>
    /// Date2StrConverter 컨버터 - alsokyun
    /// </summary>
    public class Date2StrConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return System.Convert.ToDateTime(value).ToString("yyyy-MM-dd");
        }
    }

    public class S2StrConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }


}
