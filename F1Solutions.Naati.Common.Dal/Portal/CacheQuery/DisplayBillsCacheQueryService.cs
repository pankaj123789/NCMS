using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using F1Solutions.Naati.Common.Contracts.Dal;
using F1Solutions.Naati.Common.Contracts.Dal.Enum;
using F1Solutions.Naati.Common.Contracts.Dal.Portal.CacheQuery;
using F1Solutions.Naati.Common.Contracts.Dal.QueryServices;
using F1Solutions.Naati.Common.Contracts.Dal.Request;
using F1Solutions.Naati.Common.Dal.CacheQuery;
using F1Solutions.Naati.Common.Dal.Domain;
using F1Solutions.Naati.Common.Dal.NHibernate.Configuration;
using F1Solutions.Naati.Common.Dal.QueryHelper;

namespace F1Solutions.Naati.Common.Dal.Portal.CacheQuery
{
    public class DisplayBillsCacheQueryService : BaseLazyCacheQueryService<int, bool, bool>, IDisplayBillsCacheQueryService
    {
        public bool IsDisplayBills(int naatiNumber)
        {
            return GetItem(naatiNumber);
        }

        public void RefreshDisplayBillsFlag(int naatiNumber)
        {
            RefreshItem(naatiNumber);
        }

        protected override bool GetDbSingleValue(int key)
        {
            var statusesToShow = new[]
            {   ((int)CredentialApplicationStatusTypeName.ProcessingSubmission),
                ((int)CredentialApplicationStatusTypeName.Entered),
                ((int)CredentialApplicationStatusTypeName.AwaitingApplicationPayment),
                ((int)CredentialApplicationStatusTypeName.AwaitingAssessmentPayment),
                ((int)CredentialApplicationStatusTypeName.BeingChecked),
                ((int)CredentialApplicationStatusTypeName.InProgress),
                ((int)CredentialApplicationStatusTypeName.Completed)
            };

            var displayBills = NHibernateSession.Current.Query<CredentialApplication>().Count(
                x => (x.Person.Entity.NaatiNumber == key)
                     && x.CredentialApplicationType.DisplayBills
                     && statusesToShow.Contains(x.CredentialApplicationStatusType.Id))> 0;

            return displayBills;
        }

        protected override bool MapToTResultType(bool item) => item;

        protected override int TransformKey(int key) => key;
    }
}
