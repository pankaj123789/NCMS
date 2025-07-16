using System.Collections.Generic;

namespace F1Solutions.Naati.Common.Dal.Domain
{
    public class CredentialApplicationField : EntityBase
    {
        private IList<CredentialApplicationFieldOptionOption> mOptions = new List<CredentialApplicationFieldOptionOption>();
        public virtual CredentialApplicationType CredentialApplicationType { get; set; }
        public virtual CredentialApplicationFieldCategory CredentialApplicationFieldCategory { get; set; }
        public virtual string Name { get; set; }
        public virtual string Section { get; set; }
        public virtual DataType DataType { get; set; }
        public virtual string DefaultValue { get; set; }
        public virtual bool PerCredentialRequest { get; set; }
        public virtual string Description { get; set; }
        public virtual bool Mandatory { get; set; }
        public virtual bool Disabled { get; set; }
        public virtual int DisplayOrder { get; set; }

        public virtual IEnumerable<CredentialApplicationFieldOptionOption> Options => mOptions;
    }
}
