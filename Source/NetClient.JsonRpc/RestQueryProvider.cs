using System;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace NetClient.JsonRpc
{
    /// <summary>
    ///     Defines methods to create and execute queries.
    /// </summary>
    /// <typeparam name="T">The type of element to query.</typeparam>
    internal class RestQueryProvider<T> : IQueryProvider
    {
        private readonly IElement<T> element;

        /// <summary>
        ///     Initializes a new instance of the <see cref="RestQueryProvider{T}" /> class.
        /// </summary>
        /// <param name="element">The element.</param>
        public RestQueryProvider(IElement<T> element)
        {
            this.element = element;
        }

        private Resource<T> Resource => element as Resource<T>;

        private async Task<TResult> GetRestValueAsync<TResult>(Expression expression)
        {
            var result = JsonConvert.DeserializeObject<TResult>($"[]");

            var resourceValues = new RestQueryTranslator().GetResourceValues(expression);
            if (string.IsNullOrWhiteSpace(Resource?.RouteTemplate))
            {
                element.OnError?.Invoke(new InvalidOperationException("Unable to obtain a value from the service because a route was not specified."));
                return result;
            }

            if (Resource?.BaseUri == null)
            {
                element.OnError?.Invoke(new InvalidOperationException("Unable to obtain a value from the service because a base URI was not specified."));
                return result;
            }

            var path = resourceValues.Aggregate(Resource.RouteTemplate, (current, resourceValue) => current.Replace($"{{{resourceValue.Key}}}", resourceValue.Value.ToString())).TrimStart('/');
            if (path.Contains("{") && path.Contains("}"))
            {
                element.OnError?.Invoke(new InvalidOperationException($"Route resolution has failed for the route tempate: \"{path}\". Please ensure all route placeholders can be replaced by an expression criteria."));
                return result;
            }

            var requestUri = new Uri($"{Resource.BaseUri.AbsoluteUri}{path}");

            using (var client = new HttpClient())
            {
                using (var response = await client.GetAsync(requestUri))
                {
                    using (var content = response.Content)
                    {
                        var json = await content.ReadAsStringAsync();
                        if (string.IsNullOrWhiteSpace(json)) return result;

                        if (Resource?.SerializerSettings == null)
                        {
                            result = JsonConvert.DeserializeObject<TResult>($"[{json}]");
                        }
                        else
                        {
                            result = JsonConvert.DeserializeObject<TResult>($"[{json}]", Resource.SerializerSettings);
                        }
                    }
                }
            }
            return result;
        }

        /// <summary>
        ///     Creates the query.
        /// </summary>
        /// <param name="expression">The expression.</param>
        /// <returns>IQueryable.</returns>
        public IQueryable CreateQuery(Expression expression)
        {
            return new Resource<T>(element.Client, Resource.Property, element.OnError, expression);
        }

        /// <summary>
        ///     Creates the query.
        /// </summary>
        /// <typeparam name="TElement">The type of the element.</typeparam>
        /// <param name="expression">The expression.</param>
        /// <returns>IQueryable&lt;TElement&gt;.</returns>
        public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
        {
            return (IQueryable<TElement>)new Resource<T>(element.Client, Resource.Property, element.OnError, expression);
        }

        /// <summary>
        ///     Executes the specified expression.
        /// </summary>
        /// <param name="expression">The expression.</param>
        /// <returns>System.Object.</returns>
        public object Execute(Expression expression)
        {
            return Execute<Resource<T>>(expression);
        }

        /// <summary>
        ///     Executes the specified expression.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="expression">The expression.</param>
        /// <returns>TResult</returns>
        public TResult Execute<TResult>(Expression expression)
        {
            var result = JsonConvert.DeserializeObject<TResult>($"[]");

            GetRestValueAsync<TResult>(expression).ContinueWith(task =>
            {
                if (task.IsCompleted && !task.IsFaulted)
                {
                    result = task.Result;
                }
                else
                {
                    element.OnError?.Invoke(task.Exception);
                }
            }).Wait();

            return result;
        }
    }
}