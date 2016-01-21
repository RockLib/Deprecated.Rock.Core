using System;
using NUnit.Framework;
using Rock.Extensions;

namespace Rock.Extensions
{
    [TestFixture]
    public class ActionExtensionsTest
    {
        private int actionCalledCount = 0;

        [SetUp]
        public void TestInitialize()
        {
            actionCalledCount = 0;
        }

        [Test]
        public void Action_Executed_ActuallyExecuted()
        {
            Action action = new Action(() => CustomThrower(0));

            action.Retry<CustomException>(3, 0);

            Assert.AreEqual(1, actionCalledCount);
        }

        [Test]
        public void FailingAction_Executed_TriedThreeTimes()
        {
            Action action = CustomThrower<CustomException>;

            try
            {
                action.Retry<CustomException>(3, 0);
            }
            catch (CustomException)
            {
                Assert.AreEqual(3, actionCalledCount);
                return;
            }

            Assert.Fail();
        }

        [Test]
        public void TwoExpectedActionExceptions_Executed_Executes()
        {
            Action action = new Action(() => CustomThrower(0));

            action.Retry<CustomException, CustomException>(3, 0);

            Assert.AreEqual(1, actionCalledCount);
        }

        [Test]
        public void ThreeExpectedActionExceptions_Executed_Executes()
        {
            Action action = new Action(() => CustomThrower(0));

            action.Retry<CustomException, CustomException, CustomException>(3, 0);

            Assert.AreEqual(1, actionCalledCount);
        }

        [Test]
        public void ActionThrowingUnexpectedException_Executed_ExecutedOnce()
        {
            Action action = CustomThrower<CustomException1>;

            try
            {
                action.Retry<CustomException2>(3, 0);
            }
            catch (CustomException1)
            {
                Assert.AreEqual(1, actionCalledCount);
                return;
            }

            Assert.Fail();
        }

        [Test]
        public void ActionThrowingExpectedExceptions_Executed_Retried()
        {
            Action action = RotatingCustomThrower;

            try
            {
                action.Retry<CustomException1, CustomException2, CustomException3>(3, 0);
            }
            catch (CustomException)
            {
                Assert.AreEqual(3, actionCalledCount);
                return;
            }

            Assert.Fail();
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void NullFunctionArgument_Executed_ArgumentNullExceptionThrown()
        {
            Action action = null;

            action.Retry<CustomException>(3, 3);

            Assert.Fail();
        }

        [Test]
        [ExpectedException(typeof(CustomException2))]
        public void ActionThrowingException2_Executed_Thrown()
        {
            Action action = CustomThrower<CustomException2>;

            action.Retry<CustomException1, CustomException2, CustomException3>(0, 0);

            Assert.Fail();
        }

        [Test]
        [ExpectedException(typeof(CustomException3))]
        public void ActionThrowingException3_Executed_Thrown()
        {
            Action action = CustomThrower<CustomException3>;

            action.Retry<CustomException1, CustomException2, CustomException3>(0, 0);

            Assert.Fail();
        }

        [Test]
        public void ActionWithGoofyParameters_Executed_Executed()
        {
            Action action = new Action(() => CustomThrower(0));

            action.Retry<CustomException>(-1, -1);

            Assert.AreEqual(1, actionCalledCount);
        }

        private class CustomException : Exception { }
        private class CustomException1 : CustomException { }
        private class CustomException2 : CustomException { }
        private class CustomException3 : CustomException { }

        public void CustomThrower<TException>()
            where TException : Exception, new()
        {
            actionCalledCount++;
            throw new TException();
        }

        public int CustomThrower(int throwUntil)
        {
            actionCalledCount++;
            if (actionCalledCount < throwUntil)
            {
                throw new CustomException();
            }

            return 42;
        }

        public void RotatingCustomThrower()
        {
            actionCalledCount++;
            if (actionCalledCount % 3 == 0)
            {
                throw new CustomException1();
            }
            if (actionCalledCount % 3 == 1)
            {
                throw new CustomException2();
            }
            if (actionCalledCount % 3 == 2)
            {
                throw new CustomException3();
            }
        }

        #region 1 exception
        [Test]
        public void Retry1Parameter_1Exception_Successful()
        {
            int result = 0;
            Action<int> action = (val1) => { result = val1; };
            action.Retry<int, Exception>(1, 1, 1000);
            Assert.AreEqual(1, result);
        }

        [Test]
        public void Retry2Parameter_1Exception_Successful()
        {
            int result = 0;
            Action<int, int> action = (val1, val2) => { result = val1 + val2; };
            action.Retry<int, int, Exception>(1, 2, 1, 1000);
            Assert.AreEqual(3, result);
        }

        [Test]
        public void Retry3Parameter_1Exception_Successful()
        {
            int result = 0;
            Action<int, int, int> action = (val1, val2, val3) => { result = val1 + val2 + val3; };
            action.Retry<int, int, int, Exception>(1, 2, 3, 1, 1000);
            Assert.AreEqual(6, result);
        }

        [Test]
        [ExpectedException(typeof(Exception))]
        public void Retry3Parameter_1Exception_RaiseExceptionAfterMaxAttempts()
        {
            int attempts = 0;
            Action<int, int, int> action = (val1, val2, val3) => { attempts++; throw new Exception(); };
            try
            {
                action.Retry<int, int, int, Exception>(1, 2, 3, 3, 1000);
            }
            catch
            {
                Assert.AreEqual(3, attempts);
                throw;
            }
            Assert.Fail();
        }

        [Test]
        [ExpectedException(typeof(Exception))]
        public void Retry3Parameter_1Exception_RaiseUnexpectedException()
        {
            int attempts = 0;
            Action<int, int, int> action = (val1, val2, val3) => { attempts++; throw new Exception(); };
            try
            {
                action.Retry<int, int, int, System.IO.IOException>(1, 2, 3, 3, 1000);
            }
            catch
            {
                Assert.AreEqual(1, attempts);
                throw;
            }
            Assert.Fail();
        }

        #region predicates

        [Test]
        public void RetryPredicate_1Exception_Successful()
        {
            int result = 0;
            Action action = () => { result = 1; };
            action.Retry(1, 1000, (Exception ex) => true);
            Assert.AreEqual(1, result);
        }

        [Test]
        public void RetryPredicate_1Exception_FalsePredicate_Successful()
        {
            int result = 0;
            Action action = () => { result = 1; };
            action.Retry(1, 1000, (Exception ex) => false);
            Assert.AreEqual(1, result);
        }

        [Test]
        [ExpectedException(typeof(Exception))]
        public void RetryPredicate_1Exception_RaiseExceptionAfterMaxAttempts()
        {
            int attempts = 0;
            Action action = () =>
            {
                attempts++;
                throw new Exception();
            };
            try
            {
                action.Retry(3, 1000, (Exception ex) => true);
            }
            catch
            {
                Assert.AreEqual(3, attempts);
                throw;
            }
            Assert.Fail();
        }

        [Test]
        [ExpectedException(typeof(Exception))]
        public void RetryPredicate_1Exception_RaiseExceptionFalsePredicate()
        {
            int attempts = 0;
            Action action = () =>
            {
                attempts++;
                throw new Exception();
            };
            try
            {
                action.Retry(3, 1000, (Exception ex) => false);
            }
            catch
            {
                Assert.AreEqual(1, attempts);
                throw;
            }
            Assert.Fail();
        }

        [Test]
        [ExpectedException(typeof(Exception))]
        public void RetryPredicate_1Exception_RaiseException_FalsePredicate2ndRetry()
        {
            int attempts = 0;
            bool Retry = true;
            Action action = () =>
            {
                attempts++;
                throw new Exception();
            };
            try
            {
                action.Retry(3, 1000, (Exception ex) =>
                {
                    var result = Retry;
                    Retry = false;
                    return result;
                });
            }
            catch
            {
                Assert.AreEqual(2, attempts);
                throw;
            }
            Assert.Fail();
        }

        [Test]
        [ExpectedException(typeof(Exception))]
        public void RetryPredicate_1Exception_RaiseUnexpectedException()
        {
            int attempts = 0;
            Action action = () =>
            {
                attempts++;
                throw new Exception();
            };
            try
            {
                action.Retry<System.IO.IOException>(3, 1000, ex=>true);
            }
            catch
            {
                Assert.AreEqual(1, attempts);
                throw;
            }
            Assert.Fail();
        }

        [Test]
        [ExpectedException(typeof(Exception))]
        public void RetryPredicate_1Exception_RaiseUnexpectedExceptionFalsePredicate()
        {
            int attempts = 0;
            Action action = () =>
            {
                attempts++;
                throw new Exception();
            };
            try
            {
                action.Retry<System.IO.IOException>(3, 1000, ex => false);
            }
            catch
            {
                Assert.AreEqual(1, attempts);
                throw;
            }
            Assert.Fail();
        }

        #endregion predicates

        #endregion

        #region 2 exceptions
        [Test]
        public void Retry1Parameter_2Exceptions_Successful()
        {
            int result = 0;
            Action<int> action = (val1) => { result = val1; };
            action.Retry<int, System.IO.IOException, AccessViolationException>(1, 1, 1000);
            Assert.AreEqual(1, result);
        }

        [Test]
        public void Retry2Parameter_2Exceptions_Successful()
        {
            int result = 0;
            Action<int, int> action = (val1, val2) => { result = val1 + val2; };
            action.Retry<int, int, System.IO.IOException, AccessViolationException>(1, 2, 1, 1000);
            Assert.AreEqual(3, result);
        }

        [Test]
        public void Retry3Parameter_2Exceptions_Successful()
        {
            int result = 0;
            Action<int, int, int> action = (val1, val2, val3) => { result = val1 + val2 + val3; };
            action.Retry<int, int, int, System.IO.IOException, AccessViolationException>(1, 2, 3, 1, 1000);
            Assert.AreEqual(6, result);
        }

        [Test]
        [ExpectedException(typeof(AccessViolationException))]
        public void Retry3Parameter_2Exceptions_RaiseException2ndExcpetionAfterMaxAttempts()
        {
            int attempts = 0;
            Action<int, int, int> action = (val1, val2, val3) => { attempts++; throw new AccessViolationException(); };
            try
            {
                action.Retry<int, int, int, System.IO.IOException, AccessViolationException>(1, 2, 3, 3, 1000);
            }
            catch
            {
                Assert.AreEqual(3, attempts);
                throw;
            }
            Assert.Fail();
        }

        [Test]
        [ExpectedException(typeof(System.IO.IOException))]
        public void Retry3Parameter_2Exceptions_RaiseException1stExcpetionAfterMaxAttempts()
        {
            int attempts = 0;
            Action<int, int, int> action = (val1, val2, val3) => { attempts++; throw new System.IO.IOException(); };
            try
            {
                action.Retry<int, int, int, System.IO.IOException, AccessViolationException>(1, 2, 3, 3, 1000);
            }
            catch
            {
                Assert.AreEqual(3, attempts);
                throw;
            }
            Assert.Fail();
        }

        [Test]
        [ExpectedException(typeof(Exception))]
        public void Retry3Parameter_2Exceptions_RaiseUnexpectedException()
        {
            int attempts = 0;
            Action<int, int, int> action = (val1, val2, val3) => { attempts++; throw new Exception(); };
            try
            {
                action.Retry<int, int, int, System.IO.IOException, AccessViolationException>(1, 2, 3, 3, 1000);
            }
            catch
            {
                Assert.AreEqual(1, attempts);
                throw;
            }
            Assert.Fail();
        }

        #region predicates

        [Test]
        public void RetryPredicate_2Exceptions_Successful()
        {
            int result = 0;
            Action action = () => { result = 1; };
            action.Retry(1, 1000, (System.IO.IOException ex) => true, (AccessViolationException ex) => true);
            Assert.AreEqual(1, result);
        }

        [Test]
        public void RetryPredicate_2Exceptions_FalsePredicate_Successful()
        {
            int result = 0;
            Action action = () => { result = 1; };
            action.Retry(1, 1000, (System.IO.IOException ex) => false, (AccessViolationException ex) => false);
            Assert.AreEqual(1, result);
        }

        [Test]
        [ExpectedException(typeof(System.IO.IOException))]
        public void RetryPredicate_2Exceptions_Raise1stExceptionAfterMaxAttempts()
        {
            int attempts = 0;
            Action action = () =>
            {
                attempts++;
                throw new System.IO.IOException();
            };
            try
            {
                action.Retry(3, 1000, (System.IO.IOException ex) => true, (AccessViolationException ex) => true);
            }
            catch
            {
                Assert.AreEqual(3, attempts);
                throw;
            }
            Assert.Fail();
        }

        [Test]
        [ExpectedException(typeof (System.IO.IOException))]
        public void RetryPredicate_2Exceptions_Raise1stExceptionFalsePredicate()
        {
            int attempts = 0;
            Action action = () =>
            {
                attempts++;
                throw new System.IO.IOException();
            };
            try
            {
                action.Retry(3, 1000, (System.IO.IOException ex) => false, (AccessViolationException ex) => false);
            }
            catch
            {
                Assert.AreEqual(1, attempts);
                throw;
            }
            Assert.Fail();
        }

        [Test]
        [ExpectedException(typeof(System.IO.IOException))]
        public void RetryPredicate_2Exceptions_Raise1stException_FalsePredicate2ndRetry()
        {
            int attempts = 0;
            bool Retry = true;
            Action action = () =>
            {
                attempts++;
                throw new System.IO.IOException();
            };
            try
            {
                action.Retry(3, 1000, (System.IO.IOException ex) =>
                {
                    var result = Retry;
                    Retry = false;
                    return result;
                }, (AccessViolationException ex) => false);
            }
            catch
            {
                Assert.AreEqual(2, attempts);
                throw;
            }
            Assert.Fail();
        }

        [Test]
        [ExpectedException(typeof(AccessViolationException))]
        public void RetryPredicate_2Exceptions_Raise2ndExceptionAfterMaxAttempts()
        {
            int attempts = 0;
            Action action = () =>
            {
                attempts++;
                throw new AccessViolationException();
            };
            try
            {
                action.Retry(3, 1000, (System.IO.IOException ex) => true, (AccessViolationException ex) => true);
            }
            catch
            {
                Assert.AreEqual(3, attempts);
                throw;
            }
            Assert.Fail();
        }
        
        [Test]
        [ExpectedException(typeof(AccessViolationException))]
        public void RetryPredicate_2Exceptions_Raise2ndExceptionFalsePredicate()
        {
            int attempts = 0;
            Action action = () =>
            {
                attempts++;
                throw new AccessViolationException();
            };
            try
            {
                action.Retry(3, 1000, (System.IO.IOException ex) => false, (AccessViolationException ex) => false);
            }
            catch
            {
                Assert.AreEqual(1, attempts);
                throw;
            }
            Assert.Fail();
        }

        [Test]
        [ExpectedException(typeof(AccessViolationException))]
        public void RetryPredicate_2Exceptions_Raise2ndException_FalsePredicate2ndRetry()
        {
            int attempts = 0;
            var Retry = true;
            Action action = () =>
            {
                attempts++;
                throw new AccessViolationException();
            };
            try
            {
                action.Retry(3, 1000, (System.IO.IOException ex) => false, (AccessViolationException ex) =>
                {
                    var result = Retry;
                    Retry = false;
                    return result;
                });
            }
            catch
            {
                Assert.AreEqual(2, attempts);
                throw;
            }
            Assert.Fail();
        }

        [Test]
        [ExpectedException(typeof(Exception))]
        public void RetryPredicate_2Exceptions_RaiseUnexpectedException()
        {
            int attempts = 0;
            Action action = () =>
            {
                attempts++;
                throw new Exception();
            };
            try
            {
                action.Retry(3, 1000, (System.IO.IOException ex) => true, (AccessViolationException ex) => true);
            }
            catch
            {
                Assert.AreEqual(1, attempts);
                throw;
            }
            Assert.Fail();
        }

        [Test]
        [ExpectedException(typeof(Exception))]
        public void RetryPredicate_2Exceptions_RaiseUnexpectedExceptionFalsePredicate()
        {
            int attempts = 0;
            Action action = () =>
            {
                attempts++;
                throw new Exception();
            };
            try
            {
                action.Retry(3, 1000, (System.IO.IOException ex) => false, (AccessViolationException ex) => false);
            }
            catch
            {
                Assert.AreEqual(1, attempts);
                throw;
            }
            Assert.Fail();
        }

        #endregion predicates
        #endregion

        #region 3 exceptions
        [Test]
        public void Retry1Parameter_3Exceptions_Successful()
        {
            int result = 0;
            Action<int> action = (val1) => { result = val1; };
            action.Retry<int, System.IO.IOException, AccessViolationException, System.IO.EndOfStreamException>(1, 1, 1000);
            Assert.AreEqual(1, result);
        }

        [Test]
        public void Retry2Parameter_3Exceptions_Successful()
        {
            int result = 0;
            Action<int, int> action = (val1, val2) => { result = val1 + val2; };
            action.Retry<int, int, System.IO.IOException, AccessViolationException, System.IO.EndOfStreamException>(1, 2, 1, 1000);
            Assert.AreEqual(3, result);
        }

        [Test]
        public void Retry3Parameter_3Exceptions_Successful()
        {
            int result = 0;
            Action<int, int, int> action = (val1, val2, val3) => { result = val1 + val2 + val3; };
            action.Retry<int, int, int, System.IO.IOException, AccessViolationException, System.IO.EndOfStreamException>(1, 2, 3, 1, 1000);
            Assert.AreEqual(6, result);
        }

        [Test]
        [ExpectedException(typeof(System.IO.IOException))]
        public void Retry3Parameter_3Exceptions_RaiseException1stExcpetionAfterMaxAttempts()
        {
            int attempts = 0;
            Action<int, int, int> action = (val1, val2, val3) => { attempts++; throw new System.IO.IOException(); };
            try
            {
                action.Retry<int, int, int, System.IO.IOException, AccessViolationException, System.IO.EndOfStreamException>(1, 2, 3, 3, 1000);
            }
            catch
            {
                Assert.AreEqual(3, attempts);
                throw;
            }
            Assert.Fail();
        }

        [Test]
        [ExpectedException(typeof(AccessViolationException))]
        public void Retry3Parameter_3Exceptions_RaiseException2ndExcpetionAfterMaxAttempts()
        {
            int attempts = 0;
            Action<int, int, int> action = (val1, val2, val3) => { attempts++; throw new AccessViolationException(); };
            try
            {
                action.Retry<int, int, int, System.IO.IOException, AccessViolationException, System.IO.EndOfStreamException>(1, 2, 3, 3, 1000);
            }
            catch
            {
                Assert.AreEqual(3, attempts);
                throw;
            }
            Assert.Fail();
        }

        [Test]
        [ExpectedException(typeof(System.IO.EndOfStreamException))]
        public void Retry3Parameter_3Exceptions_RaiseException3rdExcpetionAfterMaxAttempts()
        {
            int attempts = 0;
            Action<int, int, int> action = (val1, val2, val3) => { attempts++; throw new System.IO.EndOfStreamException(); };
            try
            {
                action.Retry<int, int, int, System.IO.IOException, AccessViolationException, System.IO.EndOfStreamException>(1, 2, 3, 3, 1000);
            }
            catch
            {
                Assert.AreEqual(3, attempts);
                throw;
            }
            Assert.Fail();
        }

        [Test]
        [ExpectedException(typeof(Exception))]
        public void Retry3Parameter_3Exceptions_RaiseUnexpectedException()
        {
            int attempts = 0;
            Action<int, int, int> action = (val1, val2, val3) =>
            {
                attempts++;
                throw new Exception();
            };
            try
            {
                action
                    .Retry
                    <int, int, int, System.IO.IOException, AccessViolationException, System.IO.EndOfStreamException>(1,
                        2, 3, 3, 1000);
            }
            catch
            {
                Assert.AreEqual(1, attempts);
                throw;
            }
            Assert.Fail();
        }

        #region predicates

        [Test]
        public void RetryPredicate_3Exceptions_Successful()
        {
            int result = 0;
            Action action = () => { result = 1; };
            action.Retry(1, 1000, (System.IO.IOException ex) => true, (AccessViolationException ex) => true,
                (System.IO.EndOfStreamException ex) => true);
            Assert.AreEqual(1, result);
        }

        [Test]
        public void RetryPredicate_3Exceptions_FalsePredicate_Successful()
        {
            int result = 0;
            Action action = () => { result = 1; };
            action.Retry(1, 1000, (System.IO.IOException ex) => false, (AccessViolationException ex) => false,
                (System.IO.EndOfStreamException ex) => false);
            Assert.AreEqual(1, result);
        }

        [Test]
        [ExpectedException(typeof(System.IO.IOException))]
        public void RetryPredicate_3Exceptions_Raise1stExceptionAfterMaxAttempts()
        {
            int attempts = 0;
            Action action = () =>
            {
                attempts++;
                throw new System.IO.IOException();
            };
            try
            {
                action.Retry(3, 1000, (System.IO.IOException ex) => true, (AccessViolationException ex) => true,
                    (System.IO.EndOfStreamException ex) => true);
            }
            catch
            {
                Assert.AreEqual(3, attempts);
                throw;
            }
            Assert.Fail();
        }

        [Test]
        [ExpectedException(typeof(System.IO.IOException))]
        public void RetryPredicate_3Exceptions_Raise1stExceptionFalsePredicate()
        {
            int attempts = 0;
            Action action = () =>
            {
                attempts++;
                throw new System.IO.IOException();
            };
            try
            {
                action.Retry(3, 1000, (System.IO.IOException ex) => false, (AccessViolationException ex) => false,
                    (System.IO.EndOfStreamException ex) => false);
            }
            catch
            {
                Assert.AreEqual(1, attempts);
                throw;
            }
            Assert.Fail();
        }

        [Test]
        [ExpectedException(typeof(System.IO.IOException))]
        public void RetryPredicate_3Exceptions_Raise1stException_FalsePredicate2ndRetry()
        {
            int attempts = 0;
            var Retry = true;
            Action action = () =>
            {
                attempts++;
                throw new System.IO.IOException();
            };
            try
            {
                action.Retry(3, 1000, (System.IO.IOException ex) =>
                {
                    var result = Retry;
                    Retry = false;
                    return result;
                }, (AccessViolationException ex) => false,
                    (System.IO.EndOfStreamException ex) => false);
            }
            catch
            {
                Assert.AreEqual(2, attempts);
                throw;
            }
            Assert.Fail();
        }

        [Test]
        [ExpectedException(typeof(AccessViolationException))]
        public void RetryPredicate_3Exceptions_Raise2ndExceptionAfterMaxAttempts()
        {
            int attempts = 0;
            Action action = () =>
            {
                attempts++;
                throw new AccessViolationException();
            };
            try
            {
                action.Retry(3, 1000, (System.IO.IOException ex) => true, (AccessViolationException ex) => true,
                    (System.IO.EndOfStreamException ex) => true);
            }
            catch
            {
                Assert.AreEqual(3, attempts);
                throw;
            }
            Assert.Fail();
        }

        [Test]
        [ExpectedException(typeof(AccessViolationException))]
        public void RetryPredicate_3Exceptions_Raise2ndExceptionFalsePredicate()
        {
            int attempts = 0;
            Action action = () =>
            {
                attempts++;
                throw new AccessViolationException();
            };
            try
            {
                action.Retry(3, 1000, (System.IO.IOException ex) => false, (AccessViolationException ex) => false,
                    (System.IO.EndOfStreamException ex) => false);
            }
            catch
            {
                Assert.AreEqual(1, attempts);
                throw;
            }
            Assert.Fail();
        }

        [Test]
        [ExpectedException(typeof(AccessViolationException))]
        public void RetryPredicate_3Exceptions_Raise2ndException_FalsePredicate2ndRetry()
        {
            int attempts = 0;
            var Retry = true;
            Action action = () =>
            {
                attempts++;
                throw new AccessViolationException();
            };
            try
            {
                action.Retry(3, 1000, (System.IO.IOException ex) => false, (AccessViolationException ex) =>
                {
                    var result = Retry;
                    Retry = false;
                    return result;
                },
                    (System.IO.EndOfStreamException ex) => false);
            }
            catch
            {
                Assert.AreEqual(2, attempts);
                throw;
            }
            Assert.Fail();
        }

        [Test]
        [ExpectedException(typeof(System.IO.EndOfStreamException))]
        public void RetryPredicate_3Exceptions_Raise3rdExceptionAfterMaxAttempts()
        {
            int attempts = 0;
            Action action = () =>
            {
                attempts++;
                throw new System.IO.EndOfStreamException();
            };
            try
            {
                action.Retry(3, 1000, (System.IO.IOException ex) => true, (AccessViolationException ex) => true,
                    (System.IO.EndOfStreamException ex) => true);
            }
            catch
            {
                Assert.AreEqual(3, attempts);
                throw;
            }
            Assert.Fail();
        }

        [Test]
        [ExpectedException(typeof(System.IO.EndOfStreamException))]
        public void RetryPredicate_3Exceptions_Raise3rdExceptionFalsePredicate()
        {
            int attempts = 0;
            Action action = () =>
            {
                attempts++;
                throw new System.IO.EndOfStreamException();
            };
            try
            {
                action.Retry(3, 1000, (System.IO.IOException ex) => false, (AccessViolationException ex) => false,
                    (System.IO.EndOfStreamException ex) => false);
            }
            catch
            {
                Assert.AreEqual(1, attempts);
                throw;
            }
            Assert.Fail();
        }

        [Test]
        [ExpectedException(typeof (System.IO.EndOfStreamException))]
        public void RetryPredicate_3Exceptions_Raise3rdException_FalsePredicate2ndRetry()
        {
            int attempts = 0;
            var Retry = true;
            Action action = () =>
            {
                attempts++;
                throw new System.IO.EndOfStreamException();
            };
            try
            {
                action.Retry(3, 1000, (DivideByZeroException ex) => false, (AccessViolationException ex) => false,
                    (System.IO.EndOfStreamException ex) =>
                    {
                        var result = Retry;
                        Retry = false;
                        return result;
                    });
            }
            catch
            {
                Assert.AreEqual(2, attempts);
                throw;
            }
            Assert.Fail();
        }

        [Test]
        [ExpectedException(typeof(Exception))]
        public void RetryPredicate_3Exceptions_RaiseUnexpectedException()
        {
            int attempts = 0;
            Action action = () =>
            {
                attempts++;
                throw new Exception();
            };
            try
            {
                action.Retry(3, 1000, (System.IO.IOException ex) => true, (AccessViolationException ex) => true,
                    (System.IO.EndOfStreamException ex) => true);
            }
            catch
            {
                Assert.AreEqual(1, attempts);
                throw;
            }
            Assert.Fail();
        }

        [Test]
        [ExpectedException(typeof(Exception))]
        public void RetryPredicate_3Exceptions_RaiseUnexpectedExceptionFalsePredicate()
        {
            int attempts = 0;
            Action action = () =>
            {
                attempts++;
                throw new Exception();
            };
            try
            {
                action.Retry(3, 1000, (System.IO.IOException ex) => false, (AccessViolationException ex) => false,
                    (System.IO.EndOfStreamException ex) => false);
            }
            catch
            {
                Assert.AreEqual(1, attempts);
                throw;
            }
            Assert.Fail();
        }
        #endregion predicates
        #endregion
    }
}