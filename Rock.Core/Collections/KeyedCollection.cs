namespace Rock.Collections
{
    public abstract class KeyedCollection<TKey, TItem>
        : System.Collections.ObjectModel.KeyedCollection<TKey, TItem>,
          IKeyedEnumerable<TKey, TItem>
    {
    }
}