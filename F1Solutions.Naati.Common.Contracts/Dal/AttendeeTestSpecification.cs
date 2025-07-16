using System.Collections.Generic;
using F1Solutions.Naati.Common.Contracts.Dal.DTO;

namespace F1Solutions.Naati.Common.Contracts.Dal
{
    public class AttendeeTestSpecification
    {
        public int Id { get; set; }
        public List<StoredFileMarterialDto> StoredFileList { get; set; }
    }
}