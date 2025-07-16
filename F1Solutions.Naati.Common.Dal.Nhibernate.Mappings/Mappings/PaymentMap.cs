using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;
using FluentNHibernate.Mapping;
using NAATI.Domain;

namespace F1Solutions.NAATI.WebService.NHibernate.Mappings.Mappings
{
    public class PaymentMap : IAutoMappingOverride<Payment>
    {
        public void Override(AutoMapping<Payment> mapping)
        {
            mapping.HasMany(p => p.PaymentAllocations)
               .Access.ReadOnlyPropertyThroughPascalCaseField(Prefix.m)
               .Cascade.AllDeleteOrphan()
               .Inverse();

            mapping.HasMany(p => p.PaymentCashes)
               .Access.ReadOnlyPropertyThroughPascalCaseField(Prefix.m)
               .Cascade.AllDeleteOrphan()
               .Inverse();

            mapping.HasMany(p => p.PaymentCreditCards)
               .Access.ReadOnlyPropertyThroughPascalCaseField(Prefix.m)
               .Cascade.AllDeleteOrphan()
               .Inverse();

            mapping.HasMany(p => p.PaymentDirectDeposits)
             .Access.ReadOnlyPropertyThroughPascalCaseField(Prefix.m)
             .Cascade.AllDeleteOrphan()
             .Inverse();

           mapping.HasMany(p => p.PaymentCheques)
           .Access.ReadOnlyPropertyThroughPascalCaseField(Prefix.m)
           .Cascade.AllDeleteOrphan()
           .Inverse();

           mapping.HasMany(p => p.PaymentOthers)
          .Access.ReadOnlyPropertyThroughPascalCaseField(Prefix.m)
          .Cascade.AllDeleteOrphan()
          .Inverse();

           mapping.References(p => p.BankDeposit)
               .Access.ReadOnlyPropertyThroughPascalCaseField(Prefix.m)
               .Nullable();
      
         }
    }

    public class PaymentAllocationMap : IAutoMappingOverride<PaymentAllocation>
    {
        public void Override(AutoMapping<PaymentAllocation> mapping)
        {
            mapping.References(x => x.Payment)
                .Access.ReadOnlyPropertyThroughPascalCaseField(Prefix.m);
        }
    }
}
