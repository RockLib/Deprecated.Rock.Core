using System;

// ReSharper disable once CheckNamespace
namespace Rock.StaticDependencyInjection
{
    /// <summary>
    /// Indicates that a class should be exported as a static dependency.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public sealed class ExportAttribute : Attribute
    {
        private readonly int _priority;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExportAttribute"/> class.
        /// </summary>
        public ExportAttribute()
            : this(0)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExportAttribute"/> class.
        /// </summary>
        /// <param name="priority">The priority of the class.</param>
        public ExportAttribute(int priority)
        {
            _priority = priority;
        }

        /// <summary>
        /// Gets a value that indicates this class's relative priority.
        /// </summary>
        public int Priority { get { return _priority; } }

        /// <summary>
        /// Gets a value indicating whether this class is explicitly ineligible for exporting.
        /// </summary>
        public bool Disabled { get; set; }

        /// <summary>
        /// Gets or sets the arbitrary name of this export. Named import operations use
        /// this value to filter eligible results.
        /// </summary>
        public string Name { get; set; }
    }
}
