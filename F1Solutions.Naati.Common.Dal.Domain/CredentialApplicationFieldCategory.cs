using System.Collections.Generic;

namespace F1Solutions.Naati.Common.Dal.Domain
{
    public class CredentialApplicationFieldCategory : EntityBase
    {
        public virtual string Name { get; set; }
        public virtual string DisplayName { get; set; }
    }
}
