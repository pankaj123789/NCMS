using System.Linq;

namespace F1Solutions.Naati.Common.Wiise.Mappers
{
    internal static class CustomerPaymentJournalMapper
    {
        internal static NativeModels.CustomerPaymentJournals ToNativeCustomerPaymentJournals(this PublicModels.CustomerPaymentJournals customerPaymentJournals)
        {
            return new NativeModels.CustomerPaymentJournals
            {
                _CustomerPaymentJournals = customerPaymentJournals._CustomerPaymentJournals.Select(customerPaymentJournal => ToNativeCustomerPaymentJournal(customerPaymentJournal)).ToList()
            };

        }

        internal static NativeModels.CustomerPaymentJournal ToNativeCustomerPaymentJournal(this PublicModels.CustomerPaymentJournal customerPaymentJournal)
        {
            return new NativeModels.CustomerPaymentJournal
            {
                BalancingAccountId = customerPaymentJournal.BalancingAccountId,
                BalancingAccountNumber = customerPaymentJournal.BalancingAccountNumber,
                Code = customerPaymentJournal.Code, 
                DisplayName = customerPaymentJournal.DisplayName, 
                Id = customerPaymentJournal.Id
            };
        }

        internal static PublicModels.CustomerPaymentJournals ToPublicCustomerPaymentJournals(this NativeModels.CustomerPaymentJournals customerPaymentJournals)
        {
            return new PublicModels.CustomerPaymentJournals()
            {
                _CustomerPaymentJournals = customerPaymentJournals._CustomerPaymentJournals.Select(x => x.ToPublicCustomerPaymentJournal()).ToList()
            };
        }

        internal static PublicModels.CustomerPaymentJournal ToPublicCustomerPaymentJournal(this NativeModels.CustomerPaymentJournal customerPaymentJournal)
        {
            return new PublicModels.CustomerPaymentJournal()
            {
                BalancingAccountId = customerPaymentJournal.BalancingAccountId,
                BalancingAccountNumber = customerPaymentJournal.BalancingAccountNumber,
                Code = customerPaymentJournal.Code,
                DisplayName = customerPaymentJournal.DisplayName,
                Id = customerPaymentJournal.Id
            };
        }
    }
}
