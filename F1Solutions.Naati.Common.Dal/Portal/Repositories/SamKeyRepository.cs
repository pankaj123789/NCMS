using System;
using System.Data.Linq;
using System.Linq;
using System.Threading;
using System.Transactions;
using F1Solutions.Global.Common.Logging;
//using F1Solutions.NAATI.ePortal.SAMIntegration.Data;

namespace F1Solutions.Naati.Common.Dal.Portal.Repositories
{
    //public interface ISamKeyRepository
    //{
    //    int[] AllocateKeys(string tableName);
    //}

    //public class SamKeyRepository : ISamKeyRepository
    //{
    //    private const int MaxAttempts = 1;

    //    private SamKeyGenerationDataContext mDataContext;
    //    public SamKeyRepository(SamKeyGenerationDataContext context)
    //    {
    //        mDataContext = context;
    //    }

    //    public int[] AllocateKeys(string tableName)
    //    {
    //        int attemptCount = 0;
    //        while (true)
    //        {
    //            try
    //            {
    //                using (var scope = new TransactionScope(TransactionScopeOption.RequiresNew))
    //                {
    //                    var tableData = mDataContext.TableDatas.Single(t => t.TableName == tableName);
    //                    Thread.Sleep(5000);
    //                    //Ensure this gets the latest row version; SAM might have updated it, and the line above probably just retrieved from the DataContext cache.
    //                    mDataContext.Refresh(RefreshMode.OverwriteCurrentValues, tableData);

    //                    var resultKeys = Enumerable.Range(tableData.NextKey, tableData.AllocateQuantity).ToArray();
    //                    tableData.NextKey += tableData.AllocateQuantity;

    //                    mDataContext.SubmitChanges();
    //                    scope.Complete();

    //                    return resultKeys;
    //                }
    //            }
    //            catch (Exception ex)
    //            {
    //                var limitReached = (attemptCount >= MaxAttempts);

    //                LoggingHelper.LogException(ex, $"Could not allocate new keys for tableName '{tableName}'. {(limitReached ? "FAILING" : "RETRYING")}.");

    //                if (attemptCount >= MaxAttempts)
    //                {
    //                    throw;
    //                }

    //                attemptCount++;
    //            }
    //        }
    //    }
    //}
}
