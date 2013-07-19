using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Web;

namespace MvcExtras.Utils
{
    public class Compression
    {
        /// <summary>
        /// Forces compression on the passed HttpResponse, regardless of server configuration.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="response">The response.</param>
        public static void Force(HttpRequestBase request, HttpResponseBase response)
        {
            // force compression (azure doesn't compress json)
            string acceptEncoding = request.Headers["Accept-Encoding"];
            if (!string.IsNullOrEmpty(acceptEncoding))
            {
                acceptEncoding = acceptEncoding.ToLower(CultureInfo.InvariantCulture);
                if (acceptEncoding.Contains("gzip"))
                {
                    response.Filter = new GZipStream(response.Filter, CompressionMode.Compress);
                    response.AddHeader("Content-encoding", "gzip");
                }
                else if (acceptEncoding.Contains("deflate"))
                {
                    response.Filter = new DeflateStream(response.Filter, CompressionMode.Compress);
                    response.AddHeader("Content-encoding", "deflate");
                }
            }
        }

        /// <summary>
        /// Forces compression on the passed HttpResponse, regardless of server configuration.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="response">The response.</param>
        public static void Force(HttpRequest request, HttpResponse response)
        {
            // force compression (azure doesn't compress json)
            string acceptEncoding = request.Headers["Accept-Encoding"];
            if (!string.IsNullOrEmpty(acceptEncoding))
            {
                acceptEncoding = acceptEncoding.ToLower(CultureInfo.InvariantCulture);
                if (acceptEncoding.Contains("gzip"))
                {
                    response.Filter = new GZipStream(response.Filter, CompressionMode.Compress);
                    response.AddHeader("Content-encoding", "gzip");
                }
                else if (acceptEncoding.Contains("deflate"))
                {
                    response.Filter = new DeflateStream(response.Filter, CompressionMode.Compress);
                    response.AddHeader("Content-encoding", "deflate");
                }
            }
        }
    }
}
