using System;

namespace Rock.IO
{
    public interface IExpirableBucketItem : IBucketItem
    {
        void SetValue<T>(T value, TimeSpan? expiry);
        void Touch();
    }
}