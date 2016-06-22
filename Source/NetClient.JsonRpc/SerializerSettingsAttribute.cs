using System;
using Newtonsoft.Json;

namespace NetClient.JsonRpc
{
    /// <summary>
    ///     Specifies the JSON serialization settings.
    /// </summary>
    public sealed class SerializerSettingsAttribute : Attribute
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="SerializerSettingsAttribute" /> class.
        /// </summary>
        /// <param name="type">The type.</param>
        public SerializerSettingsAttribute(Type type)
        {
            SerializerSettings = Activator.CreateInstance(type) as JsonSerializerSettings;
        }

        /// <summary>
        /// Gets the serializer settings.
        /// </summary>
        /// <value>The serializer settings.</value>
        public JsonSerializerSettings SerializerSettings { get; }
    }
}