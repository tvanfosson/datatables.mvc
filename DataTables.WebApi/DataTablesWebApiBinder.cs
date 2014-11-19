﻿#region Copyright
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

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Net.Http;
using System.Web.Http.Controllers;
using System.Web.Http.ModelBinding;
using DataTables.Core;

namespace DataTables.WebApi
{
    /// <summary>
    /// Defines a DataTables binder to bind a model with the request parameters from DataTables.
    /// </summary>
    public class DataTablesWebApiBinder<T> : IModelBinder
        where T : IDataTablesRequest, new()
    {

        /// <summary>
        /// Binds a new model with the DataTables request parameters.
        /// You should override this method to provide a custom type for internal binding to procees.
        /// </summary>
        /// <param name="actionContext">The context for the controller.</param>
        /// <param name="bindingContext">The context for the binding.</param>
        /// <returns>Your model with all it's properties set.</returns>
        public bool BindModel(HttpActionContext actionContext, ModelBindingContext bindingContext)
        {
            return BindModel(actionContext, bindingContext, typeof(T));
        }

        /// <summary>
        /// Binds a new model with both DataTables and your custom parameters.
        /// You should not override this method unless you're using request methods other than GET/POST.
        /// If that's the case, you'll have to override ResolveNameValueCollection too.
        /// </summary>
        /// <param name="actionContext">The context for the request.</param>
        /// <param name="bindingContext">The context for the binding.</param>
        /// <param name="modelType">The type of the model which will be created. Should implement IDataTablesRequest.</param>
        /// <returns>Your model with all it's properties set.</returns>
        protected virtual bool BindModel(HttpActionContext actionContext, ModelBindingContext bindingContext, Type modelType)
        {
            var request = actionContext.Request;
            var model = (IDataTablesRequest)Activator.CreateInstance(modelType);

            // We could use the `bindingContext.ValueProvider.GetValue("key")` approach but
            // directly accessing the HttpValueCollection will improve performance if you have
            // more than 2 registered value providers.
            try
            {
                var requestParameters = ResolveNameValueCollection(request);

                var formats = ParameterFormats.GetFormats(request.Method);

                // Populates the model with the draw count from DataTables.
                model.Draw = Get<int>(requestParameters, "draw");

                // Populates the model with page info (server-side paging).
                model.Start = Get<int>(requestParameters, "start");
                model.Length = Get<int>(requestParameters, "length");

                // Populates the model with search (global search).
                var searchValue = Get<string>(requestParameters, formats.SearchValue);
                var searchRegex = Get<bool>(requestParameters, formats.SearchRegex);
                model.Search = new Search(searchValue, searchRegex);

                // Get's the column collection from the request parameters.
                var columns = GetColumns(requestParameters, formats);

                // Parse column ordering.
                ParseColumnOrdering(requestParameters, columns, formats);

                // Attach columns into the model.
                model.Columns = new ColumnCollection(columns);

                // Map aditional properties into your custom request.
                MapAdditionalProperties(model, requestParameters);

                // Returns the filled model.
                bindingContext.Model = model;

                return true;
            }
            catch (ArgumentException)
            {
                return false;
            }
        }

        /// <summary>
        /// Map aditional properties (aditional fields sent with DataTables) into your custom implementation of IDataTablesRequest.
        /// You should override this method to map aditional info (non-standard DataTables parameters) into your custom 
        /// implementation of IDataTablesRequest.
        /// </summary>
        /// <param name="requestModel">The request model which will receive your custom data.</param>
        /// <param name="requestParameters">Parameters sent with the request.</param>
        protected virtual void MapAdditionalProperties(IDataTablesRequest requestModel, NameValueCollection requestParameters)
        {
        }

        /// <summary>
        /// Resolves the NameValueCollection from the request.
        /// Default implementation supports only GET and POST methods.
        /// You may override this method to support other HTTP verbs.
        /// </summary>
        /// <param name="request">The HttpRequestBase object that represents the MVC request.</param>
        /// <returns>The dictionary with request variables.</returns>
        protected virtual NameValueCollection ResolveNameValueCollection(HttpRequestMessage request)
        {
            if (request.Method == HttpMethod.Get)
            {
                var collection = new NameValueCollection();
                foreach (var kv in request.GetQueryNameValuePairs())
                {
                    collection.Add(kv.Key, kv.Value);
                }
                return collection;
            }

            if (request.Method == HttpMethod.Post)
            {
                var content = request.Content.ReadAsFormDataAsync();
                content.Wait();

                return content.Result;
            }

            throw new ArgumentException(String.Format("The provided HTTP method ({0}) is not a valid method to use with DataTablesBinder. Please, use HTTP GET or POST methods only.", request.Method), "method");
        }

        /// <summary>
        /// Get's a typed value from the collection using the provided key.
        /// This method is provided as an option for you to override the default behavior and add aditional
        /// check or change the returned value.
        /// </summary>
        /// <typeparam name="TValue">The type of the object to be returned.</typeparam>
        /// <param name="collection">The collection.</param>
        /// <param name="key">The key to access the collection item.</param>
        /// <returns>The stringly-typed object.</returns>
        protected virtual TValue Get<TValue>(NameValueCollection collection, string key)
        {
            return collection.G<TValue>(key);
        }

        /// <summary>
        /// Return's the column collection from the request values.
        /// This method is provided as an option for you to override the default behavior and add aditional
        /// check or change the returned value.
        /// </summary>
        /// <param name="collection">The request value collection.</param>
        /// <param name="formats">Formatting constants to use for the request type to extract column parameters.</param>
        /// <returns>The collumn collection or an empty list. For default behavior, do not return null!</returns>
        protected virtual IList<Column> GetColumns(NameValueCollection collection, ParameterFormats formats)
        {
            try
            {
                var columns = new List<Column>();

                // Loop through every request parameter to avoid missing any DataTable column.
                for (int i = 0; i < collection.Count; i++)
                {
                    var columnData = Get<string>(collection, string.Format(formats.ColumnDataFormat, i));

                    var columnName = Get<string>(collection, string.Format(formats.ColumnNameFormat, i));

                    if (columnData != null && columnName != null)
                    {
                        var columnSearchable = Get<bool>(collection, string.Format(formats.ColumnSearchableFormat, i));
                        var columnOrderable = Get<bool>(collection, string.Format(formats.ColumnOrderableFormat, i));
                        var columnSearchValue = Get<string>(collection, string.Format(formats.ColumnSearchValueFormat, i));
                        var columnSearchRegex = Get<bool>(collection, string.Format(formats.ColumnSearchRegexFormat, i));

                        columns.Add(new Column(columnData, columnName, columnSearchable, columnOrderable, columnSearchValue, columnSearchRegex));
                    }
                    else break; // Stops iterating because there's no more columns.
                }

                return columns;
            }
            catch
            {
                // Returns an empty column collection to avoid null exceptions.
                return new List<Column>();
            }
        }

        /// <summary>
        /// Configure column's ordering.
        /// This method is provided as an option for you to override the default behavior.
        /// </summary>
        /// <param name="collection">The request value collection.</param>
        /// <param name="columns">The column collection as returned from GetColumns method.</param>
        /// <param name="formats">Formatting strings to use for the request type to extract ordering parameters</param>
        protected virtual void ParseColumnOrdering(NameValueCollection collection, IList<Column> columns, ParameterFormats formats)
        {
            for (var i = 0; i < collection.Count; i++)
            {
                var orderColumn = Get<int>(collection, String.Format(formats.OrderColumnFormat, i));
                var orderDirection = Get<string>(collection, String.Format(formats.OrderDirectionFormat, i));

                if (orderColumn > -1 && orderDirection != null)
                {
                    columns[orderColumn].SetColumnOrdering(i, orderDirection);
                }
            }
        }
    }
}