using F1Solutions.Naati.Common.Contracts.Dal.DTO;
using System;
using System.Collections.Generic;

namespace F1Solutions.Naati.Common.Contracts.Dal.Request
{
    public class AddOrUpdateMembershipRequest
    {
        public int? PanelMembershipId { get; set; }
        public int PersonId { get; set; }
        public int PanelId { get; set; }
        public int PanelRoleId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int RolePlayerSessionLimit { get; set; }
        public decimal? RolePlayerRating { get; set; }
        public bool RolePlayerSenior { get; set; }
        public IEnumerable<int> CredentialTypeIds { get; set; }
        public IEnumerable<int> MaterailCredentialTypesIds { get; set; }
        public IEnumerable<int> CoordinatorCredentialTypesIds { get; set; }
    }
}