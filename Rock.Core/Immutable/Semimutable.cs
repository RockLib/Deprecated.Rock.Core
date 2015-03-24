using System;
using Rock.Threading;

namespace Rock.Immutable
{
    /// <summary>
    /// Represents a "semimutable" value. Its value can be changed either via the setter of the <see cref="Value"/>
    /// property, or by calling the <see cref="SetValue"/> method. However, once the getter of the
    /// <see cref="Value"/> property is accessed or the <see cref="LockValue"/> method is called, the value will
    /// never change again.
    /// <para>
    /// It's like Schrödinger's cat - once you open the box, the cat's fate is sealed.
    /// </para>
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    public class Semimutable<T>
    {
        private readonly SoftLock _lock = new SoftLock();

        private Func<T> _getValue;
        private Lazy<T> _instance;

        /// <summary>
        /// Initializes a new instance of the <see cref="Semimutable{T}"/> class.
        /// </summary>
        /// <remarks>
        /// Calls <see cref="Semimutable{T}(T)"/>, passing the value of <c>default(T)</c>.
        /// </remarks>
        public Semimutable()
            : this(default(T))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Semimutable{T}"/> class.
        /// </summary>
        /// <param name="defaultValue">The default value.</param>
        /// <remarks>
        /// Calls <see cref="Semimutable{T}(Func{T})"/>, passing a function that returns
        /// <paramref name="defaultValue"/>.
        /// </remarks>
        public Semimutable(T defaultValue)
            : this(() => defaultValue)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Semimutable{T}"/> class.
        /// </summary>
        /// <param name="getDefaultValue">A function that returns the default value.</param>
        public Semimutable(Func<T> getDefaultValue)
        {
            _getValue = getDefaultValue;
            _instance = null;
        }

        /// <summary>
        /// Gets or sets the value of the semimutable object. The setter can be called multiple times, but
        /// only the last value "wins". Once the getter is called (or the <see cref="LockValue"/> method is
        /// called), the value is "locked" - any value passed to the setter is ignored from this point forward.
        /// </summary>
        public T Value
        {
            get { return GetValue(); }
            set { SetValue(() => value); }
        }

        /// <summary>
        /// Locks the <see cref="Value"/> property and prevents any further changes from being accepted.
        /// </summary>
        public void LockValue()
        {
            // Just call GetValue and ignore the result.
            GetValue();
        }

        /// <summary>
        /// Sets the value of the <see cref="Value"/> property using a function that will not be 
        /// evaluated until either the <see cref="Value"/> property is accessed (the getter), or the
        /// <see cref="LockValue"/> method is called.
        /// </summary>
        /// <param name="getValue">
        /// A function whose return value is used to set the <see cref="Value"/> property.
        /// </param>
        public void SetValue(Func<T> getValue)
        {
            // If at any time _instance has a value, exit the loop.
            while (_instance == null)
            {
                // Synchronize with the GetValue method - only one thread can have the lock at any one time.
                if (_lock.TryAcquire())
                {
                    // If no other calls to SetValue are made, then getValue will be the value factory
                    // for _instance.
                    _getValue = getValue;

                    // Be sure to release the lock to allow other threads (and this thread later on) to
                    // set _getValue.
                    _lock.Release();

                    // Break out of  the loop - our job is done and it might be a while until _instance has a value.
                    break;
                }
            }
        }

        private T GetValue()
        {
            // If _instance isn't null, then just return its value.
            while (_instance == null)
            {
                // Synchronize with the SetValue method - only one thread can have the lock at any one time.
                if (_lock.TryAcquire())
                {
                    // The current value of _getValue will be the value factory for _instance.
                    _instance = new Lazy<T>(_getValue);

                    // Now that _instance has been created with _getValue, we have no further use for it,
                    // so get rid of it.
                    _getValue = null;

                    // Be sure to *not* release the lock. Otherwise a thread in the SetValue method could
                    // acquire the lock and set _getValue, which will never be used and never be released,
                    // resulting in potential memory leaks.
                }
            }

            return _instance.Value;
        }
    }
}
