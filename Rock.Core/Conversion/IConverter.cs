namespace Rock.Conversion
{
    /// <summary>
    /// Provides a mechanism for converting an <see cref="object"/> to a target type, <typeparamref name="TTarget"/>.
    /// </summary>
    /// <typeparam name="TTarget">The type to be converted to.</typeparam>
    public interface IConverter<out TTarget>
    {
        /// <summary>
        /// Convert the <see cref="object"/> to type <typeparamref name="TTarget"/>. An exception may be thrown if 
        /// <paramref name="obj"/> is not compatible with type <typeparamref name="TTarget"/>.
        /// </summary>
        /// <param name="obj">An object to convert.</param>
        /// <returns>
        /// An instance of <typeparamref name="TTarget"/> that represents the <paramref name="obj"/> parameter.
        /// </returns>
        TTarget Convert(object obj);
    }
}