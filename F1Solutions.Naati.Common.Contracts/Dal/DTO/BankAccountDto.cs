using System;

namespace F1Solutions.Naati.Common.Contracts.Dal.DTO
{
    public class BankAccountDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public bool? EnablePaymentsToAccount { get; set; }
    }
}