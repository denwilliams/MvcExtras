﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace MvcExtras.ActionResults
{
    /// <summary>An implementation of <see cref="ActionResult" /> that throws an <see cref="HttpException" />.</summary>
    public class HttpForbiddenResult : ActionResult
    {
        /// <summary>Initializes a new instance of <see cref="HttpForbiddenResult" /> with the specified <paramref name="message"/>.</summary>
        /// <param name="message"></param>
        public HttpForbiddenResult(string message)
        {
            this.Message = message;
        }

        /// <summary>Initializes a new instance of <see cref="HttpForbiddenResult" /> with an empty message.</summary>
        public HttpForbiddenResult()
            : this(string.Empty) { }

        /// <summary>Gets or sets the message that will be passed to the thrown <see cref="HttpException" />.</summary>
        public string Message { get; set; }

        /// <summary>Overrides the base <see cref="ActionResult.ExecuteResult" /> functionality to throw an <see cref="HttpException" />.</summary>
        public override void ExecuteResult(ControllerContext context)
        {
            throw new HttpException((Int32)HttpStatusCode.Forbidden, this.Message);
        }
    }
}