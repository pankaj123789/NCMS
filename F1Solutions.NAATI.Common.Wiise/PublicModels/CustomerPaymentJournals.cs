using System.Collections.Generic;

namespace F1Solutions.Naati.Common.Wiise.PublicModels
{
    public class CustomerPaymentJournals
    {
        public string OdataContext { get; set; }
        public List<CustomerPaymentJournal> _CustomerPaymentJournals { get; set; }
    }
}
