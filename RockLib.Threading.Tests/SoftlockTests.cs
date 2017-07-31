using System;
using FluentAssertions;
using Xunit;

namespace RockLib.Threading.Tests
{
    public class SoftlockTests
    {
        [Fact]
        public void IsLockAcquired_WillReturnTrue_WhenPriorLockHasBeenAcquired()
        {
            var softlock = new SoftLock();

            softlock.TryAcquire();

            softlock.IsLockAcquired.Should().BeTrue();
        }

        [Fact]
        public void IsLockAcquired_WillReturnFalse_WhenPriorLockHasNotBeenAcquired()
        {
            var softlock = new SoftLock();

            // call nothing, should be false by default

            softlock.IsLockAcquired.Should().BeFalse();
        }

        [Fact]
        public void Release_WillReleaseExistingLock_WillSignalByIsLockAcquired()
        {
            var softlock = new SoftLock();

            softlock.TryAcquire();

            // prove it is locked
            softlock.IsLockAcquired.Should().BeTrue();

            softlock.Release();

            // verify it has been released
            softlock.IsLockAcquired.Should().BeFalse();
        }

        [Fact]
        public void TryAcquire_WillAcquireLock()
        {
            var softlock = new SoftLock();

            softlock.TryAcquire();

            // prove it is locked
            softlock.IsLockAcquired.Should().BeTrue();
            
        }
    }
}
