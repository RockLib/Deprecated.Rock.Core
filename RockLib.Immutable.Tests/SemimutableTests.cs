using System;
using Xunit;

namespace RockLib.Immutable.Tests
{
    public class SemimutableTests
    {
        [Fact]
        public void DefaultValueIsUsedWhenValueIsNotChanged()
        {
            var semimutable = new Semimutable<int>(1);

            Assert.Equal(1, semimutable.Value);
        }

        [Fact]
        public void CanChangeValueProperty()
        {
            var semimutable = new Semimutable<int>(1);

            semimutable.Value = 2;

            Assert.Equal(2, semimutable.Value);
        }

        [Fact]
        public void CanChangeValuePropertyWithTheSetValueMethod()
        {
            var semimutable = new Semimutable<int>(1);

            semimutable.SetValue(() => 2);

            Assert.Equal(2, semimutable.Value);
        }

        [Fact]
        public void ResetChangesValueBackToDefault()
        {
            var semimutable = new Semimutable<int>(1);

            semimutable.Value = 2;

            semimutable.ResetValue();

            Assert.True(semimutable.HasDefaultValue);
        }

        [Fact]
        public void HasDefaultValueIsTrueWhenValueIsNotChanged()
        {
            var semimutable = new Semimutable<int>(1);

            Assert.True(semimutable.HasDefaultValue);
        }

        [Fact]
        public void HasDefaultValueIsFalseWhenValueIsChanged()
        {
            var semimutable = new Semimutable<int>(1);

            semimutable.Value = 2;

            Assert.False(semimutable.HasDefaultValue);
        }

        [Fact]
        public void HasDefaultValueIsFalseWhenSetValueIsCalled()
        {
            var semimutable = new Semimutable<int>(1);

            semimutable.SetValue(() => 2);

            Assert.False(semimutable.HasDefaultValue);
        }

        [Fact]
        public void HasDefaultValueIsTrueWhenResetValueIsCalled()
        {
            var semimutable = new Semimutable<int>(1);

            semimutable.Value = 2;
            semimutable.ResetValue();

            Assert.True(semimutable.HasDefaultValue);
        }

        [Fact]
        public void IsLockedIsFalseInitially()
        {
            var semimutable = new Semimutable<int>(1);

            Assert.False(semimutable.IsLocked);
        }

        [Fact]
        public void IsLockedIsTrueAfterTheValuePropertyIsAccessed()
        {
            var semimutable = new Semimutable<int>(1);

            var value = semimutable.Value;

            Assert.True(semimutable.IsLocked);
        }

        [Fact]
        public void IsLockedIsTrueAfterLockValueIsCalled()
        {
            var semimutable = new Semimutable<int>(1);

            semimutable.LockValue();

            Assert.True(semimutable.IsLocked);
        }

        [Fact]
        public void SettingTheValuePropertyThrowsWhenIsLockedIsTrue()
        {
            var semimutable = new Semimutable<int>(1);

            semimutable.LockValue();

            Assert.Throws<InvalidOperationException>(() => semimutable.Value = 2);
        }

        [Fact]
        public void CallingTheSetValueMethodThrowsWhenIsLockedIsTrue()
        {
            var semimutable = new Semimutable<int>(1);

            semimutable.LockValue();

            Assert.Throws<InvalidOperationException>(() => semimutable.SetValue(() => 2));
        }

        [Fact]
        public void CallingTheResetValueMethodThrowsWhenIsLockedIsTrue()
        {
            var semimutable = new Semimutable<int>(1);

            semimutable.LockValue();

            Assert.Throws<InvalidOperationException>(() => semimutable.ResetValue());
        }

        [Fact]
        public void InvokingTheMethodReturnedFromGetUnlockMethodUnlocksTheValue()
        {
            var semimutable = new Semimutable<int>(1);

            semimutable.LockValue();

            var unlockValue = semimutable.GetUnlockValueMethod();
            unlockValue.Invoke(semimutable, null);

            Assert.False(semimutable.IsLocked);

            semimutable.Value = 2;

            Assert.Equal(2, semimutable.Value);
        }
    }
}
