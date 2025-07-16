using System.Collections.Generic;

namespace F1Solutions.Naati.Common.Contracts.Dal.DTO
{
    public class CredentialLookupTypeDto : LookupTypeDto
    {
        public new int DisplayOrder { get; set; }
        public int CategoryId { get; set; }
        public IEnumerable<CredentialLookupTypeDto> Children { get; set; }
        public int CredentialRequestPathTypeId { get; set; }
    }
}