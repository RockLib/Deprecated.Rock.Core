using Rock.DependencyInjection.Heuristics;

namespace DefaultHelperTests.Implementation
{
    public class Default_ResolverConstructorSelectorTests : DefaultTestBase<IResolverConstructorSelector, ResolverConstructorSelector>
    {
        protected override string PropertyName
        {
            get { return "ResolverConstructorSelector"; }
        }
    }
}