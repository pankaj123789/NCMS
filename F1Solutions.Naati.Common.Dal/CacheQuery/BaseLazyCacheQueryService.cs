using System.Collections.Concurrent;
using System.Collections.Generic;

namespace F1Solutions.Naati.Common.Dal.CacheQuery
{
    public abstract class BaseLazyCacheQueryService<TKeyType, TResultType, TDataType> : BaseCacheQueryService<TKeyType, TResultType, TDataType>
    {
        public override TResultType GetItem(TKeyType key)
        {
            if (HasItem(key))
            {
                return base.GetItem(key);
            }

            var transformedKey = TransformKey(key);
            if (transformedKey == null)
            {
                return default;
            }

            var value = GetDbSingleValue(transformedKey);
            if (value == null)
            {
                return default;
            }

            Items[transformedKey] = MapToTResultType(value);

            return Items[transformedKey];
        }

        protected override IReadOnlyList<KeyValuePair<TKeyType, TResultType>> GetAllDbValues()
        {
            return new List<KeyValuePair<TKeyType, TResultType>>();
        }

        protected override bool RefreshItem(TKeyType key)
        {
            var transformedKey = TransformKey(key);

            if (transformedKey == null)
            {
                return false;
            }

            Items.Remove(transformedKey);
            return true;
        }
    }
}
