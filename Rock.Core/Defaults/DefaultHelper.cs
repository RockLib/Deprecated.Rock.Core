using System;

namespace Rock.Defaults
{
    public sealed class DefaultHelper<TContract>
    {
        private readonly Lazy<TContract> _defaultInstance;
        private Lazy<TContract> _currentInstance;

        public DefaultHelper(Func<TContract> createDefaultInstance)
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

        public void RestoreDefault()
        {
            SetCurrent(null);
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
