using Microsoft.VisualStudio.Imaging.Interop;
using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace VSMacros.Helpers
{
    /// <summary>
    /// Helper Class for IVsImageService2
    /// </summary>
    internal static class IVsImageService2Helper
    {
        /// <summary>
        /// Get a BitmapSource from the ImageMoniker
        /// </summary>
        /// <param name="moniker">ImageMoniker to get the image from</param>
        /// <param name="height">size for the image height</param>
        /// <param name="width">size for the image width</param>
        /// <see cref="https://gist.github.com/madskristensen/8877c2f4b20cc3e6ddab"/>
        /// <returns>BitmapSource</returns>
        public static BitmapSource GetImage(ImageMoniker moniker, int height, int width)
        {
            Microsoft.VisualStudio.Shell.ThreadHelper.ThrowIfNotOnUIThread();

            ImageAttributes imageAttributes = new ImageAttributes();
            imageAttributes.Flags = (uint)_ImageAttributesFlags.IAF_RequiredFlags;
            imageAttributes.ImageType = (uint)_UIImageType.IT_Bitmap;
            imageAttributes.Format = (uint)_UIDataFormat.DF_WPF;
            imageAttributes.LogicalHeight = height;
            imageAttributes.LogicalWidth = width;
            imageAttributes.StructSize = Marshal.SizeOf(typeof(ImageAttributes));

            var service = (IVsImageService2)Microsoft.VisualStudio.Shell.Package.GetGlobalService(typeof(SVsImageService));
            IVsUIObject result = service.GetImage(moniker, imageAttributes);

            object data;
            result.get_Data(out data);

            if (data == null)
                return null;

            return data as BitmapSource;
        }

        /// <summary>
        /// Get a BitmapSource from the ImageMoniker
        /// </summary>
        /// <param name="moniker">ImageMoniker to get the image from</param>
        /// <param name="size">size for the image height/width</param>
        /// <see cref="https://gist.github.com/madskristensen/8877c2f4b20cc3e6ddab"/>
        /// <returns>BitmapSource</returns>
        public static BitmapSource GetImage(ImageMoniker moniker, int size)
        {
            Microsoft.VisualStudio.Shell.ThreadHelper.ThrowIfNotOnUIThread();

            ImageAttributes imageAttributes = new ImageAttributes();
            imageAttributes.Flags = (uint)_ImageAttributesFlags.IAF_RequiredFlags;
            imageAttributes.ImageType = (uint)_UIImageType.IT_Bitmap;
            imageAttributes.Format = (uint)_UIDataFormat.DF_WPF;
            imageAttributes.LogicalHeight = size;
            imageAttributes.LogicalWidth = size;
            imageAttributes.StructSize = Marshal.SizeOf(typeof(ImageAttributes));

            var service = (IVsImageService2)Microsoft.VisualStudio.Shell.Package.GetGlobalService(typeof(SVsImageService));
            IVsUIObject result = service.GetImage(moniker, imageAttributes);

            object data;
            result.get_Data(out data);

            if (data == null)
                return null;

            return data as BitmapSource;
        }

        /// <summary>
        /// Get a BitmapSource from the ImageMoniker
        /// </summary>
        /// <param name="moniker">ImageMoniker to get the image from</param>
        /// <see cref="https://gist.github.com/madskristensen/8877c2f4b20cc3e6ddab"/>
        /// <returns>BitmapSource</returns>
        public static BitmapSource GetImage(ImageMoniker moniker)
        {
            Microsoft.VisualStudio.Shell.ThreadHelper.ThrowIfNotOnUIThread();

            ImageAttributes imageAttributes = new ImageAttributes();
            imageAttributes.Flags = (uint)_ImageAttributesFlags.IAF_RequiredFlags;
            imageAttributes.ImageType = (uint)_UIImageType.IT_Bitmap;
            imageAttributes.Format = (uint)_UIDataFormat.DF_WPF;
            imageAttributes.StructSize = Marshal.SizeOf(typeof(ImageAttributes));

            var service = (IVsImageService2)Microsoft.VisualStudio.Shell.Package.GetGlobalService(typeof(SVsImageService));
            IVsUIObject result = service.GetImage(moniker, imageAttributes);

            object data;
            result.get_Data(out data);

            if (data == null)
                return null;

            return data as BitmapSource;
        }
    }
}
