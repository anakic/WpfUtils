using System;
using System.Linq;
using System.Windows.Media;
using System.IO;
using System.Windows.Media.Imaging;
using System.Reflection;

namespace Thingie.WPF.Attributes
{
	public class IconAttribute : Attribute
    {
        readonly string _iconUri;
        readonly Assembly _assembly;

        public IconAttribute(string iconUri, Type typeInAssembly)
        {
            _iconUri = iconUri;
            _assembly = typeInAssembly.Assembly;
        }

        public ImageSource Image
        {
            get
            {
                ImageSource _image = null;
                try
                {
                    if (!string.IsNullOrEmpty(_iconUri))
                    {
                        try
                        {
                            string fullUri = _assembly.GetManifestResourceNames().First(res => res.ToUpper().Contains(_iconUri.ToUpper()));
                            Stream imgStream = _assembly.GetManifestResourceStream(fullUri);
                            PngBitmapDecoder dec = new PngBitmapDecoder(imgStream, BitmapCreateOptions.None, BitmapCacheOption.OnLoad);
                            _image = dec.Frames[0];
                            imgStream.Close();
                        }
                        catch
                        { }
                    }
                    return _image;
                }
                catch
                {
                    return null;
                }
            }
        }
    }
}
