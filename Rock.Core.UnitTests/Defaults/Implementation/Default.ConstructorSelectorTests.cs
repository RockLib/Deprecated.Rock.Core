using Rock.DependencyInjection.Heuristics;

namespace DefaultHelperTests.Implementation
{
    public class Default_ConstructorSelectorTests : DefaultTestBase<IConstructorSelector, ConstructorSelector>
    {
        protected override string PropertyName
        {
            get { return "ConstructorSelector"; }
        }
    }
}