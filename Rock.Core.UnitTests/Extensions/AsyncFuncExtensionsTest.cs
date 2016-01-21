using System;
using System.IO;
using System.Threading.Tasks;
using NUnit.Framework;
using Rock.Extensions;

namespace Rock.Core.UnitTests.Extensions
{
	[TestFixture]
	public class AsyncFuncExtensionsTest
	{
		[Test, ExpectedException(typeof(AccessViolationException))]
		public async Task AsyncRetryWithReturnValueAndThreeExceptions2Retries()
		{
			var retryCount = 2;
			var millisecondRetryDelay = 20;
			int callCount = 0;
			Func<Task<int>> func = () =>
			{
				callCount++;
				switch (callCount)
				{
					case 1:
						throw new IOException();
				}
				throw new AccessViolationException();
			};
			// ReSharper disable once UnusedVariable
			var result = await
				func.RetryAsync<IOException, AccessViolationException, DivideByZeroException, int>(retryCount,
					millisecondRetryDelay);
		}

		[Test, ExpectedException(typeof(IOException))]
		public async Task AsyncRetryWithReturnValueAndThreeExceptions1Retries()
		{
			var retryCount = 1;
			var millisecondRetryDelay = 20;
			Func<Task<int>> func = () =>
			{
				throw new IOException();
			};
			// ReSharper disable once UnusedVariable
			var result = await
				func.RetryAsync<IOException, AccessViolationException, DivideByZeroException, int>(retryCount,
					millisecondRetryDelay);
		}

		[Test]
		public async Task AsyncRetryWithReturnValueSucceedsWithExceptionsOfOneType()
		{
			var retryCount = 3;
			var millisecondRetryDelay = 20;
			int callCount = 0;
			var stopwatch = System.Diagnostics.Stopwatch.StartNew();
			Func<Task<int>> func = () =>
			{
				callCount++;
				if (callCount < retryCount) throw new Exception();
				return Task.FromResult(42);
			};
			var result = await func.RetryAsync(retryCount, millisecondRetryDelay);
			stopwatch.Stop();
			Assert.AreEqual(retryCount, callCount);
			Assert.IsTrue(stopwatch.ElapsedMilliseconds >= ((millisecondRetryDelay-1) * (callCount - 1)));
			Assert.AreEqual(42, result);
			Console.WriteLine(stopwatch.Elapsed.ToEnglishString());
		}

		[Test, ExpectedException(typeof (Exception))]
		public async Task AsyncRetryWithReturnValueWith1RetryThrowsException()
		{
			var retryCount = 1;
			var millisecondRetryDelay = 20;

			Func<Task<int>> func = () =>
			{
				throw new Exception();
			};
			// ReSharper disable once UnusedVariable
			var result = await func.RetryAsync(retryCount, millisecondRetryDelay);
			Assert.Fail();
		}


		[Test, ExpectedException(typeof(Exception))]
		public async Task AsyncRetryWithReturnValueThrowsWithSomeExceptionsOfUnexpectedType()
		{
			var retryCount = 3;
			var millisecondRetryDelay = 20;
			int callCount = 0;
			Func<Task<int>> func = () =>
			{
				callCount++;
				switch (callCount)
				{
					case 1:
						throw new IOException();
					case 2:
						throw new AccessViolationException();
					default:
						throw new Exception();
				}
			};
			// ReSharper disable once UnusedVariable
			var result =
				await
					func.RetryAsync<IOException, AccessViolationException, int>(retryCount, millisecondRetryDelay);
		}

		[Test, ExpectedException(typeof(AccessViolationException))]
		public async Task AsyncRetryWithReturnValueWithOneExceptionThrowsOfUnexpectedType()
		{
			var retryCount = 3;
			var millisecondRetryDelay = 20;
			int callCount = 0;
			Func<Task<int>> func = () =>
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
			// ReSharper disable once UnusedVariable
			var result = await func.RetryAsync<IOException, int>(retryCount, millisecondRetryDelay);
		}

		[Test, ExpectedException(typeof(DivideByZeroException))]
		public async Task AsyncRetryWithReturnValueWithThreeExceptionsThrowsOfUnexpectedType()
		{
			var retryCount = 3;
			var millisecondRetryDelay = 20;
			int callCount = 0;
			Func<Task<int>> func = () =>
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
			// ReSharper disable once UnusedVariable
			var result =
				await
					func.RetryAsync<IOException, AccessViolationException, DivideByZeroException, int>(retryCount,
						millisecondRetryDelay);
		}

		[Test]
		public async Task AsyncRetryWithReturnValueWithOneArgumentSucceeds()
		{
			var retryCount = 4;
			var millisecondRetryDelay = 20;
			int callCount = 0;
			Func<int, Task<int>> func = a =>
			{
				callCount++;
				return Task.FromResult(a);
			};
			// no timing because with no exceptions there would be no delays
			var result = await func.RetryAsync(72, retryCount, millisecondRetryDelay);
			Assert.AreEqual(1, callCount);
			Assert.AreEqual(72, result);
		}

		[Test, ExpectedException(typeof (IOException))]
		public async Task AsyncRetryWithReturnValueWithOneArgumentAndExceptionThrows()
		{
			var retryCount = 4;
			var millisecondRetryDelay = 20;
			Func<int, Task<int>> func = a =>
			{
				throw new IOException();
			};
			// no timing because with exceptions there would be no delays
			// ReSharper disable once UnusedVariable
			var result = await func.RetryAsync(72, retryCount, millisecondRetryDelay);
			Assert.Fail();
		}

		[Test]
		public async Task AsyncRetryWithReturnValueWithOneArgumentWithThreeExceptionsSucceeds()
		{
			var retryCount = 4;
			var millisecondRetryDelay = 20;
			int callCount = 0;
			Func<int, Task<int>> func = a =>
			{
				callCount++;
				switch (callCount)
				{
					case 1:
						throw new IOException();
					case 2:
						throw new AccessViolationException();
					case 3:
						throw new DivideByZeroException();
				}
				return Task.FromResult(a);
			};
			// no timing because with exceptions there would be no delays
			// ReSharper disable once UnusedVariable
			var result = await func.RetryAsync<int, IOException, AccessViolationException, DivideByZeroException, int>(72, retryCount, millisecondRetryDelay);
			Assert.AreEqual(retryCount, callCount);
			Assert.AreEqual(72, result);
		}

		[Test, ExpectedException(typeof(Exception))]
		public async Task AsyncRetryWithReturnValueWithOneArgumentWithThreeExceptionsThrows()
		{
			var retryCount = 4;
			var millisecondRetryDelay = 20;
			int callCount = 0;
			Func<int, Task<int>> func = a =>
			{
				callCount++;
				switch (callCount)
				{
					case 1:
						throw new IOException();
					case 2:
						throw new AccessViolationException();
					case 3:
						throw new DivideByZeroException();
				}
				throw new Exception();
			};
			// no timing because with exceptions there would be no delays
			// ReSharper disable once UnusedVariable
			var result = await func.RetryAsync<int, IOException, AccessViolationException, DivideByZeroException, int>(72, retryCount, millisecondRetryDelay);
			Assert.Fail();
		}

		[Test, ExpectedException(typeof(IOException))]
		public async Task AsyncRetryWithReturnValueWithOneArgumentWithThreeExceptionsThrowsFirstException()
		{
			var retryCount = 1;
			var millisecondRetryDelay = 20;
			Func<int, Task<int>> func = a =>
			{
				throw new IOException();
			};
			// no timing because with exceptions there would be no delays
			// ReSharper disable once UnusedVariable
			var result = await func.RetryAsync<int, IOException, AccessViolationException, DivideByZeroException, int>(72, retryCount, millisecondRetryDelay);
			Assert.Fail();
		}

		[Test, ExpectedException(typeof(AccessViolationException))]
		public async Task AsyncRetryWithReturnValueWithOneArgumentWithThreeExceptionsThrowsSecondException()
		{
			var retryCount = 2;
			var millisecondRetryDelay = 20;
			int callCount = 0;
			Func<int, Task<int>> func = a =>
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
			// no timing because with exceptions there would be no delays
			// ReSharper disable once UnusedVariable
			var result = await func.RetryAsync<int, IOException, AccessViolationException, DivideByZeroException, int>(72, retryCount, millisecondRetryDelay);
			Assert.Fail();
		}

		[Test, ExpectedException(typeof(DivideByZeroException))]
		public async Task AsyncRetryWithReturnValueWithOneArgumentWithThreeExceptionsThrowsThirdException()
		{
			var retryCount = 3;
			var millisecondRetryDelay = 20;
			int callCount = 0;
			Func<int, Task<int>> func = a =>
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
				throw new Exception();
			};
			// no timing because with exceptions there would be no delays
			// ReSharper disable once UnusedVariable
			var result = await func.RetryAsync<int, IOException, AccessViolationException, DivideByZeroException, int>(72, retryCount, millisecondRetryDelay);
			Assert.Fail();
		}

		[Test]
		public async Task AsyncRetryWithReturnValueWithOneArgumentWithTwoExceptionsSucceeds()
		{
			var retryCount = 3;
			var millisecondRetryDelay = 20;
			int callCount = 0;
			Func<int, Task<int>> func = a =>
			{
				callCount++;
				switch (callCount)
				{
					case 1:
						throw new IOException();
					case 2:
						throw new AccessViolationException();
				}
				return Task.FromResult(a);
			};
			// no timing because with exceptions there would be no delays
			// ReSharper disable once UnusedVariable
			var result = await func.RetryAsync<int, IOException, AccessViolationException, int>(72, retryCount, millisecondRetryDelay);
			Assert.AreEqual(retryCount, callCount);
			Assert.AreEqual(72, result);
		}

		[Test, ExpectedException(typeof(Exception))]
		public async Task AsyncRetryWithReturnValueWithOneArgumentWithTwoExceptionsThrows()
		{
			var retryCount = 3;
			var millisecondRetryDelay = 20;
			int callCount = 0;
			Func<int, Task<int>> func = a =>
			{
				callCount++;
				switch (callCount)
				{
					case 1:
						throw new IOException();
					case 2:
						throw new AccessViolationException();
				}
				throw new Exception();
			};
			// no timing because with exceptions there would be no delays
			// ReSharper disable once UnusedVariable
			var result = await func.RetryAsync<int, IOException, AccessViolationException, int>(72, retryCount, millisecondRetryDelay);
			Assert.Fail();
		}

		[Test]
		public async Task AsyncRetryWithReturnValueWithOneArgumentWithOneExceptionSucceeds()
		{
			var retryCount = 3;
			var millisecondRetryDelay = 20;
			int callCount = 0;
			Func<int, Task<int>> func = a =>
			{
				callCount++;
				switch (callCount)
				{
					case 1:
						throw new IOException();
				}
				return Task.FromResult(a);
			};
			// no timing because with exceptions there would be no delays
			// ReSharper disable once UnusedVariable
			var result = await func.RetryAsync<int, IOException, int>(72, retryCount, millisecondRetryDelay);
			Assert.AreEqual(2, callCount);
			Assert.AreEqual(72, result);
		}

		[Test, ExpectedException(typeof(Exception))]
		public async Task AsyncRetryWithReturnValueWithOneArgumentWithOneExceptionThrows()
		{
			var retryCount = 3;
			var millisecondRetryDelay = 20;
			int callCount = 0;
			Func<int, Task<int>> func = a =>
			{
				callCount++;
				switch (callCount)
				{
					case 1:
						throw new IOException();
				}
				throw new Exception();
			};
			// no timing because with exceptions there would be no delays
			// ReSharper disable once UnusedVariable
			var result = await func.RetryAsync<int, IOException, int>(72, retryCount, millisecondRetryDelay);
			Assert.Fail();
		}

		[Test]
		public async Task AsyncRetryWithReturnValueWithThreeExceptionsSucceeds()
		{
			var retryCount = 4;
			var millisecondRetryDelay = 20;
			int callCount = 0;
			Func<Task<int>> func = () =>
			{
				callCount++;
				switch (callCount)
				{
					case 1:
						throw new IOException();
					case 2:
						throw new AccessViolationException();
					case 3:
						throw new DivideByZeroException();
				}
				return Task.FromResult(42);
			};
			// no timing because with three exceptions there would be no delays
			var result =
				await
					func.RetryAsync<IOException, AccessViolationException, DivideByZeroException, int>(retryCount,
						millisecondRetryDelay);
			Assert.AreEqual(retryCount, callCount);
			Assert.AreEqual(42, result);
		}
	}
}