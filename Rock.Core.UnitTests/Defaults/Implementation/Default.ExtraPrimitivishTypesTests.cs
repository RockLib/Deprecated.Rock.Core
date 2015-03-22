using System;
using System.Collections.Generic;

namespace DefaultHelperTests.Implementation
{
    internal class Default_PrimitivishTypesTests : DefaultTestBase<IEnumerable<Type>, Type[]>
    {
        protected override string PropertyName
        {
            get { return "ExtraPrimitivishTypes"; }
        }
    }
}