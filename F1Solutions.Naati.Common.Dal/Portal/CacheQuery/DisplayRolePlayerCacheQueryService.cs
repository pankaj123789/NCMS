using System;
using System.Linq;
using F1Solutions.Naati.Common.Contracts.Dal.Enum;
using F1Solutions.Naati.Common.Contracts.Dal.Portal.CacheQuery;
using F1Solutions.Naati.Common.Dal.CacheQuery;
using F1Solutions.Naati.Common.Dal.Domain;
using F1Solutions.Naati.Common.Dal.NHibernate.Configuration;

namespace F1Solutions.Naati.Common.Dal.Portal.CacheQuery
{
    public class DisplayRolePlayerCacheQueryService : BaseLazyCacheQueryService<int, bool, bool>, IDisplayRolePlayerCacheQueryService
    {
        protected override bool GetDbSingleValue(int key)
        {
            var isRolePlayer = NHibernateSession.Current.Query<PanelMembership>()
                  .Count(
                      p => (p.Person.Entity.NaatiNumber == key)
                           && (p.PanelRole.PanelRoleCategory.Id == (int)PanelRoleCategoryName.RolePlayer)
                           && p.StartDate <= DateTime.Now
                           && p.EndDate >= DateTime.Now) > 0;

            return isRolePlayer;
        }

        protected override bool MapToTResultType(bool item) => item;

        protected override int TransformKey(int key) => key;

        public bool IsRolePlayer(int naatiNumber)
        {
            return GetItem(naatiNumber);
        }

        public void RefreshDisplayRolePlayersFlag(int naatiNumber)
        {
            RefreshItem(naatiNumber);
        }
    }
}
