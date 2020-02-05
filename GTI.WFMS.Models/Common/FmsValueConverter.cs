using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
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
    /// 토글스위치 매핑컨버터 - alsokyun
    /// </summary>
    public class Toggle3Converter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ("Y".Equals(value))
            {
                return "Visible";
            }
            else
            {
                return "Hidden";
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
    public class Date3StrConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                return DateTime.ParseExact((string)value, "yyyyMMdd", null).ToString("yyyy-MM-dd");
            }
            catch (Exception)
            {
                return null;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return System.Convert.ToDateTime(value).ToString("yyyyMMdd");
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



    /// <summary>
    /// FileLenConverter - 파일사이즈 kb & 컴마
    /// </summary>
    public class FileLenConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double len = 0;
            string size = "";

            //kbyte
            try
            {
                len = System.Convert.ToDouble(value);
                len = Math.Round(len / 1000, 0);
            }
            catch (Exception) { }

            //컴마추가
            size = string.Format("{0:n0}", len);

            return size;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }

    /// <summary>
    /// FileLenConverter - 원본파일명
    /// </summary>
    public class FileNameConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int idx = (int)value;
            string name = "";
            ObservableCollection<FileInfo> Items = new ObservableCollection<FileInfo>();

            Items = parameter as ObservableCollection<FileInfo>;
            FileInfo fi = Items[idx];
            name = fi.FullName;

            return name;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }


}
