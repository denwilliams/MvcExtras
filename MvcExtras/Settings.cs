using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvcExtras
{
    public static class Settings
    {
        /// <summary>
        /// Gets or sets a value indicating whether the stack trace will be returns in JSON errors.
        /// </summary>
        public static bool ShowStackTrack { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the site is offline.
        /// </summary>
        public static bool IsOffline { get; set; }

        /// <summary>
        /// Gets or sets the URL to redirect to if IsOffline is true.
        /// </summary>
        public static string OfflineUrl { get; set; }
        
        #region Delegates

        /// <summary>
        /// If defined, this method will be called every time an error is handled by HandleErrorPlusAttribute.
        /// Use it to log errors or possibly send notifications to an admin when certain exceptions occur.
        /// </summary>
        public static Delegates.LogExceptionDelegate HandleErrorPlugLogDelegate { get; set; }

        #endregion
    }
}
