using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;
using NAATI.Domain;

namespace F1Solutions.NAATI.WebService.NHibernate.Mappings.Mappings
{
    public class ChangeRequestMap : IAutoMappingOverride<ChangeRequest>
    {
        public void Override(AutoMapping<ChangeRequest> mapping)
        {
            mapping.HasMany(x => x.ApprovalConditions)
                .Inverse();

            mapping.HasMany(x => x.Inspections)
                .Inverse();

            mapping.HasMany(x => x.ChangeRequestCourseApprovals)
                .Inverse();


            mapping.References(x => x.EmailTemplate).Column("EmailTemplateId");
            mapping.References(x => x.LetterTemplate).Column("LetterTemplateId");
            
            mapping.References(x => x.EmailToEntity).Column("EmailToEntityId");
            mapping.References(x => x.LetterToEntity).Column("LetterToEntityId");
        }
    }
}
