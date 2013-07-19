using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MvcExtras.Extensions
{
    /// <summary>
    /// GPS EXIF Metadata stored with geotagged images
    /// </summary>
    public class GpsMetaData
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public double Altitude { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
