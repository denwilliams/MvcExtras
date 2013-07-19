using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using MvcExtras.Utils;

namespace System.Web.Mvc
{
    public static class ResponseExtensions
    {
        /// <summary>
        /// Forces the response to be downloaded as a file download by the browser.
        /// </summary>
        /// <param name="response">The response.</param>
        /// <param name="fileName">Name of the file.</param>
        public static void ForceDownload(this HttpResponseBase response, string fileName)
        {
            response.AddHeader("content-disposition", "attachment; filename=" + fileName + ".html");
        }

        /// <summary>
        /// Forces gzip compression on the response.
        /// Required to enable compression on Windows Azure responses.
        /// </summary>
        /// <param name="response">The response.</param>
        /// <param name="request">The request.</param>
        public static void ForceCompression(this HttpResponseBase response, HttpRequestBase request)
        {
            Compression.Force(request, response);
        }
    }
}
