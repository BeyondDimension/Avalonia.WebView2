namespace System;

static class ConvertibleHelper
{
    public static TOut Convert<TOut, TIn>(TIn value)
    {
        var parameter = Expression.Parameter(typeof(TIn));
        var dynamicMethod = Expression.Lambda<Func<TIn, TOut>>(
            Expression.Convert(parameter, typeof(TOut)),
            parameter);
        return dynamicMethod.Compile()(value);
    }
}
