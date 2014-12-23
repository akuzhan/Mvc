// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using Microsoft.AspNet.Mvc.Core;

namespace Microsoft.AspNet.Mvc
{
    /// <summary>
    /// Represents an <see cref="ActionResult"/> that performs route generation and content negotiation
    /// and returns a Created (201) response when content negotiation succeeds.
    /// </summary>
    public class CreatedAtActionResult : ObjectResult
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CreatedAtActionResult"/> class with the values
        /// provided.
        /// </summary>
        /// <param name="actionName">The name of the action to use for generating the URL.</param>
        /// /// <param name="controllerName">The name of the controller to use for generating the URL.</param>
        /// <param name="routeValues">The route data to use for generating the URL.</param>
        /// <param name="content">The content value to negotiate and format in the entity body.</param>
        public CreatedAtActionResult([NotNull] IUrlHelper urlHelper,
                                    string actionName,
                                    string controllerName,
                                    object routeValues,
                                    object content)
            : base(content)
        {
            UrlHelper = urlHelper;
            RouteValues = TypeHelper.ObjectToDictionary(routeValues);
        }

        public IUrlHelper UrlHelper { get; private set; }

        /// <summary>
        /// Gets the name of the action to use for generating the URL.
        /// </summary>
        public string ActionName { get; private set; }

        /// <summary>
        /// Gets the name of the controller to use for generating the URL.
        /// </summary>
        public string ControllerName { get; private set; }

        /// <summary>
        /// Gets the route data to use for generating the URL.
        /// </summary>
        public IDictionary<string, object> RouteValues { get; private set; }

        /// <inheritdoc />
        protected override void OnFormatting(ActionContext context)
        {
            context.HttpContext.Response.StatusCode = 201;

            var url = UrlHelper.Action(ActionName, ControllerName, RouteValues);

            if (string.IsNullOrEmpty(url))
            {
                throw new InvalidOperationException(Resources.NoRoutesMatched);
            }

            context.HttpContext.Response.Headers.Add("Location", new string[] { url });
        }
    }
}