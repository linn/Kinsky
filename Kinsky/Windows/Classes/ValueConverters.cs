using System.Drawing;
using System;
using System.Windows.Data;
using Upnp;
using System.Windows.Media.Imaging;
using System.Windows.Controls;
using System.Reflection;
using System.Windows;
using Linn.Kinsky;
using Linn.Topology;
using System.Text;
using Linn;
using System.Windows.Controls.Primitives;
namespace KinskyDesktopWpf
{

    #region BitmapToBitmapFrameConverter
    public class BitmapToBitmapFrameConverter : IValueConverter
    {

        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            System.Drawing.Image bmp = value as System.Drawing.Image;
            if (bmp != null)
            {
                return bmp.ToBitmapFrame();
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
    #endregion
    
    #region DidlLiteConverter
    public class DidlLiteConverter : IValueConverter
    {

        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            try
            {
                if (value != null && value is upnpObject)
                {
                    Type t = typeof(DidlLiteAdapter);
                    MemberInfo[] mi = t.FindMembers(MemberTypes.Method, BindingFlags.Public | BindingFlags.Static, (m, c) =>
                    {
                        return m.Name.Equals(c);
                    }, parameter);
                    if (mi.Length > 0)
                    {
                        object invokeResult = (mi[0] as MethodInfo).Invoke(null, new Object[] { value });
                        string result = string.Empty;
                        if (invokeResult != null)
                        {
                            result = invokeResult.ToString();
                        }
                        if (parameter.ToString() == "Artist" && !(value is Upnp.musicTrack || value is Upnp.musicAlbum))
                        {
                            result = string.Empty;
                        }
                        if (parameter.ToString() == "Count" && result == "0")
                        {
                            result = string.Empty;
                        }
                        if (parameter.ToString() == "OriginalTrackNumber" && result != string.Empty)
                        {
                            result = result + ". ";
                        }
                        if (parameter.ToString() == "AlbumArtist" && result == string.Empty)
                        {
                            result = DidlLiteAdapter.Artist(value as upnpObject);                            
                        }
                        return result;
                    }
                }
            }
            catch (Exception ex)
            {
                UserLog.WriteLine("Error caught in DidlLiteConverter: " + ex);
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
    #endregion

    #region MrItemConverter
    public class MrItemConverter : IValueConverter
    {
        private static DidlLiteConverter iDidlLiteConverter = new DidlLiteConverter();
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            MrItem item = value as MrItem;
            if (item != null && item.DidlLite.Count > 0)
            {
                return iDidlLiteConverter.Convert(item.DidlLite[0], targetType, parameter, culture);
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
    #endregion

    #region ArtistVisibilityConverter
    public class ArtistVisibilityConverter : IValueConverter
    {

        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            upnpObject obj = null;
            if (value is upnpObject)
            {
                obj = value as upnpObject;
            }
            else if (value is MrItem)
            {
                obj = (value as MrItem).DidlLite[0];
            }
            if (obj != null)
            {
                string artist = DidlLiteAdapter.Artist(obj);
                string albumArtist = DidlLiteAdapter.AlbumArtist(obj);
                if (albumArtist != string.Empty && artist != albumArtist)
                {
                    return Visibility.Visible;
                }
            }
            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
    #endregion

    #region TextBoxTextToBooleanConverter
    class TextBoxTextToBooleanConverter : IValueConverter
    {

        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string text = value as String;
            if (text != null)
            {
                return text.Length > 0;
            }
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
    #endregion

    #region BooleanVisibilityValueConverter
    public class BooleanVisibilityValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value != null)
            {
                if (((bool)value) == true)
                    return Visibility.Visible;
                else
                    return Visibility.Collapsed;
            }
            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new Exception("The method or operation is not implemented.");
        }


    }
    #endregion
    
    #region InverseBooleanVisibilityValueConverter
    public class InverseBooleanVisibilityValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value != null)
            {
                if (((bool)value) == true)
                    return Visibility.Collapsed;
                else
                    return Visibility.Visible;
            }
            return Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new Exception("The method or operation is not implemented.");
        }


    }
    #endregion

    #region BitrateValueConverter
    public class BitrateValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if ((uint)value != 0)
            {
                return value.ToString() + " kbps";
            }
            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new Exception("The method or operation is not implemented.");
        }


    }
    #endregion

    #region SampleRateAndBitDepthValueConverter
    public class SampleRateAndBitDepthValueConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if ((float)values[0] != 0f)
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendFormat("{0} kHz", values[0]);
                if ((bool)values[2] && (uint)values[1] != 0)
                {
                    sb.AppendFormat(" / {0} bits", values[1]);
                }
                return sb.ToString();
            }
            return string.Empty;
        }

        public object[] ConvertBack(object values, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new Exception("The method or operation is not implemented.");
        }


    }
    #endregion

    #region IntegerToBrushValueConverter
    public class IntegerToBrushValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            Color c = System.Drawing.Color.FromArgb(Int32.Parse(value.ToString()));
            return new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromArgb(c.A, c.R, c.G, c.B));
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new Exception("The method or operation is not implemented.");
        }


    }
    #endregion

    #region IntegerToColourValueConverter
    public class IntegerToColourValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            Color c = System.Drawing.Color.FromArgb(Int32.Parse(value.ToString()));
            return System.Windows.Media.Color.FromArgb(c.A, c.R, c.G, c.B);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new Exception("The method or operation is not implemented.");
        }
    }
    #endregion

    #region KompactTrackDisplayWidthConverter
    public class KompactTrackDisplayWidthConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            double width = (double)value;
            return width / 2;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new Exception("The method or operation is not implemented.");
        }
    }
    #endregion

    #region PlaylistPositionConverter
    public class PlaylistPositionConverter : IValueConverter
    {

        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return ((int)value + 1).ToString() + ".";
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
    #endregion

    #region ItemZoomScaleConverter
    public class ItemZoomScaleConverter : IMultiValueConverter
    {
        #region IMultiValueConverter Members
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            int position = (int)values[0];
            double scrollOffset = 0.0;
            if (values[1] != DependencyProperty.UnsetValue)
            {
                scrollOffset = (double)values[1];
            }
            double magnificationFactor = (double)values[2];
            int maxMagnifiedItems = (int)values[3];
            int exponentiationFactor = (int)values[4];

            double proximity = Math.Abs(position - scrollOffset);
            if (proximity < maxMagnifiedItems)
            {
                double multiplier = 1 - (proximity / maxMagnifiedItems);
                multiplier = Math.Pow(multiplier, exponentiationFactor);
                return 1.0 + (multiplier * magnificationFactor);
            }
            else
            {
                return 1.0;
            }

        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
    #endregion

    #region InverseItemZoomScaleConverter
    public class InverseItemZoomScaleConverter : IMultiValueConverter
    {

        #region IMultiValueConverter Members
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return (double)(new ItemZoomScaleConverter().Convert(values, targetType, parameter, culture)) * -1;

        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
    #endregion

    #region BooleanNegatingConverter
    public class BooleanNegatingConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return !(bool)value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return !(bool)value;
        }
    }
    #endregion

    #region ControlsWidthValueConverter
    public class ControlsWidthValueConverter : IValueConverter
    {
        public static double kMinWidth = 405d;
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            double screenWidth = (double)value;
            double result = screenWidth / 3;
            if (result < kMinWidth)
            {
                result = kMinWidth;
            }
            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion

    }
    #endregion

    #region WheelControlAngleValueConverter
    public class WheelControlAngleValueConverter : IValueConverter
    {
        public static double kMinWidth = 350d;
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return (double)value + 45;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion

    }
    #endregion

    #region StringConcatenationValueConverter
    public class StringConcatenationValueConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            StringBuilder sb = new StringBuilder();
            string separator = " - ";
            if (parameter != null)
            {
                separator = parameter.ToString();
            }
            for (int i = 0; i < values.Length; i++)
            {
                if (values[i] != null)
                {
                    sb.AppendFormat("{0}{1}", sb.ToString().Trim().Length > 0 && values[i].ToString().Trim().Length > 0 ? separator : "", values[i].ToString());
                }
            }
            return sb.ToString();
        }

        public object[] ConvertBack(object values, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new Exception("The method or operation is not implemented.");
        }


    }
    #endregion

    #region SourcePopupOffsetValueConverter
    public class SourcePopupOffsetValueConverter : IMultiValueConverter
    {
        #region IValueConverter Members

        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            double controlWidth = (double)values[0];
            double parentWidth = (double)values[1];
            return -(controlWidth - parentWidth);
        }

        public object[] ConvertBack(object values, Type[] targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion

    }
    #endregion
    
    #region BookmarkBreadcrumbTextValueConverter
    public class BookmarkBreadcrumbTextValueConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return BreadcrumbTrail(value as Bookmark);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public static string BreadcrumbTrail(Bookmark aBookmark)
        {
            StringBuilder builder = new StringBuilder();
            if (aBookmark != null)
            {
                foreach (Breadcrumb b in aBookmark.BreadcrumbTrail)
                {
                    builder.AppendFormat("{0}{1}", b.Title, b == aBookmark.BreadcrumbTrail[aBookmark.BreadcrumbTrail.Count - 1] ? "" : "/");
                }
            }
            return builder.ToString();
        }
        #endregion

    }
    #endregion
}