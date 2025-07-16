using System;
using System.Collections.Generic;

namespace F1Solutions.Naati.Common.Dal.Domain
{
    public class PanelMembership : EntityBase
    {
        private IList<PanelMembershipCredentialType> mPanelMembershipCredentialTypes = new List<PanelMembershipCredentialType>();
        private IList<PanelMembershipMaterialCredentialType> mPanelMembershipMaterialCredentialTypes = new List<PanelMembershipMaterialCredentialType>();
        private IList<PanelMembershipCoordinatorCredentialType> mPanelMembershipCoordinatorCredentialTypes = new List<PanelMembershipCoordinatorCredentialType>();
        private IList<JobExaminer> mJobExaminers = new List<JobExaminer>();

        public virtual Panel Panel { get; set; }
        public virtual Person Person { get; set; }
        public virtual PanelRole PanelRole { get; set; }
        public virtual DateTime StartDate { get; set; }
        public virtual DateTime EndDate { get; set; }

        public virtual IEnumerable<PanelMembershipCredentialType> PanelMembershipCredentialTypes => mPanelMembershipCredentialTypes;
        public virtual IEnumerable<PanelMembershipMaterialCredentialType> PanelMembershipMaterialCredentialTypes => mPanelMembershipMaterialCredentialTypes;
        public virtual IEnumerable<PanelMembershipCoordinatorCredentialType> PanelMembershipCoordinatorCredentialTypes => mPanelMembershipCoordinatorCredentialTypes;

        
        public virtual IEnumerable<JobExaminer> JobExaminers => mJobExaminers;
    }
}
