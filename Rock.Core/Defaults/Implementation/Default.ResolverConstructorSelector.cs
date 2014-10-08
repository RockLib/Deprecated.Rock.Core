using System;
using Rock.DependencyInjection.Heuristics;

namespace Rock.Defaults.Implementation
{
    public static partial class Default
    {
        private static readonly DefaultHelper<IResolverConstructorSelector> _resolverConstructorSelector = new DefaultHelper<IResolverConstructorSelector>(() => new ResolverConstructorSelector());

        public static IResolverConstructorSelector ResolverConstructorSelector
        {
            get { return _resolverConstructorSelector.Current; }
        }

        public static IResolverConstructorSelector DefaultResolverConstructorSelector
        {
            get { return _resolverConstructorSelector.DefaultInstance; }
        }

        public static void SetResolverConstructorSelector(Func<IResolverConstructorSelector> getResolverConstructorSelectorInstance)
        {
            _resolverConstructorSelector.SetCurrent(getResolverConstructorSelectorInstance);
        }

        public static void RestoreDefaultResolverConstructorSelector()
        {
            _resolverConstructorSelector.RestoreDefault();
        }
    }
}