using System;

namespace F1Solutions.Naati.Common.Dal.Domain
{
    public class PaymentMethodType : EntityBase
    {
        public virtual string Name { get; set; }
        public virtual string DisplayName { get; set; }
        public virtual Guid ExternalReferenceId { get; set; }
    }
}
