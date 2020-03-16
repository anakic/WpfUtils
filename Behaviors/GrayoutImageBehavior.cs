using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Thingie.WPF.Behaviors
{
    public class GrayoutImageBehavior
    {
        public static readonly DependencyProperty GrayOutOnDisabledProperty = DependencyProperty.RegisterAttached("GrayOutOnDisabled", typeof(bool), typeof(GrayoutImageBehavior), new PropertyMetadata(default(bool), OnGrayOutOnDisabledChanged));
        public static void SetGrayOutOnDisabled(Image element, bool value) { element.SetValue(GrayOutOnDisabledProperty, value); }
        public static bool GetGrayOutOnDisabled(Image element) { return (bool)element.GetValue(GrayOutOnDisabledProperty); }

        public static readonly DependencyProperty GrayOutBindingImageOnDisabledProperty = DependencyProperty.RegisterAttached("GrayOutBindingImageOnDisabled", typeof(bool), typeof(GrayoutImageBehavior), new PropertyMetadata(default(bool), OnGrayOutBindingImageOnDisabledChanged));
        public static void SetGrayOutBindingImageOnDisabled(Image element, bool value) { element.SetValue(GrayOutBindingImageOnDisabledProperty, value); }
        public static bool GetGrayOutBindingImageOnDisabled(Image element) { return (bool)element.GetValue(GrayOutBindingImageOnDisabledProperty); }

        private static Dictionary<int, Binding> imageHashCodeBindingPairs = new Dictionary<int, Binding>();

        private static void OnGrayOutOnDisabledChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            Image image = (Image)obj;

            image.IsEnabledChanged -= OnImageIsEnabledChanged;
            image.SourceUpdated -= Image_SourceUpdated;
            image.Loaded -= Image_Loaded;

            if ((bool)args.NewValue)
            {
                image.IsEnabledChanged += OnImageIsEnabledChanged;
                image.SourceUpdated += Image_SourceUpdated;
                image.Loaded += Image_Loaded;
            }
            ToggleGrayOut(image); // initial call
        }

        private static void OnGrayOutBindingImageOnDisabledChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            Image image = (Image)obj;

            

            image.IsEnabledChanged -= OnBindingImageIsEnabledChanged;
            image.SourceUpdated -= BindingImage_SourceUpdated;
            image.Loaded -= BindingImage_Loaded;

            if ((bool)args.NewValue)
            {
                image.IsEnabledChanged += OnBindingImageIsEnabledChanged;
                image.SourceUpdated += BindingImage_SourceUpdated;
                image.Loaded += BindingImage_Loaded;
            }
            ToggleBindingImageGrayOut(image); // initial call
        }

        private static void Image_Loaded(object sender, RoutedEventArgs e)
        {
            var image = (Image)sender;
            ToggleGrayOut(image);
        }

        private static void Image_SourceUpdated(object sender, DataTransferEventArgs e)
        {
            var image = (Image)sender;
            ToggleGrayOut(image);
        }

        private static void OnImageIsEnabledChanged(object sender, DependencyPropertyChangedEventArgs args)
        {
            var image = (Image)sender;
            ToggleGrayOut(image);
        }

        private static void BindingImage_Loaded(object sender, RoutedEventArgs e)
        {
            var image = (Image)sender;
            ToggleBindingImageGrayOut(image);
        }

        private static void BindingImage_SourceUpdated(object sender, DataTransferEventArgs e)
        {
            var image = (Image)sender;
            ToggleBindingImageGrayOut(image);
        }

        private static void OnBindingImageIsEnabledChanged(object sender, DependencyPropertyChangedEventArgs args)
        {
            var image = (Image)sender;
            ToggleBindingImageGrayOut(image);
        }

        private static void ToggleGrayOut(Image image)
        {
            try
            {
                if (image.IsEnabled)
                {
                    var grayImage = image.Source as FormatConvertedBitmap;
                    if (grayImage != null)
                    {
                        image.Source = grayImage.Source; // Set the Source property to the original value.
                        image.OpacityMask = null; // Reset the Opacity Mask
                        image.Opacity = 1.0;
                    }
                }
                else
                {
                    var bitmapImage = default(BitmapImage);

                    if (image.Source is BitmapImage)
                        bitmapImage = (BitmapImage)image.Source;
                    else if (image.Source is BitmapSource && image.Source is FormatConvertedBitmap == false) // assume uri source
                        bitmapImage = new BitmapImage(new Uri(image.Source.ToString()));

                    if (bitmapImage != null)
                    {
                        image.Source = new FormatConvertedBitmap(bitmapImage, PixelFormats.Gray32Float, null, 0); // Get the source bitmap
                        image.OpacityMask = new ImageBrush(bitmapImage); // Create Opacity Mask for grayscale image as FormatConvertedBitmap does not keep transparency info
                        image.Opacity = 0.3; // optional: lower opacity
                    }
                }
            }
            catch (Exception)
            {

            }
        }

        private static void ToggleBindingImageGrayOut(Image image)
        {
            try
            {
                if (image.IsEnabled)
                {
                    if (image.GetBindingExpression(Image.SourceProperty) == null && imageHashCodeBindingPairs.ContainsKey(image.GetHashCode()))
                    {
                        image.SetBinding(Image.SourceProperty, imageHashCodeBindingPairs[image.GetHashCode()]);
                    }
                    image.OpacityMask = null; // Reset the Opacity Mask
                    image.Opacity = 1.0;
                }
                else
                {
                    var bitmapImage = default(BitmapImage);
                    if (image.GetBindingExpression(Image.SourceProperty) != null)
                    {
                        imageHashCodeBindingPairs[image.GetHashCode()] = image.GetBindingExpression(Image.SourceProperty).ParentBinding;
                    }

                    if (image.Source is BitmapImage)
                        bitmapImage = (BitmapImage)image.Source;
                    else if (image.Source is BitmapSource && image.Source is FormatConvertedBitmap == false) // assume uri source
                        bitmapImage = new BitmapImage(new Uri(image.Source.ToString()));

                    if (bitmapImage != null)
                    {
                        image.Source = new FormatConvertedBitmap(bitmapImage, PixelFormats.Gray32Float, null, 0); // Get the source bitmap
                        image.OpacityMask = new ImageBrush(bitmapImage); // Create Opacity Mask for grayscale image as FormatConvertedBitmap does not keep transparency info
                        image.Opacity = 0.3; // optional: lower opacity
                    }
                }
            }
            catch (NullReferenceException)
            {
                throw new NullReferenceException("The specified element has no binding.");
            }
            catch (Exception)
            {

            }
        }
    }
}
