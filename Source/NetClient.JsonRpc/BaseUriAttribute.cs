using System;

namespace NetClient.JsonRpc
{
    /// <summary>
    ///     Specifies the base URI.
    /// </summary>
    public sealed class BaseUriAttribute : Attribute
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="BaseUriAttribute" /> class.
        /// </summary>
        /// <param name="baseUri">The base URI.</param>
        public BaseUriAttribute(string baseUri)
        {
            BaseUri = new Uri(baseUri);
        }

        /// <summary>
        ///     Gets the base URI.
        /// </summary>
        /// <value>The base URI.</value>
        public Uri BaseUri { get; }
    }
}