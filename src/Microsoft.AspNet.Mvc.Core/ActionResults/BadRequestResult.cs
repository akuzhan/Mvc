// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.AspNet.Mvc.ModelBinding;

namespace Microsoft.AspNet.Mvc
{
    /// <summary>
    /// Represents an <see cref="ObjectResult"/> that when
    /// executed will produce a Bad Request (400) response.
    /// </summary>
    public class BadRequestResult : ObjectResult
    {
        /// <summary>
        /// Creates a new <see cref="BadRequestResult"/> instance.
        /// </summary>
        public BadRequestResult() : base("")
        {
            StatusCode = 400;
        }

        /// <summary>
        /// Creates a new <see cref="BadRequestResult"/> instance.
        /// </summary>
        /// <param name="errorDictionary">Dictionary containing the errors to be returned to the client.</param>
        public BadRequestResult(object errorDictionary) : base(errorDictionary)
        {
            StatusCode = 400;
        }

        /// <summary>
        /// Creates a new <see cref="BadRequestResult"/> instance.
        /// </summary>
        /// <param name="modelState"><see cref="ModelStateDictionary"/> containing the validation errors.</param>
        public BadRequestResult(ModelStateDictionary modelState) : base(new SerializableError(modelState))
        {
            StatusCode = 400;
        }
    }
}