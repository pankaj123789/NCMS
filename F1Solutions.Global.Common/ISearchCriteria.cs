using System.Collections.Generic;

namespace F1Solutions.Global.Common
{
    public interface ISearchCriteria<TS>
    {
        TS Filter { get; set; }
        IEnumerable<string> Values { get; set; }
    }
}
