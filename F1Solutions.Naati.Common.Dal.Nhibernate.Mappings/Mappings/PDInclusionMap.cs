using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;
using NAATI.Domain;

namespace F1Solutions.NAATI.WebService.NHibernate.Mappings.Mappings
{
    public class PDInclusionMap : IAutoMappingOverride<PDInclusion>
    {
        public void Override(AutoMapping<PDInclusion> mapping)
        {
            mapping.References(m => m.FirstReminder).Column("FirstReminderEmailBatchId").Cascade.None();
            mapping.References(m => m.SecondReminder).Column("SecondReminderEmailBatchId").Cascade.None();
        }
    }
}
