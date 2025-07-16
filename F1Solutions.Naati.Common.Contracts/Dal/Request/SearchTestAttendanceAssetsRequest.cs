using System;
using System.Collections.Generic;

namespace F1Solutions.Naati.Common.Contracts.Dal.Request
{
    public class SearchTestAttendanceAssetsRequest
    {
        public IEnumerable<int> TestAttendanceId { get; set; }
        public IEnumerable<int> TestMaterialId { get; set; }
        public IEnumerable<int> JobId { get; set; }
        public IEnumerable<int> NaatiNumber { get; set; }
        public IEnumerable<int> OfficeId { get; set; }
        public IEnumerable<string> TestAttendanceAssetType { get; set; }
        public IEnumerable<int> UploadedByUserId { get; set; }
        public IEnumerable<int> UploadedByPersonNaatiNo { get; set; }
        public DateTime? SatDateFrom { get; set; }
        public DateTime? SatDateTo { get; set; }
        public bool IncludeDeleted { get; set; }
    }
}