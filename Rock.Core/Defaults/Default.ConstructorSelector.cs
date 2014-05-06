using System;
using Rock.DependencyInjection.Heuristics;

namespace Rock
{
    public static partial class Default
    {
        private static readonly Default<IConstructorSelector> _defaultConstructorSelector = new Default<IConstructorSelector>(() => new ConstructorSelector());

        public static IConstructorSelector ConstructorSelector
        {
            get { return _defaultConstructorSelector.Current; }
        }

        public static void SetConstructorSelector(Func<IConstructorSelector> getConstructorSelectorInstance)
        {
            _defaultConstructorSelector.SetCurrent(getConstructorSelectorInstance);
        }
    }
}