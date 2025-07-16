using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using F1Solutions.Naati.Common.Dal.Domain;
using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;

namespace F1Solutions.Naati.Common.Dal.Nhibernate.Mappings.Mappings
{
    public class IssuedCredentialCredentialRequestMap : IAutoMappingOverride<IssuedCredentialCredentialRequest>
    {
        public void Override(AutoMapping<IssuedCredentialCredentialRequest> mapping)
        {
            mapping.Table("vwIssuedCredentialCredentialRequest");
            mapping.ReadOnly();
            mapping.Id(x => x.Id).Column("CredentialId");
        }
    }
}
