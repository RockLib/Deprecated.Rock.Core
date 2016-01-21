using System;
using System.Threading;
using System.Threading.Tasks;

namespace Rock.Extensions
{
	/// <remarks>
	/// FuncExtensions does not contain any async retry extensions for void asynchronous delegates.
	/// Those recommended to only be async event handlers, so no real reason to need to retry
	/// </remarks>
	public static partial class FuncExtensions
	{
		/// <summary>
		/// Try an asynchronous delegate up to <paramref name="retryCount"/> times with <paramref name="millisecondRetryDelay"/> ms delay between retries.
		/// </summary>
		/// <typeparam name="TResult">The type of object the delegate returns.</typeparam>
		/// <param name="func">The asynchonous delegate to retry.</param>
		/// <param name="retryCount">Maximum number of retries.</param>
		/// <param name="millisecondRetryDelay">Millisecond delay between retries.</param>
		/// <returns>A task that can be awaited</returns>
		/// <example>
		/// <code>
		///   <![CDATA[
		///   var client = new RestClient();
		///   var request = new RestRequest("http://www.google.com");
		///   
		///   Func<Task<IRestResponse>> func = () => client.ExecuteTaskAsync(request);
		///   var restResponse = await func.RetryAsync(3, 500);
		/// ]]>
		/// </code>
		/// </example>
		public static async Task<TResult> RetryAsync<TResult>(this Func<Task<TResult>> func, int retryCount, int millisecondRetryDelay)
		{
			retryCount--;
			do
			{
				try
				{
					return await func();
				}
				catch
				{
					if (retryCount == 0)
						throw;
				}
				Thread.Sleep(millisecondRetryDelay);
			} while (retryCount-- > 0);

			throw new InvalidOperationException("You have found an unexpected path");
		}

		/// <summary>
		/// Try an asynchronous delegate up to <paramref name="retryCount"/> times with <paramref name="millisecondRetryDelay"/> ms delay between retries,
		/// when one of the following exceptions occur: <typeparamref name="TException1"/>, <typeparamref name="TException1"/>, <typeparamref name="TException1"/>
		/// </summary>
		/// <typeparam name="TException1">The first exception to catch</typeparam>
		/// <typeparam name="TException2">The second exception to catch, note if this exception is derived from <typeparamref name="TException1"/> it will never be handled.</typeparam>
		/// <typeparam name="TException3">The third exception to catch, note if this exception is derived from <typeparamref name="TException2"/> it will never be handled.</typeparam>
		/// <typeparam name="TResult"></typeparam>
		/// <param name="func">The asynchonous delegate to retry.</param>
		/// <param name="retryCount">Maximum number of retries.</param>
		/// <param name="millisecondRetryDelay">Millisecond delay between retries.</param>
		/// <returns><typeparamref name="TResult"/></returns>
		/// <example>
		/// <code>
		///   <![CDATA[
		/// string text;
		/// var sendAsync = new Func<Task<string>>(async () =>
		/// {
		/// 	using (var client = new HttpClient())
		/// 	using (var request = new HttpRequestMessage {RequestUri = new Uri("http://google.com"), Method = HttpMethod.Get})
		/// 	{
		/// 		var response = await client.SendAsync(request);
		/// 		return await response.Content.ReadAsStringAsync();
		/// 
		/// 	}
		/// });
		/// 
		/// text = await sendAsync.RetryAsync<HttpRequestException, TaskCanceledException, HttpResponseException, string>(3,500);
		/// ]]>
		/// </code>
		/// </example>
		public static async Task<TResult> RetryAsync<TException1, TException2, TException3, TResult>(this Func<Task<TResult>> func, int retryCount, int millisecondRetryDelay)
			where TException1 : Exception
			where TException2 : Exception
			where TException3 : Exception
		{
			retryCount--;
			do
			{
				try
				{
					return await func();
				}
				catch (TException1)
				{
					if (retryCount <= 0)
						throw;
				}
				catch (TException2)
				{
					if (retryCount <= 0)
						throw;
				}
				catch (TException3)
				{
					if (retryCount <= 0)
						throw;
				}
				Thread.Sleep(millisecondRetryDelay);
			} while (retryCount-- > 0);

			throw new InvalidOperationException("You have found an unexpected path");
		}

		/// <summary>
		/// Try an asynchronous delegate up to <paramref name="retryCount"/> times with <paramref name="millisecondRetryDelay"/> ms delay between retries,
		/// when one of the following exceptions occur: <typeparamref name="TException1"/>, <typeparamref name="TException2"/>
		/// </summary>
		/// <typeparam name="TException1">The first exception to catch</typeparam>
		/// <typeparam name="TException2">The second exception to catch, note if this exception is derived from <typeparamref name="TException1"/> it will never be handled.</typeparam>
		/// <typeparam name="TResult"></typeparam>
		/// <param name="func">The asynchonous delegate to retry.</param>
		/// <param name="retryCount">Maximum number of retries.</param>
		/// <param name="millisecondRetryDelay">Millisecond delay between retries.</param>
		/// <returns><typeparamref name="TResult"/></returns>
		/// <example>
		/// <code>
		///   <![CDATA[
		/// string text;
		/// var sendAsync = new Func<Task<string>>(async () =>
		/// {
		/// 	using (var client = new HttpClient())
		/// 	using (var request = new HttpRequestMessage {RequestUri = new Uri("http://google.com"), Method = HttpMethod.Get})
		/// 	{
		/// 		var response = await client.SendAsync(request);
		/// 		return await response.Content.ReadAsStringAsync();
		/// 
		/// 	}
		/// });
		/// 
		/// text = await sendAsync.RetryAsync<HttpRequestException, TaskCanceledException, string>(3,500);
		/// ]]>
		/// </code>
		/// </example>
		public static Task<TResult> RetryAsync<TException1, TException2, TResult>(this Func<Task<TResult>> func, int retryCount, int millisecondRetryDelay)
			where TException1 : Exception
			where TException2 : Exception
		{
			return RetryAsync<TException1, TException2, TException2, TResult>(func, retryCount, millisecondRetryDelay);
		}

		/// <summary>
		/// Try an asynchronous delegate up to <paramref name="retryCount"/> times with <paramref name="millisecondRetryDelay"/> ms delay between retries,
		/// when the following exception occurs: <typeparamref name="TException1"/>
		/// </summary>
		/// <typeparam name="TException1">The exception to catch</typeparam>
		/// <typeparam name="TResult"></typeparam>
		/// <param name="func">The asynchonous delegate to retry.</param>
		/// <param name="retryCount">Maximum number of retries.</param>
		/// <param name="millisecondRetryDelay">Millisecond delay between retries.</param>
		/// <returns><typeparamref name="TResult"/></returns>
		/// <example>
		/// <code>
		///   <![CDATA[
		/// string text;
		/// var sendAsync = new Func<Task<string>>(async () =>
		/// {
		/// 	using (var client = new HttpClient())
		/// 	using (var request = new HttpRequestMessage {RequestUri = new Uri("http://google.com"), Method = HttpMethod.Get})
		/// 	{
		/// 		var response = await client.SendAsync(request);
		/// 		return await response.Content.ReadAsStringAsync();
		/// 
		/// 	}
		/// });
		/// 
		/// text = await sendAsync.RetryAsync<HttpRequestException, string>(3,500);
		/// ]]>
		/// </code>
		/// </example>
		public static Task<TResult> RetryAsync<TException1, TResult>(this Func<Task<TResult>> func, int retryCount, int millisecondRetryDelay)
			where TException1 : Exception
		{
			return RetryAsync<TException1, TException1, TException1, TResult>(func, retryCount, millisecondRetryDelay);
		}

		/// <summary>
		/// Try an asynchronous delegate up to <paramref name="retryCount"/> times with <paramref name="millisecondRetryDelay"/> ms delay between retries
		/// when exceptions occur.
		/// </summary>
		/// <typeparam name="TArg">Argument type to be passed to the delegate</typeparam>
		/// <typeparam name="TResult"></typeparam>
		/// <param name="func">The asynchonous delegate to retry.</param>
		/// <param name="arg1">The argument</param>
		/// <param name="retryCount">Maximum number of retries.</param>
		/// <param name="millisecondRetryDelay">Millisecond delay between retries.</param>
		/// <code>
		///   <![CDATA[
		/// string text;
		/// var uri = new Uri("http://google.asdfcom");
		/// var sendAsync = new Func<Uri, Task<string>>(async u =>
		/// {
		/// 	using (var client = new HttpClient())
		/// 	using (
		/// 		var request = new HttpRequestMessage {RequestUri = u, Method = HttpMethod.Get})
		/// 	{
		/// 		var response = await client.SendAsync(request);
		/// 		return await response.Content.ReadAsStringAsync();
		/// 
		/// 	}
		/// });
		/// text = await sendAsync.RetryAsync(uri, 3,500);
		/// ]]>
		/// </code>
		/// <returns>TResult</returns>
		public static async Task<TResult> RetryAsync<TArg, TResult>(this Func<TArg, Task<TResult>> func, TArg arg1, int retryCount, int millisecondRetryDelay)
		{
			retryCount--;
			do
			{
				try
				{
					return await func(arg1);
				}
				catch
				{
					if (retryCount == 0)
						throw;
				}
				Thread.Sleep(millisecondRetryDelay);
			} while (retryCount-- > 0);

			throw new InvalidOperationException("You have found an unexpected path");
		}

		/// <summary>
		/// Try an asynchronous delegate up to <paramref name="retryCount"/> times with <paramref name="millisecondRetryDelay"/> ms delay between retries,
		/// when one of the following exceptions occur: <typeparamref name="TException1"/>, <typeparamref name="TException2"/>
		/// </summary>
		/// <typeparam name="TArg">Argument type to be passed to the delegate</typeparam>
		/// <typeparam name="TException1">The first exception to catch</typeparam>
		/// <typeparam name="TException2">The second exception to catch, note if this exception is derived from <typeparamref name="TException1"/> it will never be handled.</typeparam>
		/// <typeparam name="TResult"></typeparam>
		/// <param name="func">The asynchonous delegate to retry.</param>
		/// <param name="arg1">The argument</param>
		/// <param name="retryCount">Maximum number of retries.</param>
		/// <param name="millisecondRetryDelay">Millisecond delay between retries.</param>
		/// <code>
		///   <![CDATA[
		/// string text;
		/// var uri = new Uri("http://google.asdfcom");
		/// var sendAsync = new Func<Uri, Task<string>>(async u =>
		/// {
		/// 	using (var client = new HttpClient())
		/// 	using (
		/// 		var request = new HttpRequestMessage {RequestUri = u, Method = HttpMethod.Get})
		/// 	{
		/// 		var response = await client.SendAsync(request);
		/// 		return await response.Content.ReadAsStringAsync();
		/// 
		/// 	}
		/// });
		/// text = await sendAsync.RetryAsync<Uri, HttpRequestException, TaskCanceledException, HttpResponseException, string>(uri, 3,500);
		/// ]]>
		/// </code>
		/// <returns>TResult</returns>
		public static async Task<TResult> RetryAsync<TArg, TException1, TException2, TException3, TResult>(this Func<TArg, Task<TResult>> func, TArg arg1, int retryCount, int millisecondRetryDelay)
			where TException1 : Exception
			where TException2 : Exception
			where TException3 : Exception
		{
			retryCount--;
			do
			{
				try
				{
					return await func(arg1);
				}
				catch (TException1)
				{
					if (retryCount <= 0)
						throw;
				}
				catch (TException2)
				{
					if (retryCount <= 0)
						throw;
				}
				catch (TException3)
				{
					if (retryCount <= 0)
						throw;
				}
				Thread.Sleep(millisecondRetryDelay);
			} while (retryCount-- > 0);

			throw new InvalidOperationException("You have found an unexpected path");
		}

		/// <summary>
		/// Try an asynchronous delegate up to <paramref name="retryCount"/> times with <paramref name="millisecondRetryDelay"/> ms delay between retries,
		/// when one of the following exceptions occur: <typeparamref name="TException1"/>, <typeparamref name="TException2"/>
		/// </summary>
		/// <typeparam name="TArg">Argument type to be passed to the delegate</typeparam>
		/// <typeparam name="TException1">The first exception to catch</typeparam>
		/// <typeparam name="TException2">The second exception to catch, note if this exception is derived from <typeparamref name="TException1"/> it will never be handled.</typeparam>
		/// <typeparam name="TResult"></typeparam>
		/// <param name="func">The asynchonous delegate to retry.</param>
		/// <param name="arg1">The argument</param>
		/// <param name="retryCount">Maximum number of retries.</param>
		/// <param name="millisecondRetryDelay">Millisecond delay between retries.</param>
		/// <code>
		///   <![CDATA[
		/// string text;
		/// var uri = new Uri("http://google.asdfcom");
		/// var sendAsync = new Func<Uri, Task<string>>(async u =>
		/// {
		/// 	using (var client = new HttpClient())
		/// 	using (
		/// 		var request = new HttpRequestMessage {RequestUri = u, Method = HttpMethod.Get})
		/// 	{
		/// 		var response = await client.SendAsync(request);
		/// 		return await response.Content.ReadAsStringAsync();
		/// 
		/// 	}
		/// });
		/// text = await sendAsync.RetryAsync<Uri, HttpRequestException, TaskCanceledException, string>(uri, 3,500);
		/// ]]>
		/// </code>
		/// <returns>TResult</returns>
		public static Task<TResult> RetryAsync<TArg, TException1, TException2, TResult>(this Func<TArg, Task<TResult>> func, TArg arg1, int retryCount, int millisecondRetryDelay)
			where TException1 : Exception
			where TException2 : Exception
		{
			return RetryAsync<TArg, TException1, TException2, TException2, TResult>(func, arg1, retryCount, millisecondRetryDelay);
		}

		/// <summary>
		/// Try an asynchronous delegate up to <paramref name="retryCount"/> times with <paramref name="millisecondRetryDelay"/> ms delay between retries,
		/// when the following exception occurs: <typeparamref name="TException1"/>
		/// </summary>
		/// <typeparam name="TArg">Argument type to be passed to the delegate</typeparam>
		/// <typeparam name="TException1">The exception to catch</typeparam>
		/// <typeparam name="TResult"></typeparam>
		/// <param name="func">The asynchonous delegate to retry.</param>
		/// <param name="arg1">The argument</param>
		/// <param name="retryCount">Maximum number of retries.</param>
		/// <param name="millisecondRetryDelay">Millisecond delay between retries.</param>
		/// <code>
		///   <![CDATA[
		/// string text;
		/// var uri = new Uri("http://google.asdfcom");
		/// var sendAsync = new Func<Uri, Task<string>>(async u =>
		/// {
		/// 	using (var client = new HttpClient())
		/// 	using (
		/// 		var request = new HttpRequestMessage {RequestUri = u, Method = HttpMethod.Get})
		/// 	{
		/// 		var response = await client.SendAsync(request);
		/// 		return await response.Content.ReadAsStringAsync();
		/// 
		/// 	}
		/// });
		/// text = await sendAsync.RetryAsync<Uri, HttpRequestException, string>(uri, 3,500);
		/// ]]>
		/// </code>
		/// <returns>TResult</returns>
		public static Task<TResult> RetryAsync<TArg, TException1, TResult>(this Func<TArg, Task<TResult>> func, TArg arg1, int retryCount, int millisecondRetryDelay)
			where TException1 : Exception
		{
			return RetryAsync<TArg, TException1, TException1, TException1, TResult>(func, arg1, retryCount, millisecondRetryDelay);
		}
	}
}
