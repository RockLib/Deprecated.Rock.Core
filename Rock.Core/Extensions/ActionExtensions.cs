using System;

namespace Rock.Extensions
{
    /// <summary>
    /// Extension methods for the type Action.
    /// </summary>
	/// <remarks>
	/// <para>The ActionExtensions class provides helpers for retrying an action 
	/// The action is executed, and if the expected exception is
	/// encountered, the action is retried a certain number of times.  If the
	/// action never succeeds, the last encountered exception is thrown up the
	/// stack.  If an exception that wasn't expected is encountered, it is
	/// passed up the stack without any retries.</para>
	/// <para>Overloads are provided for catching multiple types of exceptions, so
	/// catching <see cref="System.Exception"/> is not necessary.</para>
	/// </remarks>
	/// <example>
	/// This example tries to move a file three times.  If an
	/// <see cref="System.IO.IOException"/> is encountered when trying to move
	/// the file on the first two tries, the thread sleeps 5 seconds then tries
	/// again.  On the third try, if an IOException is encountered, it is
	/// thrown.
	/// <code>
    /// <![CDATA[
	/// Action action = () => System.IO.File.Move(source, destination);
	/// action.Retry<System.IO.IOException>(3, 5 * 1000);
    /// ]]>
	/// </code>
	/// </example>
	public static class ActionExtensions
	{
		/// <summary>
		/// Retry an action if a TException is encountered
		/// </summary>
		/// <typeparam name="TException">exception type to catch</typeparam>
		/// <param name="action">action to execute</param>
		/// <param name="numberOfTries">number of times to try action</param>
		/// <param name="millesecondsSleepBetweenRetry">milleseconds to
		/// sleep before next retry if exception is encountered</param>
		/// <exception cref="System.ArgumentNullException">thrown when action
		/// is null</exception>
		/// <example>
		/// This example tries to move a file three times.  If an
		/// <see cref="System.IO.IOException"/> is encountered when trying to move
		/// the file on the first two tries, the thread sleeps 5 seconds then tries
		/// again.  On the third try, if an IOException is encountered, it is
		/// thrown.
		/// <code>
		/// <![CDATA[
		/// Action action = () => System.IO.File.Move(source, destination);
		/// action.Retry<System.IO.IOException>(3, 5 * 1000);
		/// ]]>
		/// </code>
		/// </example>
		public static void Retry<TException>(
			   this Action action,
			   int numberOfTries,
			   int millesecondsSleepBetweenRetry)
			where TException : Exception
		{
			TestArgumentNull(action, "action");

            Func<object> func = () =>
                                {
                                    action();
                                    return null;
                                };

			func.Retry<object, TException, TException, TException>(
				numberOfTries,
				millesecondsSleepBetweenRetry);
		}

		/// <summary>
		/// Retry an action if a TException1 or TException2 is encountered
		/// </summary>
		/// <typeparam name="TException1">exception type to catch</typeparam>
		/// <typeparam name="TException2">exception type to catch</typeparam>
		/// <param name="action">action to execute</param>
		/// <param name="numberOfTries">number of times to try action</param>
		/// <param name="millesecondsSleepBetweenRetry">milleseconds to
		/// sleep before next retry if exception is encountered</param>
		/// <exception cref="System.ArgumentNullException">thrown when action
		/// is null</exception>
		/// <example>
		/// This example tries to move a file three times.  If an
		/// <see cref="System.IO.IOException"/> or
		/// <see cref="System.UnauthorizedAccessException"/> is encountered
		/// when trying to move the file on the first two tries, the thread
		/// sleeps 5 seconds then tries again.  On the third try, if an
		/// IOException or UnauthorizedAccessException is encountered, it is thrown.
		/// <code> 
		/// <![CDATA[
		/// Action action = () => System.IO.File.Move(source, destination);
		/// action.Retry<System.IO.IOException, System.IO.UnauthorizedAccessException>(3, 5 * 1000);
		/// ]]>
		/// </code>
		/// </example>
		public static void Retry<TException1, TException2>(
			   this Action action,
			   int numberOfTries,
			   int millesecondsSleepBetweenRetry)
			where TException1 : Exception
			where TException2 : Exception
		{
			TestArgumentNull(action, "action");

            Func<object> func = () =>
                                {
                                    action();
                                    return null;
                                };

			func.Retry<object, TException1, TException1, TException2>(
				numberOfTries,
				millesecondsSleepBetweenRetry);
		}

		/// <summary>
		/// Retry an action if a TException1, TException2, or TException3 is
		/// encountered
		/// </summary>
		/// <typeparam name="TException1">exception type to catch</typeparam>
		/// <typeparam name="TException2">exception type to catch</typeparam>
		/// <typeparam name="TException3">exception type to catch</typeparam>
		/// <param name="action">action to execute</param>
		/// <param name="numberOfTries">number of times to try action</param>
		/// <param name="millesecondsSleepBetweenRetry">milleseconds to
		/// sleep before next retry if exception is encountered</param>
		/// <exception cref="System.ArgumentNullException">thrown when action
		/// is null</exception>
		/// <example>
		/// This example tries to move a file three times.  If an
		/// <see cref="System.IO.IOException"/>,
		/// <see cref="System.IO.DirectoryNotFoundException"/>, or
		/// <see cref="System.UnauthorizedAccessException"/> is encountered
		/// when trying to move the file on the first two tries, the thread
		/// sleeps 5 seconds then tries again.  On the third try, if an
		/// IOException, DirectoryNotFound, or UnauthorizedAccessException is
		/// encountered, it is thrown.
		/// <code>
		/// <![CDATA[
		/// Action action = () => System.IO.File.Move(source, destination);
		/// action.Retry<
		///     System.IO.IOException,
		///     System.IO.UnauthorizedAccessException,
		///     System.IO.DirectoryNotFoundException>(3, 5 * 1000);
		/// ]]>
		/// </code>
		/// </example>
		public static void Retry<TException1, TException2, TException3>(
			   this Action action,
			   int numberOfTries,
			   int millesecondsSleepBetweenRetry)
			where TException1 : Exception
			where TException2 : Exception
			where TException3 : Exception
		{
			TestArgumentNull(action, "action");

            Func<object> func = () =>
                                {
                                    action();
                                    return null;
                                };

			func.Retry<object, TException1, TException2, TException3>(
				numberOfTries,
				millesecondsSleepBetweenRetry);
		}

        #region 1 Parameter
        /// <summary>
        /// Retry an action if a TException is encountered
        /// </summary>
        /// <typeparam name="TException">exception type to catch</typeparam>
        /// <typeparam name="T">parameter type</typeparam>
        /// <param name="parameter">action parameter</param>
        /// <param name="action">action to execute</param>
        /// <param name="numberOfTries">number of times to try action</param>
        /// <param name="millisecondsSleepBetweenRetry">milliseconds to
        /// sleep before next retry if exception is encountered</param>
        /// <exception cref="System.ArgumentNullException">thrown when action
        /// is null</exception>
        /// <example>
        /// <code>
        ///   <![CDATA[
        ///   public static void ActOnDocument(int documentId)
        ///   {
        ///     return Wcf.Service<ILifeCycle>.Use( (serviceProxy)=>{ serviceProxy.AddToLifeCycle(documentId); } );
        ///   }
        ///   
        ///   public static void ActOnDocument(int documentId, int attempts, int waitBetweenAttempts)
        ///   {
        ///     Action<int> action = ReprocessDocument;
        ///     action.Retry<int, Exception>(documentId, attempts, waitBetweenAttempts); 
        ///   }
        /// ]]>
        /// </code>
        /// </example>
        public static void Retry<T, TException>(
               this Action<T> action,
                T parameter,
               int numberOfTries,
               int millisecondsSleepBetweenRetry)
            where TException : Exception
        {
            TestArgumentNull(action, "action");

            Func<T, object> func = param =>
            {
                action(param);
                return null;
            };

            func.Retry<T, object, TException, TException, TException>(parameter,
                numberOfTries,
                millisecondsSleepBetweenRetry);
        }

        /// <summary>
        /// Retry an action if a TException1 or TException2 is encountered
        /// </summary>
        /// <typeparam name="TException1">exception type to catch</typeparam>
        /// <typeparam name="TException2">exception type to catch</typeparam>
        /// <typeparam name="T">parameter type</typeparam>
        /// <param name="parameter">action parameter</param>
        /// <param name="action">action to execute</param>
        /// <param name="numberOfTries">number of times to try action</param>
        /// <param name="millisecondsSleepBetweenRetry">milliseconds to
        /// sleep before next retry if exception is encountered</param>
        /// <exception cref="System.ArgumentNullException">thrown when action
        /// is null</exception>
        /// <example>
        /// <code>
        ///   <![CDATA[
        ///   public static void ActOnDocument(int documentId)
        ///   {
        ///     return Wcf.Service<ILifeCycle>.Use( (serviceProxy)=>{ serviceProxy.AddToLifeCycle(documentId); } );
        ///   }
        ///   
        ///   public static void ActOnDocument(int documentId, int attempts, int waitBetweenAttempts)
        ///   {
        ///     Action<int> action = ReprocessDocument;
        ///     action.Retry<int, TimeoutException, CommunicationException>(documentId, attempts, waitBetweenAttempts); 
        ///   }
        /// ]]>
        /// </code>
        /// </example>
        public static void Retry<T, TException1, TException2>(
               this Action<T> action,
                T parameter,
               int numberOfTries,
               int millisecondsSleepBetweenRetry)
            where TException1 : Exception
            where TException2 : Exception
        {
            TestArgumentNull(action, "action");

            Func<T, object> func = param =>
            {
                action(param);
                return null;
            };

            func.Retry<T, object, TException1, TException1, TException2>(
                parameter,
                numberOfTries,
                millisecondsSleepBetweenRetry);
        }

        /// <summary>
        /// Retry an action if a TException1, TException2, or TException3 is
        /// encountered
        /// </summary>
        /// <typeparam name="TException1">exception type to catch</typeparam>
        /// <typeparam name="TException2">exception type to catch</typeparam>
        /// <typeparam name="TException3">exception type to catch</typeparam>
        /// <typeparam name="T">parameter type</typeparam>
        /// <param name="parameter">action parameter</param>
        /// <param name="action">action to execute</param>
        /// <param name="numberOfTries">number of times to try action</param>
        /// <param name="millisecondsSleepBetweenRetry">milliseconds to
        /// sleep before next retry if exception is encountered</param>
        /// <exception cref="System.ArgumentNullException">thrown when action
        /// is null</exception>
        /// <example>
        /// <code>
        ///   <![CDATA[
        ///   public static void ActOnDocument(int documentId)
        ///   {
        ///     return Wcf.Service<ILifeCycle>.Use( (serviceProxy)=>{ serviceProxy.AddToLifeCycle(documentId); } );
        ///   }
        ///   
        ///   public static void ActOnDocument(int documentId, int attempts, int waitBetweenAttempts)
        ///   {
        ///     Action<int> action = ReprocessDocument;
        ///     action.Retry<int, TimeoutException, CommunicationException, FaultException<RetriableDetail>>(documentId, attempts, waitBetweenAttempts); 
        ///   }
        /// ]]>
        /// </code>
        /// </example>
        public static void Retry<T, TException1, TException2, TException3>(
               this Action<T> action,
                T parameter,
               int numberOfTries,
               int millisecondsSleepBetweenRetry)
            where TException1 : Exception
            where TException2 : Exception
            where TException3 : Exception
        {
            TestArgumentNull(action, "action");

            Func<T, object> func = param =>
            {
                action(param);
                return null;
            };

            func.Retry<T, object, TException1, TException2, TException3>(
                parameter,
                numberOfTries,
                millisecondsSleepBetweenRetry);
        }
        #endregion
        #region 2 Parameters
        /// <summary>
        /// Retry an action if a TException is encountered
        /// </summary>
        /// <typeparam name="TException">exception type to catch</typeparam>
        /// <typeparam name="T1">parameter1 type</typeparam>
        /// <typeparam name="T2">parameter2 type</typeparam>
        /// <param name="parameter1">action parameter1</param>
        /// <param name="parameter2">action parameter2</param>
        /// <param name="action">action to execute</param>
        /// <param name="numberOfTries">number of times to try action</param>
        /// <param name="millisecondsSleepBetweenRetry">milliseconds to
        /// sleep before next retry if exception is encountered</param>
        /// <exception cref="System.ArgumentNullException">thrown when action
        /// is null</exception>
        /// <example>
        /// <code>
        /// <![CDATA[
        /// Action<string, string> action = System.IO.File.Move;
        /// action.Retry<string, string, System.IO.IOException>(source, destination, 3, 5 * 1000);
        /// ]]>
        /// </code>
        /// </example>
        public static void Retry<T1, T2, TException>(
               this Action<T1, T2> action,
                T1 parameter1,
                T2 parameter2,
               int numberOfTries,
               int millisecondsSleepBetweenRetry)
            where TException : Exception
        {
            TestArgumentNull(action, "action");

            Func<T1, T2, object> func = (param1, param2) =>
            {
                action(param1, param2);
                return null;
            };

            func.Retry<T1, T2, object, TException, TException, TException>(parameter1, parameter2,
                numberOfTries,
                millisecondsSleepBetweenRetry);
        }

        /// <summary>
        /// Retry an action if a TException1 or TException2 is encountered
        /// </summary>
        /// <typeparam name="TException1">exception type to catch</typeparam>
        /// <typeparam name="TException2">exception type to catch</typeparam>
        /// <typeparam name="T1">parameter1 type</typeparam>
        /// <typeparam name="T2">parameter2 type</typeparam>
        /// <param name="parameter1">action parameter1</param>
        /// <param name="parameter2">action parameter2</param>
        /// <param name="action">action to execute</param>
        /// <param name="numberOfTries">number of times to try action</param>
        /// <param name="millisecondsSleepBetweenRetry">milliseconds to
        /// sleep before next retry if exception is encountered</param>
        /// <exception cref="System.ArgumentNullException">thrown when action
        /// is null</exception>
        /// <example>
        /// <code>
        /// <![CDATA[
        /// Action<string, string> action = System.IO.File.Move;
        /// action.Retry<string, string, System.IO.IOException, UnauthorizedAccessException>(source, destination, 3, 5 * 1000);
        /// ]]>
        /// </code>
        /// </example>
        public static void Retry<T1, T2, TException1, TException2>(
               this Action<T1, T2> action,
                T1 parameter1,
                T2 parameter2,
               int numberOfTries,
               int millisecondsSleepBetweenRetry)
            where TException1 : Exception
            where TException2 : Exception
        {
            TestArgumentNull(action, "action");

            Func<T1, T2, object> func = (param1, param2) =>
            {
                action(param1, param2);
                return null;
            };

            func.Retry<T1, T2, object, TException1, TException1, TException2>(
                parameter1,
                parameter2,
                numberOfTries,
                millisecondsSleepBetweenRetry);
        }

        /// <summary>
        /// Retry an action if a TException1, TException2, or TException3 is
        /// encountered
        /// </summary>
        /// <typeparam name="TException1">exception type to catch</typeparam>
        /// <typeparam name="TException2">exception type to catch</typeparam>
        /// <typeparam name="TException3">exception type to catch</typeparam>
        /// <typeparam name="T1">parameter1 type</typeparam>
        /// <typeparam name="T2">parameter2 type</typeparam>
        /// <param name="parameter1">action parameter1</param>
        /// <param name="parameter2">action parameter2</param>
        /// <param name="action">action to execute</param>
        /// <param name="numberOfTries">number of times to try action</param>
        /// <param name="millisecondsSleepBetweenRetry">milliseconds to
        /// sleep before next retry if exception is encountered</param>
        /// <exception cref="System.ArgumentNullException">thrown when action
        /// is null</exception>
        /// <example>
        /// <code>
        /// <![CDATA[
        /// Action<string, string> action = System.IO.File.Move;
        /// action.Retry<string, string, System.IO.IOException, UnauthorizedAccessException, System.IO.DirectoryNotFoundException>(source, destination, 3, 5 * 1000);
        /// ]]>
        /// </code>
        /// </example>
        public static void Retry<T1, T2, TException1, TException2, TException3>(
               this Action<T1, T2> action,
                T1 parameter1,
                T2 parameter2,
               int numberOfTries,
               int millisecondsSleepBetweenRetry)
            where TException1 : Exception
            where TException2 : Exception
            where TException3 : Exception
        {
            TestArgumentNull(action, "action");

            Func<T1, T2, object> func = (param1, param2) =>
            {
                action(param1, param2);
                return null;
            };

            func.Retry<T1, T2, object, TException1, TException2, TException3>(
                parameter1,
                parameter2,
                numberOfTries,
                millisecondsSleepBetweenRetry);
        }
        #endregion
        #region 3 Parameters
        /// <summary>
        /// Retry an action if a TException is encountered
        /// </summary>
        /// <typeparam name="TException">exception type to catch</typeparam>
        /// <typeparam name="T1">parameter1 type</typeparam>
        /// <typeparam name="T2">parameter2 type</typeparam>
        /// <typeparam name="T3">parameter3 type</typeparam>
        /// <param name="parameter1">action parameter1</param>
        /// <param name="parameter2">action parameter2</param>
        /// <param name="parameter3">action parameter3</param>
        /// <param name="action">action to execute</param>
        /// <param name="numberOfTries">number of times to try action</param>
        /// <param name="millisecondsSleepBetweenRetry">milliseconds to
        /// sleep before next retry if exception is encountered</param>
        /// <exception cref="System.ArgumentNullException">thrown when action
        /// is null</exception>
        /// <example>
        /// <code>
        /// <![CDATA[
        /// Action<string, string, boolean> action = System.IO.File.Copy;
        /// action.Retry<string, string, boolean, System.IO.IOException>("originalFile", "newFile", true, 3, 5 * 1000);
        /// ]]>
        /// </code>
        /// </example>
        public static void Retry<T1, T2, T3, TException>(
               this Action<T1, T2, T3> action,
                T1 parameter1,
                T2 parameter2,
                T3 parameter3,
               int numberOfTries,
               int millisecondsSleepBetweenRetry)
            where TException : Exception
        {
            TestArgumentNull(action, "action");

            Func<T1, T2, T3, object> func = (param1, param2, param3) =>
            {
                action(param1, param2, param3);
                return null;
            };

            func.Retry<T1, T2, T3, object, TException, TException, TException>(parameter1, parameter2, parameter3,
                numberOfTries,
                millisecondsSleepBetweenRetry);
        }

        /// <summary>
        /// Retry an action if a TException is encountered
        /// </summary>
        /// <typeparam name="TException1">exception type to catch</typeparam>
        /// <typeparam name="TException2">exception type to catch</typeparam>
        /// <typeparam name="T1">parameter1 type</typeparam>
        /// <typeparam name="T2">parameter2 type</typeparam>
        /// <typeparam name="T3">parameter3 type</typeparam>
        /// <param name="parameter1">action parameter1</param>
        /// <param name="parameter2">action parameter2</param>
        /// <param name="parameter3">action parameter3</param>
        /// <param name="action">action to execute</param>
        /// <param name="numberOfTries">number of times to try action</param>
        /// <param name="millisecondsSleepBetweenRetry">milliseconds to
        /// sleep before next retry if exception is encountered</param>
        /// <exception cref="System.ArgumentNullException">thrown when action
        /// is null</exception>
        /// <example>
        /// <code>
        /// <![CDATA[
        /// Action<string, string, boolean> action = System.IO.File.Copy;
        /// action.Retry<string, string, boolean, System.IO.IOException>("originalFile", "newFile", true, 3, 5 * 1000);
        /// ]]>
        /// </code>
        /// </example>

        public static void Retry<T1, T2, T3, TException1, TException2>(
               this Action<T1, T2, T3> action,
                T1 parameter1,
                T2 parameter2,
                T3 parameter3,
               int numberOfTries,
               int millisecondsSleepBetweenRetry)
            where TException1 : Exception
            where TException2 : Exception
        {
            TestArgumentNull(action, "action");

            Func<T1, T2, T3, object> func = (param1, param2, param3) =>
            {
                action(param1, param2, param3);
                return null;
            };

            func.Retry<T1, T2, T3, object, TException1, TException1, TException2>(
                parameter1,
                parameter2,
                parameter3,
                numberOfTries,
                millisecondsSleepBetweenRetry);
        }

        /// <summary>
        /// Retry an action if a TException1, TException2, or TException3 is
        /// encountered
        /// </summary>
        /// <typeparam name="TException1">exception type to catch</typeparam>
        /// <typeparam name="TException2">exception type to catch</typeparam>
        /// <typeparam name="TException3">exception type to catch</typeparam>
        /// <typeparam name="T1">parameter1 type</typeparam>
        /// <typeparam name="T2">parameter2 type</typeparam>
        /// <typeparam name="T3">parameter3 type</typeparam>
        /// <param name="parameter1">action parameter1</param>
        /// <param name="parameter2">action parameter2</param>
        /// <param name="parameter3">action parameter3</param>
        /// <param name="action">action to execute</param>
        /// <param name="numberOfTries">number of times to try action</param>
        /// <param name="millisecondsSleepBetweenRetry">milliseconds to
        /// sleep before next retry if exception is encountered</param>
        /// <exception cref="System.ArgumentNullException">thrown when action
        /// is null</exception>
        /// <example>
        /// <code>
        /// <![CDATA[
        /// Action<string, string, boolean> action = System.IO.File.Copy;
        /// action.Retry<string, string, boolean, System.IO.IOException, UnauthorizedAccessException, System.IO.DirectoryNotFoundException>(source, destination, true, 3, 5 * 1000);
        /// ]]>
        /// </code>
        /// </example>
        public static void Retry<T1, T2, T3, TException1, TException2, TException3>(
               this Action<T1, T2, T3> action,
                T1 parameter1,
                T2 parameter2,
                T3 parameter3,
               int numberOfTries,
               int millisecondsSleepBetweenRetry)
            where TException1 : Exception
            where TException2 : Exception
            where TException3 : Exception
        {
            TestArgumentNull(action, "action");

            Func<T1, T2, T3, object> func = (param1, param2, param3) =>
            {
                action(param1, param2, param3);
                return null;
            };

            func.Retry<T1, T2, T3, object, TException1, TException2, TException3>(
                parameter1,
                parameter2,
                parameter3,
                numberOfTries,
                millisecondsSleepBetweenRetry);
        }
        #endregion

		private static void TestArgumentNull(object argument, string argumentName)
		{
			if (argument == null)
			{
				throw new ArgumentNullException(argumentName);
			}
		}

	    /// <summary>
	    /// Retry an action if a TException is encountered
	    /// </summary>
	    /// <typeparam name="TException">exception type to catch</typeparam>
	    /// <param name="action">action to execute</param>
	    /// <param name="numberOfTries">number of times to try action</param>
	    /// <param name="millisecondsSleepBetweenRetry">milliseconds to
	    /// sleep before next retry if exception is encountered</param>
	    /// <param name="retryWhen">Predicate to decide if operation is retryable for this type of exception</param>
		/// <exception cref="System.ArgumentNullException">thrown when <paramref name="action"/> is null</exception>
		/// <exception cref="System.ArgumentNullException">thrown when <paramref name="retryWhen"/> is null</exception>
		/// <example>
	    /// <code>
	    /// <![CDATA[
		/// Action action = CopyFile;
		/// action.Retry<System.IO.IOException>(
		///     source, 
		///     3, 
		///     5 * 1000,
		///     (ex)=>{
		///         var hr = Marshal.GetHRForException(ex);
		///         return hr == 0x8000FFFF
		///     }
		///     );
	    /// ]]>
	    /// </code>
	    /// </example>
		public static void Retry<TException>(
			this Action action,
			int numberOfTries,
			int millisecondsSleepBetweenRetry,
			Predicate<TException> retryWhen)
			where TException : Exception
		{
			numberOfTries = Math.Max(0, numberOfTries);
			Func<object> func = () =>
			{
				action();
				return null;
			};

			func.Retry(
				numberOfTries,
				millisecondsSleepBetweenRetry,
				retryWhen, retryWhen, retryWhen);
		}

	    /// <summary>
		/// Retry an action if a TException1 or TException2 is encountered
		/// </summary>
	    /// <typeparam name="TException1">exception type to catch</typeparam>
		/// <typeparam name="TException2">exception type to catch</typeparam>
		/// <param name="action">action to execute</param>
	    /// <param name="numberOfTries">number of times to try action</param>
	    /// <param name="millisecondsSleepBetweenRetry">milliseconds to
	    /// sleep before next retry if exception is encountered</param>
	    /// <param name="retryWhen1">Predicate to decide if operation is retryable for exception <typeparamref name="TException1"/></param>
		/// <param name="retryWhen2">Predicate to decide if operation is retryable for exception <typeparamref name="TException2"/></param>
		/// <exception cref="System.ArgumentNullException">thrown when <paramref name="action"/> is null</exception>
		/// <exception cref="System.ArgumentNullException">thrown when <paramref name="retryWhen1"/> is null</exception>
		/// <exception cref="System.ArgumentNullException">thrown when <paramref name="retryWhen2"/> is null</exception>
		/// <example>
	    /// <code>
	    /// <![CDATA[
	    /// Action action = CopyFile;
	    /// action.Retry<System.IO.IOException, UnauthorizedAccessedException>(
		///     source, 
		///     3, 
		///     5 * 1000,
		///     (ex1)=>{
		///         var hr = Marshal.GetHRForException(ex1);
		///         return hr == 0x8000FFFF
		///     },
		///     (ex2)=> true
		///     );
	    /// ]]>
	    /// </code>
	    /// </example>
		public static void Retry<TException1, TException2>(
			this Action action,
			int numberOfTries,
			int millisecondsSleepBetweenRetry,
			Predicate<TException1> retryWhen1,
			Predicate<TException2> retryWhen2)
			where TException1 : Exception
			where TException2 : Exception
		{
			numberOfTries = Math.Max(0, numberOfTries);
			Func<object> func = () =>
			{
				action();
				return null;
			};

			func.Retry(
				numberOfTries,
				millisecondsSleepBetweenRetry,
				retryWhen1, retryWhen1, retryWhen2);
		}

	    /// <summary>
		/// Retry an action if a TException1, TException2, or TException3 is encountered
		/// </summary>
	    /// <typeparam name="TException1">exception type to catch</typeparam>
		/// <typeparam name="TException2">exception type to catch</typeparam>
		/// <typeparam name="TException3">exception type to catch</typeparam>
		/// <param name="action">action to execute</param>
	    /// <param name="numberOfTries">number of times to try action</param>
	    /// <param name="millisecondsSleepBetweenRetry">milliseconds to
	    /// sleep before next retry if exception is encountered</param>
	    /// <param name="retryWhen1">Predicate to decide if operation is retryable for exception <typeparamref name="TException1"/></param>
		/// <param name="retryWhen2">Predicate to decide if operation is retryable for exception <typeparamref name="TException2"/></param>
		/// <param name="retryWhen3">Predicate to decide if operation is retryable for exception <typeparamref name="TException3"/></param>
		/// <exception cref="System.ArgumentNullException">thrown when <paramref name="action"/> is null</exception>
		/// <exception cref="System.ArgumentNullException">thrown when <paramref name="retryWhen1"/> is null</exception>
		/// <exception cref="System.ArgumentNullException">thrown when <paramref name="retryWhen2"/> is null</exception>
		/// <exception cref="System.ArgumentNullException">thrown when <paramref name="retryWhen3"/> is null</exception>
		/// <example>
	    /// <code>
	    /// <![CDATA[
		/// GetObjectResponse objectResponse = null;
		/// Action action = ()=>{objectResponse = amazonS3Client.GetObject(s3BucketName, s3Filename);};
		/// action.Retry<AmazonServiceException, AmazonS3Exception, AmazonClientException>(
		///     3, 
		///     5 * 1000,
		///     (ex1)=>ex1.ErrorType == ErrorType.Sender && ex1.ErrorCode == null && ex1.StatusCode == 0, // return on unknown error
		///     (ex2)=> ex2.StatusCode == HttpStatusCode.NotFound, // retry until found
		///     (ex3)=> true // always retry this exception
		///     );
	    /// ]]>
	    /// </code>
	    /// </example>
		public static void Retry<TException1, TException2, TException3>(
			this Action action,
			int numberOfTries,
			int millisecondsSleepBetweenRetry,
			Predicate<TException1> retryWhen1,
			Predicate<TException2> retryWhen2,
			Predicate<TException3> retryWhen3)
			where TException1 : Exception
			where TException2 : Exception
			where TException3 : Exception
		{
			numberOfTries = Math.Max(0, numberOfTries);
			Func<object> func = () =>
			{
				action();
				return null;
			};

			func.Retry(
				numberOfTries,
				millisecondsSleepBetweenRetry,
				retryWhen1, retryWhen2, retryWhen3);
		}
	}
}
