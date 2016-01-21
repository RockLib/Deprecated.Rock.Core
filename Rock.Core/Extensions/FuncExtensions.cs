using System;
using System.Threading;
using System.Threading.Tasks;

namespace Rock.Extensions
{
    /// <summary>Extensions for the Func type.</summary>
	/// <remarks>
	/// <para>The FuncExtensions class provides helpers for retrying a function 
	/// The function is executed, and if the expected exception is
	/// encountered, the function is retried a certain number of times.  If the
	/// function never succeeds, the last encountered exception is thrown up the
	/// stack.  If an exception that wasn't expected is encountered, it is
	/// passed up the stack without any retries.
    /// </para>
	/// <para>Overloads are provided for catching multiple types of exceptions, so
	/// catching <see cref="System.Exception"/> is not necessary.
    /// </para>
	/// </remarks>
	/// <example>
	/// This example returns a filesteam
	/// <code>
	/// <![CDATA[
	/// var func = new Func<System.IO.FileStream>(() =>
	/// {
	///     return System.IO.File.Open(filepath, System.IO.FileMode.Open);
	/// });
	/// var stream = func.Retry<System.IO.FileStream, System.IO.IOException>(3, 0);
	/// ]]>
	/// </code>
    /// <para>This example tries to open a file 3 times.  If a
    ///   <see cref="System.IO.IOException"/> is encountered the first two
    /// tries, it tries again.  If an IOException is encountered on the
    /// third try, it is thrown.  If a call to func succeeds, the result is
    /// returned.
    /// </para>
    /// <code>
    ///   <![CDATA[
    /// var func = new Func<System.IO.FileStream>(() =>
    /// {
    /// return System.IO.File.Open(filepath, System.IO.FileMode.Open);
    /// });
    /// var stream = func.Retry<System.IO.FileStream, System.IO.IOException>(3, 0);
    /// ]]>
    /// </code>
    ///  <para>This example tries to open a file 3 times.  If a
    ///   <see cref="System.IO.IOException"/> or
    ///   <see cref="System.UnauthorizedAccessException"/> is encountered
    /// the first two tries, it tries again.  If an IOException or an
    /// UnauthorizedAccessException is encountered on the third try, it is
    /// thrown.  If a call to func succeeds, the result is returned.
    /// </para>
    ///   <code>
    ///   <![CDATA[
    /// var func = new Func<System.IO.FileStream>(() =>
    /// {
    /// return System.IO.File.Open(filepath, System.IO.FileMode.Open);
    /// });
    /// var stream = func.Retry<
    /// System.IO.FileStream,
    /// System.IO.IOException,
    /// System.IO.UnauthorizedAccessException>(3, 0);
    /// ]]>
    /// </code>
    /// </example>
	public static partial class FuncExtensions
	{

        /// <summary>
        /// Retry a func if a TException is encountered
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <typeparam name="TException">The type of the exception.</typeparam>
        /// <param name="func">The func.</param>
        /// <param name="numberOfTries">The number of tries.</param>
        /// <param name="millesecondsSleepBetweenRetry">The milleseconds sleep between retry.</param>
        /// <returns>TResult</returns>
        /// <example>
        /// This example tries to open a file 3 times.  If a
        ///   <see cref="System.IO.IOException"/> is encountered the first two
        /// tries, it tries again.  If an IOException is encountered on the
        /// third try, it is thrown.  If a call to func succeeds, the result is
        /// returned.
        /// <code>
        ///   <![CDATA[
        /// var func = new Func<System.IO.FileStream>(() =>
        /// {
        /// return System.IO.File.Open(filepath, System.IO.FileMode.Open);
        /// });
        /// var stream = func.Retry<System.IO.FileStream, System.IO.IOException>(3, 0);
        /// ]]>
        /// </code>
        /// </example>
        /// <exception cref="System.ArgumentNullException">thrown when func is null</exception>
		public static TResult Retry<TResult, TException>(
			   this Func<TResult> func,
			   int numberOfTries,
			   int millesecondsSleepBetweenRetry)
			where TException : Exception
		{
			TestArgumentNull(func, "func");

			return Retry<TResult, TException, TException, TException>(
				func,
				numberOfTries,
				millesecondsSleepBetweenRetry);
		}

        /// <summary>
        /// Retry a func if a TException1 or TException2 is encountered
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <typeparam name="TException1">exception type to catch</typeparam>
        /// <typeparam name="TException2">exception type to catch</typeparam>
        /// <param name="func">func to execute</param>
        /// <param name="numberOfTries">number of times to try func</param>
        /// <param name="millesecondsSleepBetweenRetry">milleseconds to
        /// sleep before next retry if exception is encountered</param>
        /// <returns>TResult</returns>
        /// <exception cref="System.ArgumentNullException">thrown when func is null</exception>
        /// <example>
        /// This example tries to open a file 3 times.  If a
        ///   <see cref="System.IO.IOException"/> or
        ///   <see cref="System.UnauthorizedAccessException"/> is encountered
        /// the first two tries, it tries again.  If an IOException or an
        /// UnauthorizedAccessException is encountered on the third try, it is
        /// thrown.  If a call to func succeeds, the result is returned.
        ///   <code>
        ///   <![CDATA[
        /// var func = new Func<System.IO.FileStream>(() =>
        /// {
        /// return System.IO.File.Open(filepath, System.IO.FileMode.Open);
        /// });
        /// var stream = func.Retry<
        /// System.IO.FileStream,
        /// System.IO.IOException,
        /// System.IO.UnauthorizedAccessException>(3, 0);
        /// ]]>
        /// </code>
        /// </example>
		public static TResult Retry<TResult, TException1, TException2>(
			   this Func<TResult> func,
			   int numberOfTries,
			   int millesecondsSleepBetweenRetry)
			where TException1 : Exception
			where TException2 : Exception
		{
			TestArgumentNull(func, "func");

			return Retry<TResult, TException1, TException1, TException2>(
				func,
				numberOfTries,
				millesecondsSleepBetweenRetry);
		}

        /// <summary>
        /// Retry a func if TException1, TException2, TException3 is
        /// encountered
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <typeparam name="TException1">exception type to catch</typeparam>
        /// <typeparam name="TException2">exception type to catch</typeparam>
        /// <typeparam name="TException3">exception type to catch</typeparam>
        /// <param name="func">func to execute</param>
        /// <param name="numberOfTries">number of times to try func</param>
        /// <param name="millesecondsSleepBetweenRetry">milleseconds to
        /// sleep before next retry if exception is encountered</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">thrown when func
        /// is null</exception>
        /// <example>
        /// This example tries to open a file 3 times.  If a
        ///   <see cref="System.IO.IOException"/>,
        ///   <see cref="System.IO.FileNotFoundException"/>, or
        ///   <see cref="System.UnauthorizedAccessException"/> is encountered
        /// the first two tries, it tries again.  If an IOException,
        /// UnauthorizedAccessException, or a FileNotFoundException is
        /// encountered on the third try, it is thrown.  If a call to func
        /// succeeds, the result is returned.
        ///   <code>
        ///   <![CDATA[
        /// var func = new Func<System.IO.FileStream>(() =>
        /// {
        /// return System.IO.File.Open(filepath, System.IO.FileMode.Open);
        /// });
        /// var stream = func.Retry<
        /// System.IO.FileStream,
        /// System.IO.IOException,
        /// System.IO.UnauthorizedAccessException>(3, 0);
        /// ]]>
        ///   </code>
        ///   </example>
		public static TResult Retry<TResult, TException1, TException2, TException3>(
			   this Func<TResult> func,
			   int numberOfTries,
			   int millesecondsSleepBetweenRetry)
			where TException1 : Exception
			where TException2 : Exception
			where TException3 : Exception
		{
			TestArgumentNull(func, "func");

			numberOfTries--;

			do
			{
				try
				{
					return func();
				}
				catch (TException1)
				{
					if (numberOfTries <= 0)
					{
						throw;
					}
					else
					{
						Thread.Sleep(millesecondsSleepBetweenRetry);
					}
				}
				catch (TException2)
				{
					if (numberOfTries <= 0)
					{
						throw;
					}
					else
					{
						Thread.Sleep(millesecondsSleepBetweenRetry);
					}
				}
				catch (TException3)
				{
					if (numberOfTries <= 0)
					{
						throw;
					}
					else
					{
						Thread.Sleep(millesecondsSleepBetweenRetry);
					}
				}
			} while (numberOfTries-- > 0);

			throw new InvalidOperationException("You have found an unexpected path");
		}

        /// <summary>
        /// Retry a func if TException is encountered.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <typeparam name="TException">exception type to catch</typeparam>
        /// <param name="func">func to execute</param>
        /// <param name="numberOfTries">number of times to try func</param>
        /// <param name="millisecondsSleepBetweenRetry">milleseconds to
        /// sleep before next retry if exception is encountered</param>
        /// <param name="retryWhen">Predicate to decide if operation is retryable for exception <typeparamref name="TException"/></param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">thrown when func
        /// is null</exception>
        /// <example>
        /// This example tries to open get a file from S3 file up to 3 times.  If an
        /// AmazonClientException is encountered in
        /// the first two tries, it tries again.  If an IOException,
        /// AmazonServiceException occurs in the first two tries and the error is a sender error with StatusCode of 0,
        /// or an AmazonS3Exception with an status of NotFound is encounted it tries again.
        /// If a call to func succeeds, the result is returned.
        ///   <code>
        ///   <![CDATA[
        /// Func<GetObjectResponse> func = ()=>{return amazonS3Client.GetObject(s3BucketName, s3Filename);};
        /// GetObjectResponse response = func.Retry<AmazonServiceException, AmazonS3Exception, AmazonClientException>(
        ///     3, 
        ///     5 * 1000,
        ///     (ex1)=> ex1.ErrorType == ErrorType.Sender && ex1.ErrorCode == null && ex1.StatusCode == 0, // return on unknown error
        ///     (ex2)=> ex2.StatusCode == HttpStatusCode.NotFound, // retry until found
        ///     (ex3)=> true // always retry on this exception
        ///     );
        /// ]]>
        ///   </code>
        ///   </example>
        public static TResult Retry<TResult, TException>(
            this Func<TResult> func,
            int numberOfTries,
            int millisecondsSleepBetweenRetry,
            Predicate<TException> retryWhen)
            where TException : Exception
        {
            numberOfTries = Math.Max(0, numberOfTries);

            return func.Retry(
                numberOfTries,
                millisecondsSleepBetweenRetry,
                retryWhen, retryWhen, retryWhen);
        }

        /// <summary>
        /// Retry a func if TException1 or TException2 is encountered.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <typeparam name="TException1">exception type to catch</typeparam>
        /// <typeparam name="TException2">exception type to catch</typeparam>
        /// <param name="func">func to execute</param>
        /// <param name="numberOfTries">number of times to try func</param>
        /// <param name="millisecondsSleepBetweenRetry">milleseconds to
        /// sleep before next retry if exception is encountered</param>
        /// <param name="retryWhen1">Predicate to decide if operation is retryable for exception <typeparamref name="TException1"/></param>
        /// <param name="retryWhen2">Predicate to decide if operation is retryable for exception <typeparamref name="TException2"/></param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">thrown when func
        /// is null</exception>
        /// <example>
        /// This example tries to open get a file from S3 file up to 3 times.  If an
        /// AmazonClientException is encountered in
        /// the first two tries, it tries again.  If an IOException,
        /// AmazonServiceException occurs in the first two tries and the error is a sender error with StatusCode of 0,
        /// or an AmazonS3Exception with an status of NotFound is encounted it tries again.
        /// If a call to func succeeds, the result is returned.
        ///   <code>
        ///   <![CDATA[
        /// Func<GetObjectResponse> func = ()=>{return amazonS3Client.GetObject(s3BucketName, s3Filename);};
        /// GetObjectResponse response = func.Retry<AmazonServiceException, AmazonS3Exception, AmazonClientException>(
        ///     3, 
        ///     5 * 1000,
        ///     (ex1)=> ex1.ErrorType == ErrorType.Sender && ex1.ErrorCode == null && ex1.StatusCode == 0, // return on unknown error
        ///     (ex2)=> ex2.StatusCode == HttpStatusCode.NotFound, // retry until found
        ///     (ex3)=> true // always retry on this exception
        ///     );
        /// ]]>
        ///   </code>
        ///   </example>
        public static TResult Retry<TResult, TException1, TException2>(
            this Func<TResult> func,
            int numberOfTries,
            int millisecondsSleepBetweenRetry,
            Predicate<TException1> retryWhen1,
            Predicate<TException2> retryWhen2)
            where TException1 : Exception
            where TException2 : Exception
        {
            numberOfTries = Math.Max(0, numberOfTries);

            return func.Retry(
                numberOfTries,
                millisecondsSleepBetweenRetry,
                retryWhen1, retryWhen1, retryWhen2);
        }

		/// <summary>
		/// Retry a func if TException1, TException2, or TException3 is encountered.
		/// </summary>
		/// <typeparam name="TResult">The type of the result.</typeparam>
		/// <typeparam name="TException1">exception type to catch</typeparam>
		/// <typeparam name="TException2">exception type to catch</typeparam>
		/// <typeparam name="TException3">exception type to catch</typeparam>
		/// <param name="func">func to execute</param>
		/// <param name="numberOfTries">number of times to try func</param>
		/// <param name="millisecondsSleepBetweenRetry">milleseconds to
		/// sleep before next retry if exception is encountered</param>
		/// <param name="retryWhen1">Predicate to decide if operation is retryable for exception <typeparamref name="TException1"/></param>
		/// <param name="retryWhen2">Predicate to decide if operation is retryable for exception <typeparamref name="TException2"/></param>
		/// <param name="retryWhen3">Predicate to decide if operation is retryable for exception <typeparamref name="TException3"/></param>
		/// <returns></returns>
		/// <exception cref="System.ArgumentNullException">thrown when func
		/// is null</exception>
		/// <example>
		/// This example tries to open get a file from S3 file up to 3 times.  If an
		/// AmazonClientException is encountered in
		/// the first two tries, it tries again.  If an IOException,
		/// AmazonServiceException occurs in the first two tries and the error is a sender error with StatusCode of 0,
		/// or an AmazonS3Exception with an status of NotFound is encounted it tries again.
		/// If a call to func succeeds, the result is returned.
		///   <code>
		///   <![CDATA[
		/// Func<GetObjectResponse> func = ()=>{return amazonS3Client.GetObject(s3BucketName, s3Filename);};
        /// GetObjectResponse response = func.Retry<AmazonServiceException, AmazonS3Exception, AmazonClientException>(
		///     3, 
		///     5 * 1000,
		///     (ex1)=> ex1.ErrorType == ErrorType.Sender && ex1.ErrorCode == null && ex1.StatusCode == 0, // return on unknown error
		///     (ex2)=> ex2.StatusCode == HttpStatusCode.NotFound, // retry until found
		///     (ex3)=> true // always retry on this exception
		///     );
		/// ]]>
		///   </code>
		///   </example>
		public static TResult Retry<TResult, TException1, TException2, TException3>(
			this Func<TResult> func,
			int numberOfTries,
			int millisecondsSleepBetweenRetry,
			Predicate<TException1> retryWhen1,
			Predicate<TException2> retryWhen2,
			Predicate<TException3> retryWhen3)
			where TException1 : Exception
			where TException2 : Exception
			where TException3 : Exception
		{
			TestArgumentNull(func, "func");
			TestArgumentNull(retryWhen1, "retryWhen1");
			TestArgumentNull(retryWhen2, "retryWhen2");
			TestArgumentNull(retryWhen3, "retryWhen3");

			numberOfTries--;

			do
			{
				try
				{
					return func();
				}
				catch (TException1 ex)
				{
					if (!retryWhen1(ex) || numberOfTries <= 0)
					{
						throw;
					}
					Thread.Sleep(millisecondsSleepBetweenRetry);
				}
				catch (TException2 ex)
				{
					if (!retryWhen2(ex) || numberOfTries <= 0)
					{
						throw;
					}
					Thread.Sleep(millisecondsSleepBetweenRetry);
				}
				catch (TException3 ex)
				{
					if (!retryWhen3(ex) || numberOfTries <= 0)
					{
						throw;
					}
					Thread.Sleep(millisecondsSleepBetweenRetry);
				}
			} while (numberOfTries-- > 0);

			throw new InvalidOperationException("You have found an unexpected path");
		}

	    #region 1 Parameter
        /// <summary>
        /// Retry a Func if a TException is encountered
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <typeparam name="TException">The type of the exception.</typeparam>
        /// <typeparam name="T">Parameter type.</typeparam>
        /// <param name="func">The Func.</param>
        /// <param name="parameter">Value to be used by function</param>
        /// <param name="numberOfTries">The number of tries.</param>
        /// <param name="millisecondsSleepBetweenRetry">The milliseconds sleep between retry.</param>
        /// <returns>TResult</returns>
        /// <example>
        /// 
        /// <code>
        ///   <![CDATA[
        ///   public static bool IsFileAvailable(string fileName)
        ///   {
        ///     if (File.Exists(fileName) == false) 
        ///     {
        ///       throw new IOException("File Not Found: " + fileName); 
        ///     }
        ///     if(fileName.IsFileInUse()) 
        ///     {
        ///       throw new IOException(“File is in use”); 
        ///     }
        ///     return true;
        ///   }
        ///   
        ///   public static bool IsFileAvailable(string fileName, int attempts, int waitBetweenAttempts)
        ///   {
        ///     Func<string, bool> func = IsFileAvailable;
        ///     try{
        ///      return func.Retry<string, bool, IOException>(fileName, attempts, waitBetweenAttempts);
        ///     }
        ///     catch(IOException){
        ///      return false;
        ///     }
        ///   }
        /// ]]>
        /// </code>
        /// </example>
        /// <exception cref="System.ArgumentNullException">thrown when func is null</exception>
        public static TResult Retry<T, TResult, TException>(
               this Func<T, TResult> func,
                T parameter,
               int numberOfTries,
               int millisecondsSleepBetweenRetry)
            where TException : Exception
        {
            TestArgumentNull(func, "func");

            return Retry<T, TResult, TException, TException, TException>(
                func,
                parameter,
                numberOfTries,
                millisecondsSleepBetweenRetry);
        }

        /// <summary>
        /// Retry a func if a TException1 or TException2 is encountered
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <typeparam name="TException1">exception type to catch</typeparam>
        /// <typeparam name="TException2">exception type to catch</typeparam>
        /// <typeparam name="T">Parameter type.</typeparam>
        /// <param name="func">func to execute</param>
        /// <param name="parameter">Value to be used by function</param>
        /// <param name="numberOfTries">number of times to try func</param>
        /// <param name="millisecondsSleepBetweenRetry">milliseconds to
        /// sleep before next retry if exception is encountered</param>
        /// <returns>TResult</returns>
        /// <exception cref="System.ArgumentNullException">thrown when func is null</exception>
        /// <example>
        /// 
        ///   <code>
        ///   <![CDATA[
        ///   public static bool IsFileAvailable(string fileName)
        ///   {
        ///     if (File.Exists(fileName) == false) 
        ///     {
        ///       throw new FileNotFoundException("File Not Found: " + fileName); 
        ///     }
        ///     if(fileName.IsFileInUse()) 
        ///     {
        ///       throw new IOException(“File is in use”); 
        ///     }
        ///     return true;
        ///   }
        ///   
        ///   public static bool IsFileAvailable(string fileName, int attempts, int waitBetweenAttempts)
        ///   {
        ///     FuncRetryWithParameters<string, bool> func = IsFileAvailable;
        ///     try{
        ///      return func.Retry<string, bool, IOException, FileNotFoundException>(fileName, attempts, waitBetweenAttempts);
        ///     }
        ///     catch(IOException){
        ///      return false;
        ///     }
        ///   }
        /// ]]>
        /// </code>
        /// </example>
        public static TResult Retry<T, TResult, TException1, TException2>(
               this Func<T, TResult> func,
                T parameter,
               int numberOfTries,
               int millisecondsSleepBetweenRetry)
            where TException1 : Exception
            where TException2 : Exception
        {
            TestArgumentNull(func, "func");

            return Retry<T, TResult, TException1, TException1, TException2>(
                func,
                parameter,
                numberOfTries,
                millisecondsSleepBetweenRetry);
        }

        /// <summary>
        /// Retry a func if TException1, TException2, TException3 is
        /// encountered
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <typeparam name="TException1">exception type to catch</typeparam>
        /// <typeparam name="TException2">exception type to catch</typeparam>
        /// <typeparam name="TException3">exception type to catch</typeparam>
        /// <typeparam name="T">Parameter type.</typeparam>
        /// <param name="func">func to execute</param>
        /// <param name="parameter">Value to be used by function</param>
        /// <param name="numberOfTries">number of times to try func</param>
        /// <param name="millisecondsSleepBetweenRetry">milliseconds to
        /// sleep before next retry if exception is encountered</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">thrown when func
        /// is null</exception>
        /// <example>
        /// 
        ///   <code>
        ///   <![CDATA[
        ///   public static bool IsFileAvailable(string fileName)
        ///   {
        ///     if (File.Exists(fileName) == false) 
        ///     {
        ///       throw new FileNotFoundException("File Not Found: " + fileName); 
        ///     }
        ///     if(fileName.IsFileInUse()) 
        ///     {
        ///       throw new IOException(“File is in use”); 
        ///     }
        ///     return true;
        ///   }
        ///   
        ///   public static bool IsFileAvailable(string fileName, int attempts, int waitBetweenAttempts)
        ///   {
        ///     Func<string, bool> func = IsFileAvailable;
        ///     try{
        ///      return func.Retry<string, bool, IOException, FileNotFoundException, Exception>(fileName, attempts, waitBetweenAttempts);
        ///     }
        ///     catch(IOException){
        ///      return false;
        ///     }
        ///   }
        /// ]]>
        ///   </code>
        /// </example>
        public static TResult Retry<T, TResult, TException1, TException2, TException3>(
               this Func<T, TResult> func,
               T parameter,
               int numberOfTries,
               int millisecondsSleepBetweenRetry)
            where TException1 : Exception
            where TException2 : Exception
            where TException3 : Exception
        {
            TestArgumentNull(func, "func");

            numberOfTries--;

            do
            {
                try
                {
                    return func(parameter);
                }
                catch (TException1)
                {
                    if (numberOfTries <= 0)
                    {
                        throw;
                    }

                }
                catch (TException2)
                {
                    if (numberOfTries <= 0)
                    {
                        throw;
                    }
                }
                catch (TException3)
                {
                    if (numberOfTries <= 0)
                    {
                        throw;
                    }
                }
                Thread.Sleep(millisecondsSleepBetweenRetry);
            } while (numberOfTries-- > 0);

            throw new InvalidOperationException("You have found an unexpected path");
        }
        #endregion
        #region 2 Parameter
        /// <summary>
        /// Retry a func if a TException is encountered
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <typeparam name="TException">The type of the exception.</typeparam>
        /// <typeparam name="T1">Parameter1 type.</typeparam>
        /// <typeparam name="T2">Parameter2 type.</typeparam>
        /// <param name="parameter1">Parameter1 to func</param>
        /// <param name="parameter2">Parameter12 to func</param>
        /// <param name="func">The func.</param>
        /// <param name="numberOfTries">The number of tries.</param>
        /// <param name="millisecondsSleepBetweenRetry">The milliseconds sleep between retry.</param>
        /// <returns>TResult</returns>
        /// <example>
        /// <code>
        ///   <![CDATA[
        ///   public static byte[] ConvertFile(byte[] fileBytes, FileType newType)
        ///   {
        ///     return Wcf.Service<IConversionService>.Use<byte[]>( (serviceProxy)=>{ return serviceProxy.Convert(fileBytes, newType); } );
        ///   }
        ///   
        ///   public static byte[] ConvertFile(byte[] fileBytes, FileType newType, int attempts, int waitBetweenAttempts)
        ///   {
        ///     Func<string, bool> func = ConvertFile;
        ///     return func.Retry<string, bool, TimeoutException, CommunicationException>(fileName, attempts, waitBetweenAttempts); 
        ///   }
        /// ]]>
        /// </code>
        /// </example>
        /// <exception cref="System.ArgumentNullException">thrown when func is null</exception>
        public static TResult Retry<T1, T2, TResult, TException>(
               this Func<T1, T2, TResult> func,
                T1 parameter1,
                T2 parameter2,
               int numberOfTries,
               int millisecondsSleepBetweenRetry)
            where TException : Exception
        {
            TestArgumentNull(func, "func");

            return Retry<T1, T2, TResult, TException, TException, TException>(
                func,
                parameter1,
                parameter2,
                numberOfTries,
                millisecondsSleepBetweenRetry);
        }

        /// <summary>
        /// Retry a func if a TException1 or TException2 is encountered
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <typeparam name="TException1">exception type to catch</typeparam>
        /// <typeparam name="TException2">exception type to catch</typeparam>
        /// <typeparam name="T1">Parameter1 type.</typeparam>
        /// <typeparam name="T2">Parameter2 type.</typeparam>
        /// <param name="parameter1">Parameter1 to func</param>
        /// <param name="parameter2">Parameter12 to func</param>
        /// <param name="func">func to execute</param>
        /// <param name="numberOfTries">number of times to try func</param>
        /// <param name="millisecondsSleepBetweenRetry">milliseconds to
        /// sleep before next retry if exception is encountered</param>
        /// <returns>TResult</returns>
        /// <exception cref="System.ArgumentNullException">thrown when func is null</exception>
        /// <example>
        ///   <code>
        ///   <![CDATA[
        ///   public static byte[] ConvertFile(byte[] fileBytes, FileType newType)
        ///   {
        ///     return Wcf.Service<IConversionService>.Use<byte[]>( (serviceProxy)=>{ return serviceProxy.Convert(fileBytes, newType); } );
        ///   }
        ///   
        ///   public static byte[] ConvertFile(byte[] fileBytes, FileType newType, int attempts, int waitBetweenAttempts)
        ///   {
        ///     Func<string, bool> func = ConvertFile;
        ///     return func.Retry<string, bool, TimeoutException, CommunicationException>(fileName, attempts, waitBetweenAttempts); 
        ///   }
        /// ]]>
        /// </code>
        /// </example>
        public static TResult Retry<T1, T2, TResult, TException1, TException2>(
               this Func<T1, T2, TResult> func,
                T1 parameter1,
                T2 parameter2,
               int numberOfTries,
               int millisecondsSleepBetweenRetry)
            where TException1 : Exception
            where TException2 : Exception
        {
            TestArgumentNull(func, "func");

            return Retry<T1, T2, TResult, TException1, TException1, TException2>(
                func,
                parameter1,
                parameter2,
                numberOfTries,
                millisecondsSleepBetweenRetry);
        }

        /// <summary>
        /// Retry a func if TException1, TException2, TException3 is
        /// encountered
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <typeparam name="TException1">exception type to catch</typeparam>
        /// <typeparam name="TException2">exception type to catch</typeparam>
        /// <typeparam name="TException3">exception type to catch</typeparam>
        /// <typeparam name="T1">Parameter1 type.</typeparam>
        /// <typeparam name="T2">Parameter2 type.</typeparam>
        /// <param name="parameter1">Parameter1 to func</param>
        /// <param name="parameter2">Parameter12 to func</param>
        /// <param name="func">func to execute</param>
        /// <param name="numberOfTries">number of times to try func</param>
        /// <param name="millisecondsSleepBetweenRetry">milliseconds to
        /// sleep before next retry if exception is encountered</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">thrown when func
        /// is null</exception>
        /// <example>
        ///   <code>
        ///   <![CDATA[
        ///   public static byte[] ConvertFile(byte[] fileBytes, FileType newType)
        ///   {
        ///     return Wcf.Service<IConversionService>.Use<byte[]>( (serviceProxy)=>{ return serviceProxy.Convert(fileBytes, newType); } );
        ///   }
        ///   
        ///   public static byte[] ConvertFile(byte[] fileBytes, FileType newType, int attempts, int waitBetweenAttempts)
        ///   {
        ///     Func<string, bool> func = ConvertFile;
        ///     return func.Retry<string, bool, TimeoutException, CommunicationException, FaultException<RetriableDetail>>(fileName, attempts, waitBetweenAttempts); 
        ///   }
        /// ]]>
        ///   </code>
        ///   </example>
        public static TResult Retry<T1, T2, TResult, TException1, TException2, TException3>(
               this Func<T1, T2, TResult> func,
               T1 parameter1,
               T2 parameter2,
               int numberOfTries,
               int millisecondsSleepBetweenRetry)
            where TException1 : Exception
            where TException2 : Exception
            where TException3 : Exception
        {
            TestArgumentNull(func, "func");

            numberOfTries--;

            do
            {
                try
                {
                    return func(parameter1, parameter2);
                }
                catch (TException1)
                {
                    if (numberOfTries <= 0)
                    {
                        throw;
                    }

                }
                catch (TException2)
                {
                    if (numberOfTries <= 0)
                    {
                        throw;
                    }
                }
                catch (TException3)
                {
                    if (numberOfTries <= 0)
                    {
                        throw;
                    }
                }
                Thread.Sleep(millisecondsSleepBetweenRetry);
            } while (numberOfTries-- > 0);

            throw new InvalidOperationException("You have found an unexpected path");
        }
        #endregion
        #region 3 Parameter
        /// <summary>
        /// Retry a func if a TException is encountered
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <typeparam name="TException">The type of the exception.</typeparam>
        /// <typeparam name="T1">Parameter1 type.</typeparam>
        /// <typeparam name="T2">Parameter2 type.</typeparam>
        /// <typeparam name="T3">Parameter2 type.</typeparam>
        /// <param name="parameter1">Parameter1 to func</param>
        /// <param name="parameter2">Parameter12 to func</param>
        /// <param name="parameter3">Parameter12 to func</param>
        /// <param name="func">The func.</param>
        /// <param name="numberOfTries">The number of tries.</param>
        /// <param name="millisecondsSleepBetweenRetry">The milliseconds sleep between retry.</param>
        /// <returns>TResult</returns>
        /// <example>
        /// <code>
        ///   <![CDATA[
        ///   public static byte[] CalcRate(int creditScore1, int creditScore2, int creditScore3)
        ///   {
        ///     return Wcf.Service<IRateCalculator>.Use<float>( (serviceProxy)=>{ return serviceProxy.GetRate(creditScore1, creditScore2, creditScore3) }; );
        ///   }
        ///   
        ///   public static byte[] ConvertFile(byte[] fileBytes, FileType newType, int attempts, int waitBetweenAttempts)
        ///   {
        ///     Func<string, bool> func = ConvertFile;
        ///     return func.Retry<string, bool, Exception>(fileName, attempts, waitBetweenAttempts); 
        ///   }
        /// ]]>
        /// </code>
        /// </example>
        /// <exception cref="System.ArgumentNullException">thrown when func is null</exception>
        public static TResult Retry<T1, T2, T3, TResult, TException>(
               this Func<T1, T2, T3, TResult> func,
                T1 parameter1,
                T2 parameter2,
                T3 parameter3,
               int numberOfTries,
               int millisecondsSleepBetweenRetry)
            where TException : Exception
        {
            TestArgumentNull(func, "func");

            return Retry<T1, T2, T3, TResult, TException, TException, TException>(
                func,
                parameter1,
                parameter2,
                parameter3,
                numberOfTries,
                millisecondsSleepBetweenRetry);
        }

        /// <summary>
        /// Retry a func if a TException1 or TException2 is encountered
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <typeparam name="TException1">exception type to catch</typeparam>
        /// <typeparam name="TException2">exception type to catch</typeparam>
        /// <typeparam name="T1">Parameter1 type.</typeparam>
        /// <typeparam name="T2">Parameter2 type.</typeparam>
        /// <typeparam name="T3">Parameter2 type.</typeparam>
        /// <param name="parameter1">Parameter1 to func</param>
        /// <param name="parameter2">Parameter12 to func</param>
        /// <param name="parameter3">Parameter12 to func</param>
        /// <param name="func">func to execute</param>
        /// <param name="numberOfTries">number of times to try func</param>
        /// <param name="millisecondsSleepBetweenRetry">milliseconds to
        /// sleep before next retry if exception is encountered</param>
        /// <returns>TResult</returns>
        /// <exception cref="System.ArgumentNullException">thrown when func is null</exception>
        /// <example>
        /// <code>
        ///   <![CDATA[
        ///   public static byte[] CalcRate(int creditScore1, int creditScore2, int creditScore3)
        ///   {
        ///     return Wcf.Service<IRateCalculator>.Use<float>( (serviceProxy)=>{ return serviceProxy.GetRate(creditScore1, creditScore2, creditScore3) }; );
        ///   }
        ///   
        ///   public static byte[] ConvertFile(byte[] fileBytes, FileType newType, int attempts, int waitBetweenAttempts)
        ///   {
        ///     Func<string, bool> func = ConvertFile;
        ///     return func.Retry<string, bool, TimeoutException, CommunicationException>(fileName, attempts, waitBetweenAttempts); 
        ///   }
        /// ]]>
        /// </code>
        /// </example>
        public static TResult Retry<T1, T2, T3, TResult, TException1, TException2>(
               this Func<T1, T2, T3, TResult> func,
                T1 parameter1,
                T2 parameter2,
                T3 parameter3,
               int numberOfTries,
               int millisecondsSleepBetweenRetry)
            where TException1 : Exception
            where TException2 : Exception
        {
            TestArgumentNull(func, "func");

            return Retry<T1, T2, T3, TResult, TException1, TException1, TException2>(
                func,
                parameter1,
                parameter2,
                parameter3,
                numberOfTries,
                millisecondsSleepBetweenRetry);
        }

        /// <summary>
        /// Retry a func if TException1, TException2, TException3 is
        /// encountered
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <typeparam name="TException1">exception type to catch</typeparam>
        /// <typeparam name="TException2">exception type to catch</typeparam>
        /// <typeparam name="TException3">exception type to catch</typeparam>
        /// <typeparam name="T1">Parameter1 type.</typeparam>
        /// <typeparam name="T2">Parameter2 type.</typeparam>
        /// <typeparam name="T3">Parameter2 type.</typeparam>
        /// <param name="parameter1">Parameter1 to func</param>
        /// <param name="parameter2">Parameter12 to func</param>
        /// <param name="parameter3">Parameter12 to func</param>
        /// <param name="func">func to execute</param>
        /// <param name="numberOfTries">number of times to try func</param>
        /// <param name="millisecondsSleepBetweenRetry">milliseconds to
        /// sleep before next retry if exception is encountered</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">thrown when func
        /// is null</exception>
        /// <example>
        /// <code>
        ///   <![CDATA[
        ///   public static byte[] CalcRate(int creditScore1, int creditScore2, int creditScore3)
        ///   {
        ///     return Wcf.Service<IRateCalculator>.Use<float>( (serviceProxy)=>{ return serviceProxy.GetRate(creditScore1, creditScore2, creditScore3) }; );
        ///   }
        ///   
        ///   public static byte[] ConvertFile(byte[] fileBytes, FileType newType, int attempts, int waitBetweenAttempts)
        ///   {
        ///     Func<string, bool> func = ConvertFile;
        ///     return func.Retry<string, bool, TimeoutException, CommunicationException, FaultException<RetriableException>>(fileName, attempts, waitBetweenAttempts); 
        ///   }
        /// ]]>
        /// </code>
        ///   </example>
        public static TResult Retry<T1, T2, T3, TResult, TException1, TException2, TException3>(
               this Func<T1, T2, T3, TResult> func,
               T1 parameter1,
               T2 parameter2,
               T3 parameter3,
               int numberOfTries,
               int millisecondsSleepBetweenRetry)
            where TException1 : Exception
            where TException2 : Exception
            where TException3 : Exception
        {
            TestArgumentNull(func, "func");

            numberOfTries--;

            do
            {
                try
                {
                    return func(parameter1, parameter2, parameter3);
                }
                catch (TException1)
                {
                    if (numberOfTries <= 0)
                    {
                        throw;
                    }

                }
                catch (TException2)
                {
                    if (numberOfTries <= 0)
                    {
                        throw;
                    }
                }
                catch (TException3)
                {
                    if (numberOfTries <= 0)
                    {
                        throw;
                    }
                }
                Thread.Sleep(millisecondsSleepBetweenRetry);
            } while (numberOfTries-- > 0);

            throw new InvalidOperationException("You have found an unexpected path");
        }
        #endregion

		private static void TestArgumentNull(object argument, string argumentName)
		{
			if (argument == null)
			{
				throw new ArgumentNullException(argumentName);
			}
		}

	}
}
