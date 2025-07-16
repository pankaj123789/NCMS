using System;
using System.Collections.Generic;

namespace F1Solutions.Naati.Common.Dal.Domain
{
    public class ProfessionalDevelopmentActivity : EntityBase
    {
        private IList<ProfessionalDevelopmentActivityAttachment> mProfessionalDevelopmentActivityAttachments = new List<ProfessionalDevelopmentActivityAttachment>();

        public virtual DateTime DateCompleted { get; set; }
        public virtual string Description { get; set; }
        public virtual string Notes { get; set; }
        public virtual Person Person { get; set; }
        public virtual ProfessionalDevelopmentCategory ProfessionalDevelopmentCategory { get; set; }
        public virtual ProfessionalDevelopmentRequirement ProfessionalDevelopmentRequirement { get; set; }

        public virtual IEnumerable<ProfessionalDevelopmentActivityAttachment> ProfessionalDevelopmentActivityAttachments => mProfessionalDevelopmentActivityAttachments;
    }
}
