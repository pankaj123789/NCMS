using System.Collections.Generic;

namespace F1Solutions.Naati.Common.Contracts.Dal.DTO
{
    public class CredentialApplicationFieldDto
    {
        public int Id { get; set; }
        public int FieldTypeId { get; set; }
        public string Name { get; set; }
        public string Section { get; set; }
        public int DataTypeId { get; set; }
        public string DefaultValue { get; set; }
        public bool PerCredentialRequest { get; set; }
        public string Description { get; set; }
        public bool Mandatory { get; set; }
        public bool Disabled { get; set; }
        public int DisplayOrder { get; set; }

        public IEnumerable<CredentialApplicationFieldOptionDto> Options { get; set; }
    }
}
