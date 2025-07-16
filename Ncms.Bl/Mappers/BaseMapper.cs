using System.Collections.Generic;
using System.Linq;

namespace Ncms.Bl.Mappers
{
    public abstract class BaseMapper<TSource, TDestination> : IMapper<TSource, TDestination>
    {
        public virtual List<TDestination> Map(IEnumerable<TSource> list)
        {
            return list?.Select(Map).ToList();
        }

        public abstract TDestination Map(TSource source);

        public virtual List<TSource> MapInverse(IEnumerable<TDestination> list)
        {
            return list?.Select(MapInverse).ToList();
        }

        public abstract TSource MapInverse(TDestination source);
    }
}
