using System;

namespace F1Solutions.Naati.Common.Dal.Domain
{
    public class Language : EntityBase
    {
        public virtual string Name { get; set; }
        public virtual string Code { get; set; }
        public virtual LanguageGroup LanguageGroup { get; set; }
        public virtual User ModifiedUser { get; set; }
		public virtual bool ModifiedByNaati { get; set; }
		public virtual DateTime ModifiedDate { get; set; }
        public virtual string Note { get; set; }
	}
}

