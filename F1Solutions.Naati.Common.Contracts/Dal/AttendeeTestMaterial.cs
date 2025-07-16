using System.Collections.Generic;
using F1Solutions.Naati.Common.Contracts.Dal.DTO;

namespace F1Solutions.Naati.Common.Contracts.Dal
{
    public class AttendeeTestMaterial
    {
        public int Id { get; set; }
        public string TaskTypeLabel { get; set; }
        public int TestComponentNumber { get; set; }
        public string Label { get; set; }
        public List<StoredFileMarterialDto> StoredFileList { get; set; }
    }
}