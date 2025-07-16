using System;
using System.Collections.Generic;

namespace F1Solutions.Naati.Common.Contracts.Dal.Response
{
    public class GetArchiveHistoryResponse
    {
        public IEnumerable<DateTime> ArchiveHistory { get; set; }
    }
}