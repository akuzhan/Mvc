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
    public class CreatedAtRouteResult : ObjectResult
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CreatedAtRouteResult"/> class with the values
        /// provided.
        /// </summary>
        /// <param name="routeValues">The route data to use for generating the URL.</param>
        /// <param name="content">The content value to negotiate and format in the entity body.</param>
        public CreatedAtRouteResult([NotNull] IUrlHelper urlHelper,
                                    object routeValues,
                                    object content)
            : this(urlHelper, routeName: null, routeValues: routeValues, content: content)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CreatedAtRouteResult"/> class with the values
        /// provided.
        /// </summary>
        /// <param name="routeName">The name of the route to use for generating the URL.</param>
        /// <param name="routeValues">The route data to use for generating the URL.</param>
        /// <param name="content">The content value to negotiate and format in the entity body.</param>
        public CreatedAtRouteResult([NotNull] IUrlHelper urlHelper, 
                                    string routeName, 
                                    object routeValues, 
                                    object content) 
            : base(content)
        {
            UrlHelper = urlHelper;
            RouteName = routeName;
            RouteValues = TypeHelper.ObjectToDictionary(routeValues);
        }

        public IUrlHelper UrlHelper { get; private set; }

        /// <summary>
        /// Gets the name of the route to use for generating the URL.
        /// </summary>
        public string RouteName { get; private set; }

        /// <summary>
        /// Gets the route data to use for generating the URL.
        /// </summary>
        public IDictionary<string, object> RouteValues { get; private set; }

        /// <inheritdoc />
        protected override void OnFormatting(ActionContext context)
        {
            context.HttpContext.Response.StatusCode = 201;

            var url = UrlHelper.RouteUrl(RouteValues);

            if (string.IsNullOrEmpty(url))
            {
                throw new InvalidOperationException(Resources.NoRoutesMatched);
            }

            context.HttpContext.Response.Headers.Add("Location", new string[] { url });
        }
    }
}