using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using MvcExtras.Extensions;

namespace System.Web.Mvc
{
    public static class ImageExtensions
    {
        /// <summary>
        /// Converts the byte array to a System.Drawing.Image
        /// </summary>
        /// <param name="imageIn">The image to convert.</param>
        /// <param name="format">The image format to use. Default is JPEG.</param>
        /// <returns></returns>
        public static byte[] ToByteArray(this Image imageIn, ImageFormat format = null)
        {
            var ms = imageIn.ToStream(format);
            return ms.ToArray();
        }

        /// <summary>
        /// Converts the image to a memory stream.
        /// </summary>
        /// <param name="imageIn">The image to convert.</param>
        /// <param name="format">The image format to use. Default is JPEG.</param>
        /// <returns></returns>
        public static MemoryStream ToStream(this Image imageIn, ImageFormat format = null)
        {
            if (format == null) format = ImageFormat.Jpeg;
            if (imageIn == null) return null;
            var ms = new MemoryStream();
            imageIn.Save(ms, format);
            return ms;
        }

        /// <summary>
        /// Converts the byte array to an image.
        /// </summary>
        /// <param name="byteArrayIn">The byte array to convert.</param>
        /// <returns></returns>
        public static Image ToImage(this byte[] byteArrayIn)
        {
            if (byteArrayIn == null) return null;
            try
            {
                var ms = new MemoryStream(byteArrayIn);
                Image returnImage = Image.FromStream(ms);
                return returnImage;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Resizes the specified image, optionally disposes the original and returns a new one that fits the specified size.
        /// Aspect ratio is always maintained. The resulting image will always fit inside the specified size.
        /// </summary>
        /// <param name="image">The image.</param>
        /// <param name="newSize">The new size.</param>
        /// <param name="onlyShrink">if set to <c>true</c> and the specified size is larger than the current image, the image will not be shrunk.</param>
        /// <param name="copyMetadata">if set to <c>true</c> meta data (EXIF info) will be copied to the new image.</param>
        /// <param name="disposeAfterResize">if set to <c>true</c> the original image will be disposed after the resize.</param>
        /// <returns></returns>
        public static Image Resize(this Image image, Size newSize, bool onlyShrink = true, bool copyMetadata = false, bool disposeAfterResize = false, InterpolationMode quality = InterpolationMode.HighQualityBicubic)
        {
            if (onlyShrink && image.Width <= newSize.Width && image.Height <= newSize.Height) return image;

            PropertyItem[] propertyItems = copyMetadata ? image.PropertyItems : null;

            int sourceX = 0, sourceY = 0, destX = 0, destY = 0;
            float nPercent = 0, nPercentW = 0, nPercentH = 0;

            nPercentW = ((float)newSize.Width / (float)image.Width);
            nPercentH = ((float)newSize.Height / (float)image.Height);
            nPercent = nPercentH < nPercentW ? nPercentH : nPercentW;

            int destWidth = (int)(image.Width * nPercent);
            int destHeight = (int)(image.Height * nPercent);

            Bitmap bmPhoto = new Bitmap(destWidth, destHeight, PixelFormat.Format24bppRgb);

            bmPhoto.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            Graphics grPhoto = Graphics.FromImage(bmPhoto);
            grPhoto.Clear(Color.Black);
            grPhoto.InterpolationMode = InterpolationMode.HighQualityBicubic;

            grPhoto.DrawImage(image, new Rectangle(destX, destY, destWidth, destHeight),
                new Rectangle(sourceX, sourceY, image.Width, image.Height),
                GraphicsUnit.Pixel);

            if (propertyItems != null)
            {
                foreach (var propertyItem in propertyItems)
                {
                    bmPhoto.SetPropertyItem(propertyItem);
                }
            }

            grPhoto.Dispose();
            if (disposeAfterResize) image.Dispose();

            return bmPhoto;
        }

        /// <summary>
        /// Creates a thumbnail for the specified image.
        /// </summary>
        /// <param name="image">The image.</param>
        /// <param name="size">The size of the bounding box to fit the thumbnail into.</param>
        /// <returns></returns>
        public static Image Thumbnail(this Image image, Size size)
        {
            return image.Resize(size, true, false, false, InterpolationMode.Low);
        }

        /// <summary>
        /// Creates a thumbnail for the specified image.
        /// </summary>
        /// <param name="image">The image.</param>
        /// <param name="size">The size of the long edge in pixels.</param>
        /// <returns></returns>
        public static Image Thumbnail(this Image image, int size)
        {
            return image.Resize(new Size(size, size), true, false, false, InterpolationMode.Low);
        }

        // see:
        // http://www.codeproject.com/Articles/43665/ExifLibrary-for-NET
        // http://stackoverflow.com/questions/2169444/how-can-i-read-the-exif-data-from-an-image-taken-with-an-apple-iphone
        // http://weblogs.asp.net/zroiy/archive/2008/07/13/embedding-gps-coordinates-and-other-info-in-jpeg-images-with-c.aspx
        // http://blog.tutorem.com/post/2011/08/28/C-Utility-to-Read-Exif-Image-Data.aspx
        // http://msdn.microsoft.com/en-us/library/xddt0dz7.aspx
        // http://archive.msdn.microsoft.com/changexifwithcsharp
        // http://stackoverflow.com/questions/4983766/getting-gps-data-from-an-images-exif-in-c-sharp

        /// <summary>
        /// Gets the description of the image from the ImageDescription EXIF data.
        /// </summary>
        /// <param name="image">The image.</param>
        /// <returns>A string value, or null if not found</returns>
        public static string GetDescription(this Image image)
        {
            try
            {
                //ImageDescription
                const int exifId = 0x010e;
                if (!image.PropertyIdList.Contains(exifId)) return null;
                PropertyItem propItem = image.GetPropertyItem(exifId);
                return Encoding.UTF8.GetString(propItem.Value).Replace("\0", string.Empty).Trim();
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Gets a new description for the image to the ImageDescription EXIF value.
        /// </summary>
        /// <param name="image">The image.</param>
        /// <param name="description">The description to set.</param>
        /// <returns>
        /// true if succeeded, false otherwise
        /// </returns>
        public static bool SetDescription(this Image image, string description)
        {
            try
            {
                //ImageDescription
                const int exifId = 0x010e;
                // note: ideally we would do image.GetPropertyItem(0x010e),
                // but if there isn't a description already that would fail.
                // This is actually the recommendation from Microsoft - 
                // to get any other property, modify it, then set it back!
                PropertyItem propItem = image.PropertyItems[0]; 
                SetExifProperty(ref propItem, exifId, description);
                image.SetPropertyItem(propItem);
                return true;
            }
            catch
            {
                return false;
            }
        }

        //My SetProperty code... (for ASCII property items only!)
        //Exif 2.2 requires that ASCII property items terminate with a null (0x00).
        private static void SetExifProperty(ref PropertyItem prop, int id, string text)
        {
            var encodedString = Encoding.UTF8.GetBytes(text);

            int iLen = encodedString.Length + 1;
            byte[] bTxt = new byte[iLen];
            for (int i = 0; i < iLen - 1; i++)
                bTxt[i] = encodedString[i];
            bTxt[iLen - 1] = 0x00; // null terminated string
            prop.Id = id;
            prop.Type = 2;
            prop.Value = bTxt;
            prop.Len = iLen;
        }

        /// <summary>
        /// Gets the date the image was taken.
        /// </summary>
        /// <param name="image">The image.</param>
        /// <returns></returns>
        public static DateTime? GetDateTaken(this Image image)
        {
            try
            {
                //DateTimeOriginal
                PropertyItem propItem = image.GetPropertyItem(0x9003);
                // See also - DateTimeDigitized / CreateDate 0x9004
                // .. TimeZoneOffset 0x882a & ModifyDate (DateTime) 0x0132

                //Convert date taken metadata to a DateTime object
                string sdate = Encoding.UTF8.GetString(propItem.Value).Replace("\0", String.Empty).Trim();
                string secondhalf = sdate.Substring(sdate.IndexOf(" "), (sdate.Length - sdate.IndexOf(" ")));
                string firsthalf = sdate.Substring(0, 10);
                firsthalf = firsthalf.Replace(":", "-");
                sdate = firsthalf + secondhalf;
                return DateTime.Parse(sdate);
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Gets the datetime stamp from the GPS metadata (more likely to be accurate than camera timestamp).
        /// </summary>
        /// <param name="image">The image.</param>
        /// <returns></returns>
        public static DateTime? GetGpsDateTimeStamp(this Image image)
        {
            try
            {
                //GPSTimeStamp
                PropertyItem propTime = image.GetPropertyItem(0x0007);
                uint hours = GetExifSubValue(propTime, 0);
                uint mins = GetExifSubValue(propTime, 1);
                uint secs = GetExifSubValue(propTime, 2);
                string stime = string.Format("{0:00}:{1:00}:{2:00}", hours, mins, secs);

                //GPSDateStamp
                PropertyItem propDate = image.GetPropertyItem(0x001d);

                //Convert date taken metadata to a DateTime object
                string sdate = Encoding.UTF8.GetString(propDate.Value).Replace("\0", String.Empty).Trim();
                sdate = sdate.Replace(":", "-");
                return DateTime.Parse(sdate + " " + stime);
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Gets the GPS info for the image.
        /// </summary>
        /// <param name="image">The image.</param>
        /// <returns></returns>
        public static GpsMetaData GetGpsInfo(this Image image)
        {
            float? lat = GetLatitude(image);
            float? lon = GetLongitude(image);
            var dTs = GetGpsDateTimeStamp(image);
            var alt = GetAltitude(image);
            var dTaken = GetDateTaken(image);
            var result = new GpsMetaData();
            if (lat.HasValue) result.Latitude = lat.Value;
            if (lon.HasValue) result.Longitude = lon.Value;
            if (alt.HasValue) result.Altitude = alt.Value;
            if (dTs.HasValue) result.Timestamp = dTs.Value;
            else if (dTaken.HasValue) result.Timestamp = dTaken.Value;

            return result;
        }

        /// <summary>
        /// Gets the latitude component from the GPS metadata.
        /// </summary>
        /// <param name="image">The image.</param>
        /// <returns></returns>
        public static float? GetLatitude(this Image image)
        {
            try
            {
                //PropertyTagGpsLatitudeRef - 'N' or 'S'
                PropertyItem propItemRef = image.GetPropertyItem(1);
                //PropertyTagGpsLatitude
                PropertyItem propItemLat = image.GetPropertyItem(2);
                return ExifGpsToFloat(propItemRef, propItemLat);
            }
            catch (ArgumentException)
            {
                return null;
            }
        }

        /// <summary>
        /// Gets the longitude component from the GPS metadata.
        /// </summary>
        /// <param name="image">The image.</param>
        /// <returns></returns>
        public static float? GetLongitude(this Image image)
        {
            try
            {
                //PropertyTagGpsLongitudeRef - 'E' or 'W'
                PropertyItem propItemRef = image.GetPropertyItem(3);
                //PropertyTagGpsLongitude
                PropertyItem propItemLong = image.GetPropertyItem(4);
                return ExifGpsToFloat(propItemRef, propItemLong);
            }
            catch (ArgumentException)
            {
                return null;
            }
        }

        /// <summary>
        /// Gets the altitude component from the GPS metadata.
        /// </summary>
        /// <param name="image">The image.</param>
        /// <returns></returns>
        public static float? GetAltitude(this Image image)
        {
            try
            {
                //GPSAltitudeRef - 0 (above sea level) or 1 (below sea level)
                PropertyItem propItemRef = image.GetPropertyItem(0x0005);
                //GPSAltitude
                PropertyItem propItemLong = image.GetPropertyItem(0x0006);
                float value = GetExifSubValue(propItemLong, 0);
                if (propItemRef.Value[0] == 1)
                    value = 0 - value;
                return value;
            }
            catch (ArgumentException)
            {
                return null;
            }
        }

        private static float ExifGpsToFloat(PropertyItem propItemRef, PropertyItem propItem)
        {
            uint degrees = GetExifSubValue(propItem, 0);
            uint minutes = GetExifSubValue(propItem, 1);
            uint seconds = GetExifSubValue(propItem, 2);

            float coorditate = degrees + (minutes / 60f) + (seconds / 3600f);
            string gpsRef = System.Text.Encoding.ASCII.GetString(new byte[1] { propItemRef.Value[0] }); //N, S, E, or W
            if (gpsRef == "S" || gpsRef == "W")
                coorditate = 0 - coorditate;
            return coorditate;
        }

        private static uint GetExifSubValue(PropertyItem property, int index)
        {
            int baseIndex = index * 8;
            uint numerator = BitConverter.ToUInt32(property.Value, baseIndex);
            uint denominator = BitConverter.ToUInt32(property.Value, baseIndex + 4);
            return numerator / denominator;
        }
    }
}
