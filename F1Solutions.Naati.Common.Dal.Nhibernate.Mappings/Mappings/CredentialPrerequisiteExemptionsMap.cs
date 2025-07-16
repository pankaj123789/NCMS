using F1Solutions.Naati.Common.Dal.Domain;
using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace F1Solutions.Naati.Common.Dal.Nhibernate.Mappings.Mappings
{
    public class CredentialPrerequisiteExemptionsMap : IAutoMappingOverride<CredentialPrerequisiteExemption>
    {
        public void Override(AutoMapping<CredentialPrerequisiteExemption> mapping)
        {
            mapping.References(x => x.ModifiedUser).Column("ModifiedUser");
        }
    }
}
