using System;
using System.Reflection;
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
        private readonly SoftLock _softLock = new SoftLock();

        private Lazy<T> _potentialInstance;
        private Lazy<T> _lockedInstance;

        private readonly Func<T> _getDefaultValue;

        /// <summary>
        /// Initializes a new instance of the <see cref="Semimutable{T}"/> class.
        /// </summary>
        /// <remarks>
        /// Calls <see cref="Semimutable{T}(T)"/>, passing the value of <c>default(T)</c> as the parameter.
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
        /// Calls <see cref="Semimutable{T}(Func{T})"/> passing a function that returns
        /// <paramref name="defaultValue"/> as the parameter.
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
            _getDefaultValue = getDefaultValue;
            _potentialInstance = new Lazy<T>(getDefaultValue);
            _lockedInstance = null;
            HasDefaultValue = true;
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
        /// Gets a value indicating whether this instance is locked.
        /// </summary>
        public bool IsLocked
        {
            get { return _lockedInstance != null; }
        }
        
        /// <summary>
        /// Gets a value indicating whether this instance has (or will have) the default value.
        /// </summary>
        public bool HasDefaultValue { get; private set; }

        /// <summary>
        /// Sets the <see cref="Value"/> property to this instance's original default value.
        /// </summary>
        public void ResetValue()
        {
            SetValue(_getDefaultValue);
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
        /// Gets a method that unlocks the <see cref="Value"/> property, allowing changes to be accepted.
        /// </summary>
        /// <remarks>
        /// This method should not be used "in production". Its main use is to help facilitate testing.
        /// </remarks>
        public MethodInfo GetUnlockValueMethod()
        {
            return new Action(UnlockValue).GetMethodInfo();
        }

        private void UnlockValue()
        {
            if (_lockedInstance != null)
            {
                lock (this)
                {
                    if (_lockedInstance != null)
                    {
                        _potentialInstance = _lockedInstance;
                        _lockedInstance = null;
                        _softLock.Release();
                    }
                }
            }
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
            // If at any time _lockedInstance has a value, exit the loop.
            while (_lockedInstance == null)
            {
                // Synchronize with the GetValue method - only one thread can have the lock at any one time.
                if (_softLock.TryAcquire())
                {
                    HasDefaultValue = (getValue == _getDefaultValue);

                    // If no other calls to SetValue are made, then getValue will be the value factory
                    // for _lockedInstance.
                    _potentialInstance = new Lazy<T>(getValue);

                    // Be sure to release the lock to allow other threads (and this thread later on) to
                    // set _potentialInstance.
                    _softLock.Release();

                    // Return from the method - our job is done and it might be a while until _lockedInstance has a value.
                    return;
                }
            }

            throw new InvalidOperationException("Setting the value of a Semimutable object is not permitted after it has been locked.");
        }

        private T GetValue()
        {
            // In the rare case that _lockedIntance is cleared (via the UnlockValue method)
            // after the while loop's null check and before its Value property is access,
            // capture _lockedIntance in a local variable to prevent a null reference exception.
            Lazy<T> local;

            // If _lockedInstance has been set already, then just return its value.
            while ((local = _lockedInstance) == null)
            {
                // Synchronize with the SetValue method - only one thread can have the lock at any one time.
                if (_softLock.TryAcquire())
                {
                    // _potentialInstance will be the new value for _lockedInstance.
                    var temp = _potentialInstance;

                    // Clear out the value for _potentialInstance before setting _lockedInstance.
                    _potentialInstance = null;

                    _lockedInstance = temp;

                    // Be sure to *not* release the lock. Otherwise a thread in the SetValue method could
                    // acquire the lock and set _potentialInstance, which will never be used and never be released,
                    // resulting in potential memory leaks.
                }
            }

            // If we used _lockedInstance instead of a local variable, it would be possible to have a null
            // reference exception when accessing the Value property.
            return local.Value;
        }
    }
}
