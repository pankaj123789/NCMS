using System;

namespace F1Solutions.Naati.Common.Contracts.Dal
{
    public class MyNaatiAccountDetails
    {
        public int NaatiNumber { get; set; }
        public bool? Active { get; set; }
        public DateTime? DateCreated { get; set; }
        public DateTime? LastUpdate { get; set; }

    }
}