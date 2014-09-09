using System;

namespace Rock.Collections
{
    public class DeepEqualityComparerConfiguration : DeepEqualityComparer.IConfiguration
    {
        public StringComparison StringComparison { get; set; }
    }
}