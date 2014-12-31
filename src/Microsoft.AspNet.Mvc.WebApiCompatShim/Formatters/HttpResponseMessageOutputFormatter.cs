// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNet.HttpFeature;
using Microsoft.AspNet.Mvc.HeaderValueAbstractions;

namespace Microsoft.AspNet.Mvc.WebApiCompatShim
{
    public class HttpResponseMessageOutputFormatter : IOutputFormatter
    {
        public bool CanWriteResult(OutputFormatterContext context, MediaTypeHeaderValue contentType)
        {
            return context.Object is HttpResponseMessage;
        }

        public IReadOnlyList<MediaTypeHeaderValue> GetSupportedContentTypes(
            Type declaredType,
            Type runtimeType,
            MediaTypeHeaderValue contentType)
        {
            return null;
        }

        public async Task WriteAsync(OutputFormatterContext context)
        {
            var response = context.ActionContext.HttpContext.Response;

            var responseMessage = context.Object as HttpResponseMessage;
            if (responseMessage == null)
            {
                var message = Resources.FormatHttpResponseMessageFormatter_UnsupportedType(
                    nameof(HttpResponseMessageOutputFormatter),
                    nameof(HttpResponseMessage));

                throw new InvalidOperationException(message);
            }

            using (responseMessage)
            {
                bool isTransferEncodingChunked = responseMessage.Headers.TransferEncodingChunked == true;

                if (isTransferEncodingChunked)
                {
                    // According to section 4.4 of the HTTP 1.1 spec, HTTP responses that use chunked transfer
                    // encoding must not have a content length set. Chunked should take precedence over content
                    // length in this case because chunked is always set explicitly by users while the Content-Length
                    // header can be added implicitly by System.Net.Http.
                    responseMessage.Content.Headers.ContentLength = null;
                }
                else
                {
                    Exception exception = null;

                    // Copy the response content headers only after ensuring they are complete.
                    // We ask for Content-Length first because HttpContent lazily computes this
                    // and only afterwards writes the value into the content headers.
                    try
                    {
                        var unused = responseMessage.Content.Headers.ContentLength;
                    }
                    catch (Exception ex)
                    {
                        exception = ex;
                    }
                }

                // Ignore the Transfer-Encoding header if it is just "chunked"; the host will provide it when no
                // Content-Length is present and BufferOutput is disabled (and this method guarantees those conditions).
                // HttpClient sets this header when it receives chunked content, but HttpContent does not include the
                // frames. The ASP.NET contract is to set this header only when writing chunked frames to the stream.
                // A Web API caller who desires custom framing would need to do a different Transfer-Encoding (such as
                // "identity, chunked").
                var transferEncoding = responseMessage.Headers.TransferEncoding;
                if (isTransferEncodingChunked && transferEncoding.Count == 1)
                {
                    transferEncoding.Clear();
                }
                
                //-----------------------------------

                response.StatusCode = (int)responseMessage.StatusCode;

                var responseFeature = context.ActionContext.HttpContext.GetFeature<IHttpResponseFeature>();
                if (responseFeature != null)
                {
                    responseFeature.ReasonPhrase = responseMessage.ReasonPhrase;
                }

                var responseHeaders = responseMessage.Headers;
                foreach (var header in responseHeaders)
                {
                    response.Headers.AppendValues(header.Key, header.Value.ToArray());
                }

                if (responseMessage.Content != null)
                {
                    var contentHeaders = responseMessage.Content.Headers;
                    foreach (var header in contentHeaders)
                    {
                        response.Headers.AppendValues(header.Key, header.Value.ToArray());
                    }

                    await responseMessage.Content.CopyToAsync(response.Body);
                }
            }
        }
    }
}
