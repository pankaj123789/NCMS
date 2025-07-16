using System.Collections.Generic;

namespace MyNaati.Ui.AutoMapperRegistry
{
    public interface IMapper<TSource, TDestination>
    {
        TDestination Map(TSource source);
        List<TDestination> Map(IEnumerable<TSource> list);
        TSource MapInverse(TDestination source);
        List<TSource> MapInverse(IEnumerable<TDestination> list);
    }
}