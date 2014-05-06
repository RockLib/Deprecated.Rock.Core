using System;

namespace Rock
{
    public sealed class Default<TContract>
    {
        private readonly Lazy<TContract> _defaultInstance;
        private Lazy<TContract> _currentInstance;

        public Default(Func<TContract> createDefaultInstance)
        {
            _defaultInstance = new Lazy<TContract>(createDefaultInstance);
            _currentInstance = _defaultInstance;
        }

        public TContract DefaultInstance
        {
            get { return _defaultInstance.Value; }
        }

        public TContract Current
        {
            get { return _currentInstance.Value; }
        }

        public void SetCurrent(Func<TContract> getInstance)
        {
            _currentInstance =
                getInstance == null
                    ? _defaultInstance
                    : new Lazy<TContract>(getInstance);
        }
    }
}
