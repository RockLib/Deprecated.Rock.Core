using System;
using System.Collections.Generic;
using Rock.Extensions;

namespace Rock.Defaults.Implementation
{
    public static partial class Default
    {
        private static readonly DefaultHelper<IEnumerable<Type>> _primitivishTypes = new DefaultHelper<IEnumerable<Type>>(() =>
            new[]
            {
                typeof(string),
                typeof(decimal),
                typeof(DateTime),
                typeof(DateTimeOffset),
                typeof(Guid),
                typeof(TimeSpan)
            });

        public static IEnumerable<Type> PrimitivishTypes
        {
            get { return _primitivishTypes.Current; }
        }

        public static IEnumerable<Type> DefaultPrimitivishTypes
        {
            get { return _primitivishTypes.DefaultInstance; }
        }

        public static void SetPrimitivishTypes(Func<IEnumerable<Type>> getPrimitivishTypes)
        {
            _primitivishTypes.SetCurrent(getPrimitivishTypes);
            IsPrimitivishExtension.ClearCache();
        }

        public static void RestoreDefaultPrimitivishTypes()
        {
            _primitivishTypes.RestoreDefault();
            IsPrimitivishExtension.ClearCache();
        }
    }
}