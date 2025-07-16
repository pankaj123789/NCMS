using System;

namespace F1Solutions.Naati.Common.Dal.Domain
{
    public class ExternalAccountingOperation : EntityBase
    {
        public virtual User RequestedByUser { get; set; }
        public virtual DateTime RequestedDateTime { get; set; }
        public virtual ExternalAccountingOperationType Type { get; set; }
        public virtual string InputType { get; set; }
        public virtual string Reference { get; set; }
        public virtual string Input { get; set; }
        public virtual string Output { get; set; }
        public virtual ExternalAccountingOperationStatus Status { get; set; }
        public virtual DateTime? ProcessedDateTime { get; set; }
        public virtual string Message { get; set; }
        public virtual string Exception { get; set; }
        public virtual ExternalAccountingOperation PrerequisiteOperation { get; set; }
        public virtual string CompletionType { get; set; }
        public virtual string CompletionInput { get; set; }
        public virtual string Description { get; set; }
        public virtual bool BatchProcess { get; set; }

        public override IAuditObject RootAuditObject => this;
    }
}