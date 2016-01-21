using System;
using System.IO;
using NUnit.Framework;
using Rock.Extensions;

namespace Rock.Core.UnitTests.Extensions
{
	[TestFixture]
	public class FuncExtensionsTest
	{
		private int actionCalledCount;

		[SetUp]
		public void TestInitialize()
		{
			actionCalledCount = 0;
		}

		#region result with three exceptions
		#region TResult Retry<T1, TResult, TException1, TException2, TException3>(this Func<T1, TResult> func,T1 parameter1,int numberOfTries,int millisecondsSleepBetweenRetry)
		[Test]
		public void RetryWithResultAndOneParameterThreeExceptionsSucceeds()
		{
			var callCount = 0;
			Func<int, int> func = p1 =>
			{
				callCount++;
				return 99;
			};

			int result = func.Retry<int, int, IOException, AccessViolationException, DivideByZeroException>(42, 3, 0);

			Assert.AreEqual(99, result);
			Assert.AreEqual(1, callCount);
		}

		[Test]
		[ExpectedException(typeof(Exception))]
		public void RetryWithResultAndOneParameterThreeExceptionsWithFourthExceptionThrows()
		{
			Func<int, int> func = p1 =>
			{
				throw new Exception();
			};

			int result = func.Retry<int, int, IOException, AccessViolationException, DivideByZeroException>(42, 3, 0);

			Assert.Fail();
		}

		[Test]
		[ExpectedException(typeof(DivideByZeroException))]
		public void RetryWithResultAndOneParameterThreeExceptionsThreeRetriesThrows()
		{
			var callCount = 0;
			Func<int, int> func = p1 =>
			{
				callCount++;
				switch (callCount)
				{
					case 1:
						throw new IOException();
					case 2:
						throw new AccessViolationException();
					default:
						throw new DivideByZeroException();
				}
			};

			int result = func.Retry<int, int, IOException, AccessViolationException, DivideByZeroException>(42, 3, 0);

			Assert.Fail();
		}

		[Test]
		[ExpectedException(typeof(AccessViolationException))]
		public void RetryWithResultAndOneParameterThreeExceptionsTwoRetriesThrows()
		{
			var callCount = 0;
			Func<int, int> func = p1 =>
			{
				callCount++;
				switch (callCount)
				{
					case 1:
						throw new IOException();
					default:
						throw new AccessViolationException();
				}
			};

			int result = func.Retry<int, int, IOException, AccessViolationException, DivideByZeroException>(42, 2, 0);

			Assert.Fail();
		}

		[Test]
		[ExpectedException(typeof(IOException))]
		public void RetryWithResultAndOneParameterThreeExceptionsOneRetriesThrows()
		{
			Func<int, int> func = p1 =>
			{
				throw new IOException();
			};

			int result = func.Retry<int, int, IOException, AccessViolationException, DivideByZeroException>(42, 1, 0);

			Assert.Fail();
		}
		#endregion
		#region TResult Retry<T1, T2, TResult, TException1, TException2, TException3>(this Func<T1, T2, TResult> func,T1 parameter1,T2 parameter2,int numberOfTries,int millisecondsSleepBetweenRetry)
		[Test]
		public void RetryWithResultAndTwoParametersThreeExceptionsSucceeds()
		{
			var callCount = 0;
			Func<int, int, int> func = (p1, p2) =>
			{
				callCount++;
				return 99;
			};

			int result = func.Retry<int, int, int, IOException, AccessViolationException, DivideByZeroException>(42, 72, 3, 0);

			Assert.AreEqual(99, result);
			Assert.AreEqual(1, callCount);
		}

		[Test]
		[ExpectedException(typeof(Exception))]
		public void RetryWithResultAndTwoParametersThreeExceptionsWithFourthExceptionThrows()
		{
			Func<int, int, int> func = (p1, p2) =>
			{
				throw new Exception();
			};

			int result = func.Retry<int, int, int, IOException, AccessViolationException, DivideByZeroException>(42, 72, 3, 0);

			Assert.Fail();
		}

		[Test]
		[ExpectedException(typeof(DivideByZeroException))]
		public void RetryWithResultAndTwoParametersThreeExceptionsThreeRetriesThrows()
		{
			var callCount = 0;
			Func<int, int, int> func = (p1, p2) =>
			{
				callCount++;
				switch (callCount)
				{
					case 1:
						throw new IOException();
					case 2:
						throw new AccessViolationException();
					default:
						throw new DivideByZeroException();
				}
			};

			int result = func.Retry<int, int, int, IOException, AccessViolationException, DivideByZeroException>(42, 72, 3, 0);

			Assert.Fail();
		}

		[Test]
		[ExpectedException(typeof(AccessViolationException))]
		public void RetryWithResultAndTwoParametersThreeExceptionsTwoRetriesThrows()
		{
			var callCount = 0;
			Func<int, int, int> func = (p1, p2) =>
			{
				callCount++;
				switch (callCount)
				{
					case 1:
						throw new IOException();
					default:
						throw new AccessViolationException();
				}
			};

			int result = func.Retry<int, int, int, IOException, AccessViolationException, DivideByZeroException>(42, 72, 2, 0);

			Assert.Fail();
		}

		[Test]
		[ExpectedException(typeof(IOException))]
		public void RetryWithResultAndTwoParametersThreeExceptionsOneRetriesThrows()
		{
			Func<int, int, int> func = (p1, p2) =>
			{
				throw new IOException();
			};

			int result = func.Retry<int, int, int, IOException, AccessViolationException, DivideByZeroException>(42, 72, 1, 0);

			Assert.Fail();
		}
		#endregion
		#region TResult Retry<T1, T2, T3, TResult, TException1, TException2, TException3>(this Func<T1, T2, T3, TResult> func,T1 parameter1,T2 parameter2, T3 parameter3, int numberOfTries,int millisecondsSleepBetweenRetry)
		[Test]
		public void RetryWithResultAndThreeParametersThreeExceptionsSucceeds()
		{
			var callCount = 0;
			Func<int, int, int, int> func = (p1, p2, p3) =>
			{
				callCount++;
				return 99;
			};

			int result = func.Retry<int, int, int, int, IOException, AccessViolationException, DivideByZeroException>(42, 72, 1812, 3, 0);

			Assert.AreEqual(99, result);
			Assert.AreEqual(1, callCount);
		}

		[Test]
		[ExpectedException(typeof(Exception))]
		public void RetryWithResultAndThreeParametersThreeExceptionsWithFourthExceptionThrows()
		{
			Func<int, int, int, int> func = (p1, p2, p3) =>
			{
				throw new Exception();
			};

			int result = func.Retry<int, int, int, int, IOException, AccessViolationException, DivideByZeroException>(42, 72, 1812, 3, 0);

			Assert.Fail();
		}

		[Test]
		[ExpectedException(typeof(DivideByZeroException))]
		public void RetryWithResultAndThreeParametersThreeExceptionsThreeRetriesThrows()
		{
			var callCount = 0;
			Func<int, int, int, int> func = (p1, p2, p3) =>
			{
				callCount++;
				switch (callCount)
				{
					case 1:
						throw new IOException();
					case 2:
						throw new AccessViolationException();
					default:
						throw new DivideByZeroException();
				}
			};

			int result = func.Retry<int, int, int, int, IOException, AccessViolationException, DivideByZeroException>(42, 72, 1812, 3, 0);

			Assert.Fail();
		}

		[Test]
		[ExpectedException(typeof(AccessViolationException))]
		public void RetryWithResultAndThreeParametersThreeExceptionsTwoRetriesThrows()
		{
			var callCount = 0;
			Func<int, int, int, int> func = (p1, p2, p3) =>
			{
				callCount++;
				switch (callCount)
				{
					case 1:
						throw new IOException();
					default:
						throw new AccessViolationException();
				}
			};

			int result = func.Retry<int, int, int, int, IOException, AccessViolationException, DivideByZeroException>(42, 72, 1812, 2, 0);

			Assert.Fail();
		}

		[Test]
		[ExpectedException(typeof(IOException))]
		public void RetryWithResultAndThreeParametersThreeExceptionsOneRetriesThrows()
		{
			Func<int, int, int, int> func = (p1, p2, p3) =>
			{
				throw new IOException();
			};

			int result = func.Retry<int, int, int, int, IOException, AccessViolationException, DivideByZeroException>(42, 72, 1812, 1, 0);

			Assert.Fail();
		}
		#endregion
		#endregion
		#region result with two exceptions
		#region TResult Retry<T1, TResult, TException1, TException2>(this Func<T1, TResult> func,T1 parameter1,int numberOfTries,int millisecondsSleepBetweenRetry)
		[Test]
		public void RetryWithResultAndOneParameterTwoExceptionsSucceeds()
		{
			var callCount = 0;
			Func<int, int> func = p1 =>
			{
				callCount++;
				return 99;
			};

			int result = func.Retry<int, int, IOException, AccessViolationException>(42, 3, 0);

			Assert.AreEqual(99, result);
			Assert.AreEqual(1, callCount);
		}

		[Test]
		[ExpectedException(typeof(Exception))]
		public void RetryWithResultAndOneParameterTwoExceptionsWithFourthExceptionThrows()
		{
			Func<int, int> func = p1 =>
			{
				throw new Exception();
			};

			int result = func.Retry<int, int, IOException, AccessViolationException>(42, 3, 0);

			Assert.Fail();
		}

		[Test]
		[ExpectedException(typeof(DivideByZeroException))]
		public void RetryWithResultAndOneParameterTwoExceptionsThreeRetriesThrows()
		{
			var callCount = 0;
			Func<int, int> func = p1 =>
			{
				callCount++;
				switch (callCount)
				{
					case 1:
						throw new IOException();
					case 2:
						throw new AccessViolationException();
					default:
						throw new DivideByZeroException();
				}
			};

			int result = func.Retry<int, int, IOException, AccessViolationException>(42, 3, 0);

			Assert.Fail();
		}

		[Test]
		[ExpectedException(typeof(AccessViolationException))]
		public void RetryWithResultAndOneParameterTwoExceptionsTwoRetriesThrows()
		{
			var callCount = 0;
			Func<int, int> func = p1 =>
			{
				callCount++;
				switch (callCount)
				{
					case 1:
						throw new IOException();
					default:
						throw new AccessViolationException();
				}
			};

			int result = func.Retry<int, int, IOException, AccessViolationException>(42, 2, 0);

			Assert.Fail();
		}

		[Test]
		[ExpectedException(typeof(IOException))]
		public void RetryWithResultAndOneParameterTwoExceptionsOneRetriesThrows()
		{
			Func<int, int> func = p1 =>
			{
				throw new IOException();
			};

			int result = func.Retry<int, int, IOException, AccessViolationException>(42, 1, 0);

			Assert.Fail();
		}
		#endregion
		#region TResult Retry<T1, T2, TResult, TException1, TException2>(this Func<T1, T2, TResult> func,T1 parameter1,T2 parameter2,int numberOfTries,int millisecondsSleepBetweenRetry)
		[Test]
		public void RetryWithResultAndTwoParametersTwoExceptionsSucceeds()
		{
			var callCount = 0;
			Func<int, int, int> func = (p1, p2) =>
			{
				callCount++;
				return 99;
			};

			int result = func.Retry<int, int, int, IOException, AccessViolationException>(42, 72, 3, 0);

			Assert.AreEqual(99, result);
			Assert.AreEqual(1, callCount);
		}

		[Test]
		[ExpectedException(typeof(Exception))]
		public void RetryWithResultAndTwoParametersTwoExceptionsWithFourthExceptionThrows()
		{
			Func<int, int, int> func = (p1, p2) =>
			{
				throw new Exception();
			};

			int result = func.Retry<int, int, int, IOException, AccessViolationException>(42, 72, 3, 0);

			Assert.Fail();
		}

		[Test]
		[ExpectedException(typeof(DivideByZeroException))]
		public void RetryWithResultAndTwoParametersTwoExceptionsThreeRetriesThrows()
		{
			var callCount = 0;
			Func<int, int, int> func = (p1, p2) =>
			{
				callCount++;
				switch (callCount)
				{
					case 1:
						throw new IOException();
					case 2:
						throw new AccessViolationException();
					default:
						throw new DivideByZeroException();
				}
			};

			int result = func.Retry<int, int, int, IOException, AccessViolationException>(42, 72, 3, 0);

			Assert.Fail();
		}

		[Test]
		[ExpectedException(typeof(AccessViolationException))]
		public void RetryWithResultAndTwoParametersTwoExceptionsTwoRetriesThrows()
		{
			var callCount = 0;
			Func<int, int, int> func = (p1, p2) =>
			{
				callCount++;
				switch (callCount)
				{
					case 1:
						throw new IOException();
					default:
						throw new AccessViolationException();
				}
			};

			int result = func.Retry<int, int, int, IOException, AccessViolationException>(42, 72, 2, 0);

			Assert.Fail();
		}

		[Test]
		[ExpectedException(typeof(IOException))]
		public void RetryWithResultAndTwoParametersTwoExceptionsOneRetriesThrows()
		{
			Func<int, int, int> func = (p1, p2) =>
			{
				throw new IOException();
			};

			int result = func.Retry<int, int, int, IOException, AccessViolationException>(42, 72, 1, 0);

			Assert.Fail();
		}
		#endregion
		#region TResult Retry<T1, T2, T3, TResult, TException1, TException2>(this Func<T1, T2, T3, TResult> func,T1 parameter1,T2 parameter2, T3 parameter3, int numberOfTries,int millisecondsSleepBetweenRetry)
		[Test]
		public void RetryWithResultAndThreeParametersTwoExceptionsSucceeds()
		{
			var callCount = 0;
			Func<int, int, int, int> func = (p1, p2, p3) =>
			{
				callCount++;
				return 99;
			};

			int result = func.Retry<int, int, int, int, IOException, AccessViolationException>(42, 72, 1812, 3, 0);

			Assert.AreEqual(99, result);
			Assert.AreEqual(1, callCount);
		}

		[Test]
		[ExpectedException(typeof(Exception))]
		public void RetryWithResultAndThreeParametersTwoExceptionsWithFourthExceptionThrows()
		{
			Func<int, int, int, int> func = (p1, p2, p3) =>
			{
				throw new Exception();
			};

			int result = func.Retry<int, int, int, int, IOException, AccessViolationException>(42, 72, 1812, 3, 0);

			Assert.Fail();
		}

		[Test]
		[ExpectedException(typeof(DivideByZeroException))]
		public void RetryWithResultAndThreeParametersTwoExceptionsThreeRetriesThrows()
		{
			var callCount = 0;
			Func<int, int, int, int> func = (p1, p2, p3) =>
			{
				callCount++;
				switch (callCount)
				{
					case 1:
						throw new IOException();
					case 2:
						throw new AccessViolationException();
					default:
						throw new DivideByZeroException();
				}
			};

			int result = func.Retry<int, int, int, int, IOException, AccessViolationException>(42, 72, 1812, 3, 0);

			Assert.Fail();
		}

		[Test]
		[ExpectedException(typeof(AccessViolationException))]
		public void RetryWithResultAndThreeParametersTwoExceptionsTwoRetriesThrows()
		{
			var callCount = 0;
			Func<int, int, int, int> func = (p1, p2, p3) =>
			{
				callCount++;
				switch (callCount)
				{
					case 1:
						throw new IOException();
					default:
						throw new AccessViolationException();
				}
			};

			int result = func.Retry<int, int, int, int, IOException, AccessViolationException>(42, 72, 1812, 2, 0);

			Assert.Fail();
		}

		[Test]
		[ExpectedException(typeof(IOException))]
		public void RetryWithResultAndThreeParametersTwoExceptionsOneRetriesThrows()
		{
			Func<int, int, int, int> func = (p1, p2, p3) =>
			{
				throw new IOException();
			};

			int result = func.Retry<int, int, int, int, IOException, AccessViolationException>(42, 72, 1812, 1, 0);

			Assert.Fail();
		}
		#endregion
		#endregion
		#region result with one exception
		#region TResult Retry<T1, TResult, TException1, TException2>(this Func<T1, TResult> func,T1 parameter1,int numberOfTries,int millisecondsSleepBetweenRetry)
		[Test]
		public void RetryWithResultAndOneParameterOneExceptionSucceeds()
		{
			var callCount = 0;
			Func<int, int> func = p1 =>
			{
				callCount++;
				return 99;
			};

			int result = func.Retry<int, int, IOException>(42, 3, 0);

			Assert.AreEqual(99, result);
			Assert.AreEqual(1, callCount);
		}

		[Test]
		[ExpectedException(typeof(Exception))]
		public void RetryWithResultAndOneParameterOneExceptionWithFourthExceptionThrows()
		{
			Func<int, int> func = p1 =>
			{
				throw new Exception();
			};

			int result = func.Retry<int, int, IOException>(42, 3, 0);

			Assert.Fail();
		}

		[Test]
		[ExpectedException(typeof(DivideByZeroException))]
		public void RetryWithResultAndOneParameterOneExceptionThreeRetriesThrows()
		{
			var callCount = 0;
			Func<int, int> func = p1 =>
			{
				callCount++;
				switch (callCount)
				{
					case 1:
						throw new IOException();
					case 2:
						throw new AccessViolationException();
					default:
						throw new DivideByZeroException();
				}
			};

			int result = func.Retry<int, int, IOException>(42, 3, 0);

			Assert.Fail();
		}

		[Test]
		[ExpectedException(typeof(AccessViolationException))]
		public void RetryWithResultAndOneParameterOneExceptionTwoRetriesThrows()
		{
			var callCount = 0;
			Func<int, int> func = p1 =>
			{
				callCount++;
				switch (callCount)
				{
					case 1:
						throw new IOException();
					default:
						throw new AccessViolationException();
				}
			};

			int result = func.Retry<int, int, IOException>(42, 2, 0);

			Assert.Fail();
		}

		[Test]
		[ExpectedException(typeof(IOException))]
		public void RetryWithResultAndOneParameterOneExceptionOneRetriesThrows()
		{
			Func<int, int> func = p1 =>
			{
				throw new IOException();
			};

			int result = func.Retry<int, int, IOException>(42, 1, 0);

			Assert.Fail();
		}
		#endregion
		#region TResult Retry<T1, T2, TResult, TException1, TException2>(this Func<T1, T2, TResult> func,T1 parameter1,T2 parameter2,int numberOfTries,int millisecondsSleepBetweenRetry)
		[Test]
		public void RetryWithResultAndTwoParametersOneExceptionSucceeds()
		{
			var callCount = 0;
			Func<int, int, int> func = (p1, p2) =>
			{
				callCount++;
				return 99;
			};

			int result = func.Retry<int, int, int, IOException>(42, 72, 3, 0);

			Assert.AreEqual(99, result);
			Assert.AreEqual(1, callCount);
		}

		[Test]
		[ExpectedException(typeof(Exception))]
		public void RetryWithResultAndTwoParametersOneExceptionWithFourthExceptionThrows()
		{
			Func<int, int, int> func = (p1, p2) =>
			{
				throw new Exception();
			};

			int result = func.Retry<int, int, int, IOException>(42, 72, 3, 0);

			Assert.Fail();
		}

		[Test]
		[ExpectedException(typeof(DivideByZeroException))]
		public void RetryWithResultAndTwoParametersOneExceptionThreeRetriesThrows()
		{
			var callCount = 0;
			Func<int, int, int> func = (p1, p2) =>
			{
				callCount++;
				switch (callCount)
				{
					case 1:
						throw new IOException();
					case 2:
						throw new AccessViolationException();
					default:
						throw new DivideByZeroException();
				}
			};

			int result = func.Retry<int, int, int, IOException>(42, 72, 3, 0);

			Assert.Fail();
		}

		[Test]
		[ExpectedException(typeof(AccessViolationException))]
		public void RetryWithResultAndTwoParametersOneExceptionTwoRetriesThrows()
		{
			var callCount = 0;
			Func<int, int, int> func = (p1, p2) =>
			{
				callCount++;
				switch (callCount)
				{
					case 1:
						throw new IOException();
					default:
						throw new AccessViolationException();
				}
			};

			int result = func.Retry<int, int, int, IOException>(42, 72, 2, 0);

			Assert.Fail();
		}

		[Test]
		[ExpectedException(typeof(IOException))]
		public void RetryWithResultAndTwoParametersOneExceptionOneRetriesThrows()
		{
			Func<int, int, int> func = (p1, p2) =>
			{
				throw new IOException();
			};

			int result = func.Retry<int, int, int, IOException>(42, 72, 1, 0);

			Assert.Fail();
		}
		#endregion
		#region TResult Retry<T1, T2, T3, TResult, TException1, TException2>(this Func<T1, T2, T3, TResult> func,T1 parameter1,T2 parameter2, T3 parameter3, int numberOfTries,int millisecondsSleepBetweenRetry)
		[Test]
		public void RetryWithResultAndThreeParametersOneExceptionSucceeds()
		{
			var callCount = 0;
			Func<int, int, int, int> func = (p1, p2, p3) =>
			{
				callCount++;
				return 99;
			};

			int result = func.Retry<int, int, int, int, IOException>(42, 72, 1812, 3, 0);

			Assert.AreEqual(99, result);
			Assert.AreEqual(1, callCount);
		}

		[Test]
		[ExpectedException(typeof(Exception))]
		public void RetryWithResultAndThreeParametersOneExceptionWithFourthExceptionThrows()
		{
			Func<int, int, int, int> func = (p1, p2, p3) =>
			{
				throw new Exception();
			};

			int result = func.Retry<int, int, int, int, IOException>(42, 72, 1812, 3, 0);

			Assert.Fail();
		}

		[Test]
		[ExpectedException(typeof(DivideByZeroException))]
		public void RetryWithResultAndThreeParametersOneExceptionThreeRetriesThrows()
		{
			var callCount = 0;
			Func<int, int, int, int> func = (p1, p2, p3) =>
			{
				callCount++;
				switch (callCount)
				{
					case 1:
						throw new IOException();
					case 2:
						throw new AccessViolationException();
					default:
						throw new DivideByZeroException();
				}
			};

			int result = func.Retry<int, int, int, int, IOException>(42, 72, 1812, 3, 0);

			Assert.Fail();
		}

		[Test]
		[ExpectedException(typeof(AccessViolationException))]
		public void RetryWithResultAndThreeParametersOneExceptionTwoRetriesThrows()
		{
			var callCount = 0;
			Func<int, int, int, int> func = (p1, p2, p3) =>
			{
				callCount++;
				switch (callCount)
				{
					case 1:
						throw new IOException();
					default:
						throw new AccessViolationException();
				}
			};

			int result = func.Retry<int, int, int, int, IOException>(42, 72, 1812, 2, 0);

			Assert.Fail();
		}

		[Test]
		[ExpectedException(typeof(IOException))]
		public void RetryWithResultAndThreeParametersOneExceptionOneRetriesThrows()
		{
			Func<int, int, int, int> func = (p1, p2, p3) =>
			{
				throw new IOException();
			};

			int result = func.Retry<int, int, int, int, IOException>(42, 72, 1812, 1, 0);

			Assert.Fail();
		}
		#endregion
		#endregion
		[Test]
		public void TwoExpectedFunctionExceptions_Executed_Executes()
		{
			var func = new Func<int>(() => CustomThrower(0));

			int result = func.Retry<int, CustomException, CustomException>(3, 0);

			Assert.AreEqual(42, result);
			Assert.AreEqual(1, actionCalledCount);
		}

		[Test]
		public void ThreeExpectedFunctionExceptions_Predicate_Executed_Executes()
		{
			Func<int> func = () => CustomThrower(0);

			int result = func.Retry<int, CustomException, CustomException, CustomException>(3, 0, e => true, e => true, e => true);

			Assert.AreEqual(42, result);
			Assert.AreEqual(1, actionCalledCount);
		}

		[Test]
		public void TwoExpectedFunctionExceptions_Predicate_Executed_Executes()
		{
			Func<int> func = () => CustomThrower(0);

			int result = func.Retry<int, CustomException, CustomException>(3, 0, e => true, e => true);

			Assert.AreEqual(42, result);
			Assert.AreEqual(1, actionCalledCount);
		}

		[Test]
		public void OneExpectedFunctionExceptions_Predicate_Executed_Executes()
		{
			Func<int> func = () => CustomThrower(0);

			int result = func.Retry<int, CustomException>(3, 0, e => true);

			Assert.AreEqual(42, result);
			Assert.AreEqual(1, actionCalledCount);
		}

		[Test]
		[ExpectedException(typeof(CustomException))]
		public void ThreeExpectedFunctionExceptions_FalsePredicate_Executed_Throws()
		{
			Func<int> func = () => CustomThrower(2);

			int result = func.Retry<int, CustomException, CustomException, CustomException>(3, 0, e => false, e => false, e => false);

			Assert.AreEqual(42, result);
			Assert.AreEqual(1, actionCalledCount);
		}

		[Test]
		[ExpectedException(typeof(CustomException))]
		public void TwoExpectedFunctionExceptions_FalsePredicate_Executed_Throws()
		{
			Func<int> func = () => CustomThrower(2);

			int result = func.Retry<int, CustomException, CustomException>(3, 0, e => false, e => false);

			Assert.AreEqual(42, result);
			Assert.AreEqual(1, actionCalledCount);
		}

		[Test]
		[ExpectedException(typeof(CustomException))]
		public void OneExpectedFunctionExceptions_FalsePredicate_Executed_Throws()
		{
			Func<int> func = () => CustomThrower(2);

			int result = func.Retry<int, CustomException>(3, 0, e => false);

			Assert.AreEqual(42, result);
			Assert.AreEqual(1, actionCalledCount);
		}

		[Test]
		public void Function_Executed_ResultReturned()
		{
			Func<int> func = () => CustomThrower(0);

			int result = func.Retry<int, CustomException>(1, 0);

			Assert.AreEqual(42, result);
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void NullFunctionArgument_Executed_ArgumentNullExceptionThrown()
		{

			Func<int> func = null;

			func.Retry<int, CustomException>(3, 3);

			Assert.Fail();
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
	}
}