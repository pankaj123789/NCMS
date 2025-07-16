using System.Collections.Generic;

namespace F1Solutions.Naati.Common.Dal.Domain
{
    public class MaterialRequestPanelMembership : EntityBase
    {
        private IList<MaterialRequestPanelMembershipTask> mMaterialRequestPanelMembershipTasks = new List<MaterialRequestPanelMembershipTask>();

        public virtual MaterialRequest MaterialRequest { get; set; }
        public virtual  PanelMembership PanelMembership { get; set; }
        public virtual MaterialRequestPanelMembershipType MaterialRequestPanelMembershipType { get; set; }

        public virtual IEnumerable<MaterialRequestPanelMembershipTask> MaterialRequestPanelMembershipTasks => mMaterialRequestPanelMembershipTasks;
    }
}
