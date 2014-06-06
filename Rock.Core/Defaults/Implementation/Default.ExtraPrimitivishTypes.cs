using System;
using System.Collections.Generic;
using System.Linq;

namespace Rock.Defaults.Implementation
{
    public static partial class Default
    {
        private static readonly DefaultHelper<IEnumerable<Type>> _extraPrimitivishTypes = new DefaultHelper<IEnumerable<Type>>(Enumerable.Empty<Type>);

        public static IEnumerable<Type> ExtraPrimitivishTypes
        {
            get { return _extraPrimitivishTypes.Current; }
        }

        public static IEnumerable<Type> DefaultExtraPrimitivishTypes
        {
            get { return _extraPrimitivishTypes.DefaultInstance; }
        }

        public static void SetExtraPrimitivishTypes(Func<IEnumerable<Type>> getExtraPrimitivishTypes)
        {
            _extraPrimitivishTypes.SetCurrent(getExtraPrimitivishTypes);
        }

        public static void RestoreDefaultExtraPrimitivishTypes()
        {
            _extraPrimitivishTypes.RestoreDefault();
        }
    }
}