using System;

namespace Ncms.Contracts.Models.Language
{
    public class ExaminerModel
    {
        public bool DoNotSendCorrespondence { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int EntityId { get; set; }
        public int NAATINumber { get; set; }
        public string NaatiNumberDisplay { get; set; }
        public int PanelId { get; set; }
        public int PanelMembershipId { get; set; }
        public int PersonId { get; set; }
        public string PersonName { get; set; }
        public int PanelRoleId { get; set; }
        public bool IsChair { get; set; }
    }
}
