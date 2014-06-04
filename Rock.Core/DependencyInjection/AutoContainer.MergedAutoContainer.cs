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

            public override bool CanGet(Type type)
            {
                return base.CanGet(type) || _secondaryContainer.CanGet(type);
            }

            public override object Get(Type type)
            {
                return
                    base.CanGet(type)
                        ? base.Get(type)
                        : _secondaryContainer.CanGet(type)
                            ? _secondaryContainer.Get(type)
                            : ThrowResolveException(type);
            }

            private static object ThrowResolveException(Type type)
            {
                throw new ResolveException("Neither the primary AutoContainer nor the secondary IResolver were able to retrieve an instance of type " + type);
            }
        }
    }
}
