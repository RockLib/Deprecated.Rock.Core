using System;
using System.Collections.Generic;

namespace DefaultHelperTests.Implementation
{
    public class Default_PrimitivishTypesTests : DefaultTestBase<IEnumerable<Type>, Type[]>
    {
        protected override string PropertyName
        {
            get { return "PrimitivishTypes"; }
        }
    }
}