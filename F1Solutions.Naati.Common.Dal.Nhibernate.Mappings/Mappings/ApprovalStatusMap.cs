using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentNHibernate.Automapping.Alterations;
using NAATI.Domain;
using FluentNHibernate.Automapping;

namespace F1Solutions.NAATI.WebService.NHibernate.Mappings.Mappings
{
    public class ApprovalStatusMap : IAutoMappingOverride<ApprovalStatus>
    {
        public void Override(AutoMapping<ApprovalStatus> mapping)
        {
            mapping.Table("tluApprovalStatus");

        }
    }

}
