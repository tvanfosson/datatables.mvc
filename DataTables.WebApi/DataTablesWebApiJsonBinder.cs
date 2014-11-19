#region Copyright
/* The MIT License (MIT)

Copyright (c) 2014 Anderson Luiz Mendes Matos

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in
all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE.
*/
#endregion Copyright
using System.Net.Http;
using System.Web.Http.Controllers;
using System.Web.Http.ModelBinding;

using DataTables.Core;
using Newtonsoft.Json;

namespace DataTables.WebApi
{
    /// <summary>
    /// Defines an abstract DataTables binder to bind a model with the JSON request from DataTables.
    /// </summary>
    public abstract class DataTablesJsonBinder<T> : IModelBinder
        where T : IDataTablesRequest
    {
        /// <summary>
        /// Get's the JSON parameter name to retrieve data. 
        /// You may override this to change to your parameter.
        /// </summary>
        protected virtual string JSON_PARAMETER_NAME { get { return "json"; } }
       
        /// <summary>
        /// Binds a new model with the DataTables request parameters.
        /// </summary>
        /// <param name="actionContext"></param>
        /// <param name="bindingContext"></param>
        /// <returns></returns>
        public bool BindModel(HttpActionContext actionContext, ModelBindingContext bindingContext)
        {
            var request = actionContext.Request;

            if (!IsJsonRequest(request))
                return false;

            // Desserializes the JSON request using the .Net Json implementation.
            var model = Deserialize(bindingContext.ValueProvider.GetValue(JSON_PARAMETER_NAME).AttemptedValue);

            bindingContext.Model = model;

            return true;
        }

        /// <summary>
        /// Checks if a request is a JsonRequest or not. 
        /// You may override this to check for other values or indicators.
        /// </summary>
        /// <param name="request">The HttpRequestBase object representing the MVC request.</param>
        /// <returns>True if the ContentType contains "json", False otherwise.</returns>
        public virtual bool IsJsonRequest(HttpRequestMessage request)
        {
            return request.Content.Headers.ContentType.MediaType.Contains(JSON_PARAMETER_NAME);
        }

        /// <summary>
        /// When overriden, deserializes the JSON data into a DataTablesRequest object.
        /// </summary>
        /// <param name="jsonData">The JSON data to be deserialized.</param>
        /// <returns>The DataTablesRequest object.</returns>
        protected virtual IDataTablesRequest Deserialize(string jsonData)
        {
            return JsonConvert.DeserializeObject<T>(jsonData);
        }
    }
}
