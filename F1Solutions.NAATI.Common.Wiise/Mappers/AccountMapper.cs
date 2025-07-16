using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static F1Solutions.Naati.Common.Wiise.PublicModels.Account;

namespace F1Solutions.Naati.Common.Wiise.Mappers
{
    internal static class AccountMapper
    {
        internal static PublicModels.Accounts ToPublicModelAccounts(this NativeModels.Accounts accounts)
        {
            return new PublicModels.Accounts()
            {
                _Accounts = accounts._Accounts.Select(x => x.ToPublicModelAccount()).ToList()
            };
        }

        internal static PublicModels.Account ToPublicModelAccount(this NativeModels.Account account)
        {
            return new PublicModels.Account()
            {
                //AccountID = account.id,
                BankAccountNumber = account.Number,
                Description = account.DisplayName,
                Name = account.DisplayName,
                BankAccountType = account.AccountType.ToBankAccountType(),
            };
        }

        internal static BankAccountTypeEnum ToBankAccountType(this string acccount)
        {
            switch(acccount)
            {
                case "Assets":
                case "Inventory":
                    return BankAccountTypeEnum.NONE;
                default:
                    return BankAccountTypeEnum.NONE;
            }
        }
    }
}
