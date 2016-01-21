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

		#region async void retry support tests
#if false
		[Test, ExpectedException(typeof(Exception))]
		public async Task AsyncRetryWithReturnValueWithTwoExceptionsThrowsOfUnexpectedType()
		{
			var retryCount = 3;
			var millisecondRetryDelay = 20;
			int callCount = 0;
			Func<int, Task<int>> func = _ =>
			{
				callCount++;
				switch (callCount)
				{
					case 1:
						throw new IOException();
					case 2:
						throw new AccessViolationException();
					case 3:
						throw new Exception();
				}
				return Task.FromResult(42);
			};
			await
				func.RetryAsync<int, IOException, AccessViolationException>(72, retryCount, millisecondRetryDelay);
		}


		[Test, ExpectedException(typeof(AccessViolationException))]
		public async Task AsyncRetryWithReturnValueWithOneArgumentWithOneExceptionThrowsOfUnexpectedType()
		{
			var retryCount = 3;
			var millisecondRetryDelay = 20;
			int callCount = 0;
			Func<int, Task<int>> func = _ =>
			{
				callCount++;
				switch (callCount)
				{
					case 1:
						throw new IOException();
					case 2:
						throw new AccessViolationException();
					case 3:
						throw new Exception();
				}
				return Task.FromResult(42);
			};
			await func.RetryAsync<int, IOException>(72, retryCount, millisecondRetryDelay);
		}

		[Test, ExpectedException(typeof(DivideByZeroException))]
		public async Task AsyncRetryWithOneArgumentWithThreeExceptions3RetriesThrowsOfUnexpectedType()
		{
			var retryCount = 3;
			var millisecondRetryDelay = 20;
			int callCount = 0;
			Func<int, Task> func = _ =>
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
					case 4:
						throw new Exception();
				}
				return (Task)Task.FromResult(1);
			};
			var stopwatch = System.Diagnostics.Stopwatch.StartNew();
			await
				func.RetryAsync<int, IOException, AccessViolationException, DivideByZeroException>(72, retryCount,
					millisecondRetryDelay);
			stopwatch.Stop();
			Assert.AreEqual(retryCount, callCount);
			Assert.IsTrue(stopwatch.ElapsedMilliseconds >= ((millisecondRetryDelay-1) * (callCount - 1)));
			Console.WriteLine(stopwatch.Elapsed.ToEnglishString());
		}

		[Test, ExpectedException(typeof(AccessViolationException))]
		public async Task AsyncRetryWithOneArgumentWithThreeExceptions2RetriesThrowsOfUnexpectedType()
		{
			var retryCount = 2;
			var millisecondRetryDelay = 20;
			int callCount = 0;
			Func<int, Task> func = _ =>
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
					case 4:
						throw new Exception();
				}
				return (Task)Task.FromResult(1);
			};
			var stopwatch = System.Diagnostics.Stopwatch.StartNew();
			await
				func.RetryAsync<int, IOException, AccessViolationException, DivideByZeroException>(72, retryCount,
					millisecondRetryDelay);
			stopwatch.Stop();
			Assert.AreEqual(retryCount, callCount);
			Assert.IsTrue(stopwatch.ElapsedMilliseconds >= ((millisecondRetryDelay-1) * (callCount - 1)));
			Console.WriteLine(stopwatch.Elapsed.ToEnglishString());
		}

		[Test, ExpectedException(typeof(IOException))]
		public async Task AsyncRetryWithOneArgumentWithThreeExceptions1RetriesThrowsOfUnexpectedType()
		{
			var retryCount = 1;
			var millisecondRetryDelay = 20;
			int callCount = 0;
			Func<int, Task> func = _ =>
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
					case 4:
						throw new Exception();
				}
				return (Task)Task.FromResult(1);
			};
			var stopwatch = System.Diagnostics.Stopwatch.StartNew();
			await
				func.RetryAsync<int, IOException, AccessViolationException, DivideByZeroException>(72, retryCount,
					millisecondRetryDelay);
			stopwatch.Stop();
			Assert.AreEqual(retryCount, callCount);
			Assert.IsTrue(stopwatch.ElapsedMilliseconds >= ((millisecondRetryDelay-1) * (callCount - 1)));
			Console.WriteLine(stopwatch.Elapsed.ToEnglishString());
		}

		[Test]
		public async Task AsyncRetryWithOneArgumentSucceedsWithSomeExceptionsOfOneType()
		{
			var retryCount = 3;
			var millisecondRetryDelay = 20;
			int callCount = 0;
			var stopwatch = System.Diagnostics.Stopwatch.StartNew();
			Func<int, Task> func = _ =>
			{
				callCount++;
				if (callCount < retryCount) throw new Exception();
				return (Task)Task.FromResult(1);
			};
			await func.RetryAsync(72, retryCount, millisecondRetryDelay);
			stopwatch.Stop();
			Assert.AreEqual(retryCount, callCount);
			Assert.IsTrue(stopwatch.ElapsedMilliseconds >= ((millisecondRetryDelay-1) * (callCount - 1)));
			Console.WriteLine(stopwatch.Elapsed.ToEnglishString());
		}

		[Test]
		public async Task AsyncRetryWithTwoArgumentsSucceedsWithSomeExceptionsOfOneType()
		{
			var retryCount = 3;
			var millisecondRetryDelay = 20;
			int callCount = 0;
			var stopwatch = System.Diagnostics.Stopwatch.StartNew();
			Func<int, int, Task> func = (a1, a2) =>
			{
				callCount++;
				if (callCount < retryCount) throw new Exception();
				return (Task)Task.FromResult(1);
			};
			await func.RetryAsync(72, 42, retryCount, millisecondRetryDelay);
			stopwatch.Stop();
			Assert.AreEqual(retryCount, callCount);
			Assert.IsTrue(stopwatch.ElapsedMilliseconds >= ((millisecondRetryDelay-1) * (callCount - 1)));
			Console.WriteLine(stopwatch.Elapsed.ToEnglishString());
		}

		[Test, ExpectedException(typeof(Exception))]
		public async Task AsyncRetryWithOneArgumentThrowsWithExceptionsOfOneType()
		{
			var retryCount = 3;
			var millisecondRetryDelay = 20;
			int callCount = 0;
			var stopwatch = System.Diagnostics.Stopwatch.StartNew();
			Func<int, Task> func = _ =>
			{
				callCount++;
				throw new Exception();
			};
			await func.RetryAsync(72, retryCount, millisecondRetryDelay);
			stopwatch.Stop();
			Assert.AreEqual(retryCount, callCount);
			Assert.IsTrue(stopwatch.ElapsedMilliseconds >= ((millisecondRetryDelay-1) * (callCount - 1)));
			Console.WriteLine(stopwatch.Elapsed.ToEnglishString());
		}

		[Test, ExpectedException(typeof(Exception))]
		public async Task AsyncRetryWithTwoArgumentsThrowsWithExceptionsOfOneType()
		{
			var retryCount = 3;
			var millisecondRetryDelay = 20;
			int callCount = 0;
			var stopwatch = System.Diagnostics.Stopwatch.StartNew();
			Func<int, int, Task> func = (a1, a2) =>
			{
				callCount++;
				throw new Exception();
			};
			await func.RetryAsync(72, 42, retryCount, millisecondRetryDelay);
			stopwatch.Stop();
			Assert.AreEqual(retryCount, callCount);
			Assert.IsTrue(stopwatch.ElapsedMilliseconds >= ((millisecondRetryDelay-1) * (callCount - 1)));
			Console.WriteLine(stopwatch.Elapsed.ToEnglishString());
		}

		[Test, ExpectedException(typeof(Exception))]
		public async Task AsyncRetryWithReturnValueSucceedsWith1RetriesThrowsException()
		{
			var retryCount = 2;
			var millisecondRetryDelay = 20;
			int callCount = 0;
			var stopwatch = System.Diagnostics.Stopwatch.StartNew();
			Func<Task> func = () =>
			{
				callCount++;
				throw new Exception();
			};
			await func.RetryAsync(retryCount, millisecondRetryDelay);
			Assert.Fail();
		}

		[Test]
		public async Task AsyncRetrySucceedsWith3RetriesSomeExceptionsOfOneType()
		{
			var retryCount = 3;
			var millisecondRetryDelay = 20;
			int callCount = 0;
			var stopwatch = System.Diagnostics.Stopwatch.StartNew();
			Func<Task> func = () =>
			{
				callCount++;
				if (callCount < retryCount) throw new Exception();
				return (Task)Task.FromResult(1);
			};
			await func.RetryAsync(retryCount, millisecondRetryDelay);
			stopwatch.Stop();
			Assert.AreEqual(retryCount, callCount);
			Assert.IsTrue(stopwatch.ElapsedMilliseconds >= ((millisecondRetryDelay-1) * (callCount - 1)));
			Console.WriteLine(stopwatch.Elapsed.ToEnglishString());
		}

		[Test, ExpectedException(typeof(Exception))]
		public async Task AsyncRetryWithReturnValueThrowsWithExceptionsOfUnexpectedType()
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
						throw new Exception();
				}
				return Task.FromResult(42);
			};
			await func.RetryAsync<IOException>(retryCount, millisecondRetryDelay);
		}

		[Test, ExpectedException(typeof(Exception))]
		public async Task AsyncRetryThrowsWithSomeExceptionsOfUnexpectedType()
		{
			var retryCount = 3;
			var millisecondRetryDelay = 20;
			int callCount = 0;
			Func<Task> func = () =>
			{
				callCount++;
				switch (callCount)
				{
					case 1:
						throw new IOException();
					case 2:
						throw new AccessViolationException();
					case 3:
						throw new Exception();
				}
				return (Task)Task.FromResult(1);
			};
			await func.RetryAsync<IOException, AccessViolationException>(retryCount, millisecondRetryDelay);
		}

		[Test, ExpectedException(typeof(DivideByZeroException))]
		public async Task AsyncRetryWithThreeExceptionsThrowsOfUnexpectedType()
		{
			var retryCount = 3;
			var millisecondRetryDelay = 20;
			int callCount = 0;
			Func<Task> func = () =>
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
					case 4:
						throw new Exception();
				}
				return (Task)Task.FromResult(1);
			};
			await
				func.RetryAsync<IOException, AccessViolationException, DivideByZeroException>(retryCount,
					millisecondRetryDelay);
		}

		[Test]
		public async Task AsyncRetryWithThreeExceptionsSucceeds()
		{
			var retryCount = 4;
			var millisecondRetryDelay = 20;
			int callCount = 0;
			Func<Task> func = () =>
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
				return (Task)Task.FromResult(1);
			};
			var stopwatch = System.Diagnostics.Stopwatch.StartNew();
			await
				func.RetryAsync<IOException, AccessViolationException, DivideByZeroException>(retryCount,
					millisecondRetryDelay);
			stopwatch.Stop();
			Assert.AreEqual(retryCount, callCount);
			Console.WriteLine(stopwatch.Elapsed.ToEnglishString());
			Assert.IsTrue(stopwatch.ElapsedMilliseconds >= ((millisecondRetryDelay-1) * (callCount - 1)));
		}

		[Test, ExpectedException(typeof(IOException))]
		public async Task AsyncRetryWithThreeExceptionsAnd1RetryThows()
		{
			var retryCount = 1;
			var millisecondRetryDelay = 20;
			int callCount = 0;
			Func<Task> func = () =>
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
				return (Task)Task.FromResult(1);
			};
			await
				func.RetryAsync<IOException, AccessViolationException, DivideByZeroException>(retryCount,
					millisecondRetryDelay);
			Assert.Fail();
		}

		[Test, ExpectedException(typeof(AccessViolationException))]
		public async Task AsyncRetryWithThreeExceptionsAnd2RetriesThows()
		{
			var retryCount = 2;
			var millisecondRetryDelay = 20;
			int callCount = 0;
			Func<Task> func = () =>
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
				return (Task)Task.FromResult(1);
			};
			await
				func.RetryAsync<IOException, AccessViolationException, DivideByZeroException>(retryCount,
					millisecondRetryDelay);
			Assert.Fail();
		}
		[Test, ExpectedException(typeof(DivideByZeroException))]
		public async Task AsyncRetryWithThreeExceptionsAnd3RetriesThows()
		{
			var retryCount = 3;
			var millisecondRetryDelay = 20;
			int callCount = 0;
			Func<Task> func = () =>
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
				return (Task)Task.FromResult(1);
			};
			await
				func.RetryAsync<IOException, AccessViolationException, DivideByZeroException>(retryCount,
					millisecondRetryDelay);
			Assert.Fail();
		}

		[Test]
		public async Task AsyncRetryWithOneArgumentWithThreeExceptionsSucceeds()
		{
			var retryCount = 4;
			var millisecondRetryDelay = 20;
			int callCount = 0;
			Func<int, Task> func = _ =>
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
				return (Task)Task.FromResult(1);
			};
			var stopwatch = System.Diagnostics.Stopwatch.StartNew();
			await
				func.RetryAsync<int, IOException, AccessViolationException, DivideByZeroException>(72, retryCount,
					millisecondRetryDelay);
			stopwatch.Stop();
			Assert.AreEqual(retryCount, callCount);
			Console.WriteLine(stopwatch.Elapsed.ToEnglishString());
			Assert.IsTrue(stopwatch.ElapsedMilliseconds >= ((millisecondRetryDelay-1) * (callCount - 1)));
		}
#endif
		#endregion async void retry support tests
	}
}