using System;

namespace F1Solutions.Naati.Common.Contracts.Dal.DTO
{
    public class PaymentMethodTypeModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public Guid ExternalExternalReferenceId { get; set; }
    }
}
