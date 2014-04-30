namespace Rock
{
    public interface IConvertObjectsTo<TTarget>
    {
        TTarget Convert(object @object);
    }
}