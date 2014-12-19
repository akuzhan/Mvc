// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;

namespace Microsoft.AspNet.Mvc
{
    /// <summary>
    /// Represents an <see cref="ActionResult"/> that performs route generation and content negotiation
    /// and returns a Created (201) response when content negotiation succeeds.
    /// </summary>
    public class CreatedResult : ObjectResult
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CreatedResult"/> class with the values
        /// provided.
        /// </summary>
        /// <param name="location">The location at which the content has been created.</param>
        /// <param name="content">The content value to negotiate and format in the entity body.</param>
        public CreatedResult([NotNull]Uri location, object content) : base(content)
        {
            Location = location;
        }

        /// <summary>
        /// Gets the location at which the content has been created.
        /// </summary>
        public Uri Location { get; private set; }

        /// <inheritdoc />
        protected override void OnFormatting(ActionContext context)
        {
            context.HttpContext.Response.StatusCode = 201;

            string location;
            if (Location.IsAbsoluteUri)
            {
                location = Location.AbsoluteUri;
            }
            else
            {
                location = Location.GetComponents(UriComponents.SerializationInfoString, UriFormat.UriEscaped);
            }

            context.HttpContext.Response.Headers.Add("Location", new string[] { location });
        }
    }
}