using System;

namespace Ncms.Contracts.Models.Accounting
{
    public class BankAcountModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public bool? EnablePaymentsToAccount { get; set; }
    }
}
