using System;

namespace F1Solutions.Naati.Common.Contracts.Dal.DTO
{
    public class PanelMembershipDto
    {
        public PanelDto Panel { get; set; }
        public string RoleName { get; set; }
        public DateTime From { get; set; }
        public DateTime To { get; set; }
    }
}