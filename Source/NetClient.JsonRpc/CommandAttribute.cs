using System;

namespace NetClient.JsonRpc
{
    /// <summary>
    ///     Specifies the route.
    /// </summary>
    public sealed class CommandAttribute : Attribute
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="CommandAttribute" /> class.
        /// </summary>
        /// <param name="name">The template.</param>
        public CommandAttribute(string name)
        {
            Name = name;
        }

        /// <summary>
        ///     Gets the template.
        /// </summary>
        /// <value>The template.</value>
        public string Name { get; }
    }
}