using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace NetClient.JsonRpc
{
    /// <summary>
    ///     Provides a base class for sending requests and receiving responses over network boundaries using JSON-RPC.
    /// </summary>
    public abstract class JsonRpcClient : INetClient
    {
        private Uri baseUri;
        private JsonSerializerSettings serializerSettings;

        /// <summary>
        ///     Initializes a new instance of the <see cref="JsonRpcClient" /> class.
        /// </summary>
        protected JsonRpcClient()
        {
            foreach (var property in GetType().GetProperties())
            {
                //if (!property.PropertyType.IsGenericType || property.PropertyType.GetGenericTypeDefinition() != typeof(Resource<>)) continue;

                //var route = property.GetCustomAttribute<RouteAttribute>()?.Template;
                //var element = Activator.CreateInstance(property.PropertyType, this, property, null, null);

                //property.SetValue(this, element);
            }
        }

        /// <summary>
        ///     Gets or sets the base URI.
        /// </summary>
        /// <value>The base URI.</value>
        public Uri BaseUri
        {
            get
            {
                if (baseUri != null) return baseUri;

                var attribute = GetType().GetCustomAttributes(typeof(BaseUriAttribute), true).FirstOrDefault() as BaseUriAttribute;
                return attribute?.BaseUri;
            }
            set { baseUri = value; }
        }

        /// <summary>
        ///     Gets or sets the serializer settings.
        /// </summary>
        /// <value>The serializer settings.</value>
        public JsonSerializerSettings SerializerSettings
        {
            get
            {
                if (serializerSettings != null) return serializerSettings;

                var attribute = GetType().GetCustomAttributes(typeof(SerializerSettingsAttribute), true).FirstOrDefault() as SerializerSettingsAttribute;
                return attribute?.SerializerSettings;
            }
            set { serializerSettings = value; }
        }

        /// <summary>
        ///     Saves the changes asynchronously.
        /// </summary>
        /// <returns>Task.</returns>
        public Task SaveChangesAsync()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///     Gets or sets the error action.
        /// </summary>
        /// <value>The error action.</value>
        public Action<Exception> OnError { get; set; }
    }
}