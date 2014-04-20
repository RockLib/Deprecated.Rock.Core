using System;

namespace Rock.DependencyInjection
{
    public partial class AutoContainer
    {
        private class MergedAutoContainer : AutoContainer
        {
            private readonly IResolver _secondaryContainer;

            public MergedAutoContainer(AutoContainer primaryContainer, IResolver seconaryContainer)
                : base(primaryContainer)
            {
                _secondaryContainer = seconaryContainer;
            }

            public override bool CanResolve(Type type)
            {
                return base.CanResolve(type) || _secondaryContainer.CanResolve(type);
            }

            public override object Get(Type type)
            {
                return base.CanResolve(type) ? base.Get(type) : _secondaryContainer.Get(type);
            }
        }
    }
}
