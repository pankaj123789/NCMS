using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace F1Solutions.Naati.Common.Contracts.Dal.DTO
{
    public class NotificationDownloadTestMaterialParameterDto
    {
        public virtual string Path { get; set; }
        public virtual int TestSessionId { get; set; }
        public virtual string TestSessionName { get; set; }
    }
    public class NotificationDownloadTestMaterialParameterSanitizedDto : NotificationDownloadTestMaterialParameterDto
    {
        [JsonIgnore]
        public override string Path { get; set; }
    }

    public class ErrorMessageParameterDto 
    {
        public virtual string Content { get; set; }
        public virtual string Exception { get; set; }
    }
    public class ErrorMessageParameterSanitizedDto : ErrorMessageParameterDto
    {
        [JsonIgnore]
        public override string Exception { get; set; }
    }

}
