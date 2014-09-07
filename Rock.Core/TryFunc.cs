namespace Rock
{
    public delegate bool TryFunc<TResult>(out TResult result);
    public delegate bool TryFunc<in T, TResult>(T input, out TResult result);
    public delegate bool TryFunc<in T1, in T2, TResult>(T1 input1, T2 input2, out TResult result);
    public delegate bool TryFunc<in T1, in T2, in T3, TResult>(T1 input1, T2 input2, T3 input3, out TResult result);
    public delegate bool TryFunc<in T1, in T2, in T3, in T4, TResult>(T1 input1, T2 input2, T3 input3, T4 input4, out TResult result);
}
