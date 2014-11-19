using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Security.Policy;
using System.Text;
using System.Web;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Metadata;
using System.Web.Http.Metadata.Providers;
using System.Web.Http.ModelBinding;
using System.Web.Http.Routing;
using DataTables.Core;
using FakeItEasy;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DataTables.WebApi.Test
{
    [TestClass]
    public class DataTablesWebApiBinderTest
    {

        private DataTablesWebApiBinderTestContext _c;

        [TestMethod]
        public void When_a_request_with_query_parameters_is_bound_the_result_is_successful()
        {
            var dataRequest = _c.GetValidRequest();

            var binder = _c.GetBinder(dataRequest, true);

            var bindingContext = _c.GetBindingContext();

            var result = binder.BindModel(_c.ActionContext, bindingContext);

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void When_a_request_with_query_parameters_is_bound_the_model_is_populated()
        {
            var dataRequest = _c.GetValidRequest();

            var binder = _c.GetBinder(dataRequest, true);

            var bindingContext = _c.GetBindingContext();

            binder.BindModel(_c.ActionContext, bindingContext);

            var boundModel = bindingContext.Model as IDataTablesRequest;

            Assert.IsNotNull(boundModel);
        }

        [TestInitialize]
        public void Init()
        {
            _c = new DataTablesWebApiBinderTestContext();
        }

        private class DataTablesWebApiBinderTestContext
        {
            public HttpActionContext ActionContext;

            public DataTablesWebApiBinder<T> GetBinder<T>(T data, bool useQueryString = true)
                where T : IDataTablesRequest, new()
            {
                const string baseUrl = "http://localhost/api/data/";

                var request = new HttpRequestMessage();

                var content = GetContent(data);

                if (useQueryString)
                {
                    var urlBuilder = new StringBuilder(baseUrl + "?");
                    foreach (var kv in content)
                    {
                        urlBuilder.Append(EncodeKeyValuePair(kv.Key, kv.Value) + "&");
                    }

                    request.RequestUri = new Uri(urlBuilder.ToString().TrimEnd('&'));
                }
                else
                {
                    request.RequestUri = new Uri(baseUrl);
                    request.Content = new FormUrlEncodedContent(GetContent(data));
                }

                var controllerContext = new HttpControllerContext(new HttpConfiguration(), A.Fake<IHttpRouteData>(), request);
                ActionContext = new HttpActionContext(controllerContext, A.Fake<HttpActionDescriptor>());

                return new DataTablesWebApiBinder<T>();
            }

            public DefaultDataTablesRequest GetValidRequest()
            {
                return new DefaultDataTablesRequest
                {
                    Draw = 42,
                    Start = 60,
                    Length = 20,
                    Search = new Search("foo", true),
                    Columns = new ColumnCollection(new List<Column>
                    {
                        new Column("data", "name", true, true, "foo", true),
                        new Column("data1", "name1", false, false, "", false)
                    })
                };
            }

            public ModelBindingContext GetBindingContext()
            {
                return new ModelBindingContext
                {
                    ModelMetadata = new ModelMetadata(new EmptyModelMetadataProvider(),
                                                      null,
                                                      null,
                                                      typeof(DefaultDataTablesRequest),
                                                      null)
                };
            }


            private static IEnumerable<KeyValuePair<string, string>> GetContent(IDataTablesRequest data)
            {
                var dictionary = new Dictionary<string, string>();
                dictionary.Add("draw", data.Draw.ToString("d"));
                dictionary.Add("start", data.Start.ToString("d"));
                dictionary.Add("length", data.Length.ToString("d"));
                dictionary.Add("search[value]", data.Search.Value);
                dictionary.Add("search[regex]", data.Search.IsRegexValue.ToString().ToLower());

                var idx = 0;
                foreach (var column in data.Columns)
                {
                    var columnId = "columns[" + idx + "]";
                    dictionary.Add(columnId + "[data]", column.Data);
                    dictionary.Add(columnId + "[name]", column.Name);
                    dictionary.Add(columnId + "[searchable]", column.Searchable.ToString().ToLower());
                    dictionary.Add(columnId + "[orderable]", column.Orderable.ToString().ToLower());
                    if (column.Search != null)
                    {
                        dictionary.Add(columnId + "[search][value]", column.Search.Value);
                        dictionary.Add(columnId + "[search][regex]", column.Search.IsRegexValue.ToString().ToLower());
                    }

                    if (column.IsOrdered)
                    {
                        dictionary.Add("order[" + column.OrderNumber + "][column]", idx.ToString("d"));
                        dictionary.Add("order[" + column.OrderNumber + "][dir]", column.SortDirection.ToString("d"));
                    }
                    ++idx;
                }

                return dictionary;
            }

            private static string EncodeKeyValuePair(string key, string value)
            {
                return HttpUtility.UrlEncode(key) + "=" + HttpUtility.UrlEncode(value ?? "");
            }
        }
    }
}
