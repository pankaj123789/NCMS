using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;
using FluentNHibernate.Mapping;
using NAATI.Domain;

namespace F1Solutions.NAATI.WebService.NHibernate.Mappings.Mappings
{
    public class BankDepositMap : IAutoMappingOverride<BankDeposit>
    {
        public void Override(AutoMapping<BankDeposit> mapping)
        {

            mapping.HasMany(x => x.Payments)
                .Access.ReadOnlyPropertyThroughPascalCaseField(Prefix.m)
                .Inverse();
        }
    }
}
