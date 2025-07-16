using F1Solutions.Naati.Common.Contracts.Dal.Enum;

namespace Ncms.Contracts.Models.Application
{
    public class ValidationResultModel
    {
        public string Field { get; set; }
        public string Message { get; set; }
    }

    public class ActionEventLevel
    {
        public SystemActionEventTypeName Event { get; set; }
        public int Level { get; set; }
    }
}