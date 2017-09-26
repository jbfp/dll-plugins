using System;

namespace ClassLibrary1
{
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class PluginAttribute : Attribute
    {
        public PluginAttribute(string name)
        {
            if (name is null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("Name must be provided.", nameof(name));
            }

            Name = name;
        }

        public string Name { get; }
    }
}
