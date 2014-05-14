using System;
using Rock.DependencyInjection.Heuristics;

namespace Rock.Defaults.Implementation
{
    public static partial class Default
    {
        private static readonly DefaultHelper<IConstructorSelector> _constructorSelector = new DefaultHelper<IConstructorSelector>(() => new ConstructorSelector());

        public static IConstructorSelector ConstructorSelector
        {
            get { return _constructorSelector.Current; }
        }

        public static IConstructorSelector DefaultConstructorSelector
        {
            get { return _constructorSelector.DefaultInstance; }
        }

        public static void SetConstructorSelector(Func<IConstructorSelector> getConstructorSelectorInstance)
        {
            _constructorSelector.SetCurrent(getConstructorSelectorInstance);
        }
    }
}