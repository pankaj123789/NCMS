using System;
using System.Collections.Generic;

namespace F1Solutions.Naati.Common.Dal.Domain
{
    public class EmailTemplate : EntityBase
    {
        public EmailTemplate()
        {
            mSystemActionEmailTemplates = new List<SystemActionEmailTemplate>();
            mEmailMessages = new List<EmailMessage>();
        }

        public virtual string Name { get; set; }
        public virtual string Subject { get; set; }
        public virtual string Content { get; set; }
        public virtual bool Active { get; set; }
        public virtual string FromAddress { get; set; }

		public virtual bool ModifiedByNaati { get; set; }
		public virtual DateTime ModifiedDate { get; set; }
		public virtual int ModifiedUser { get; set; }

		private IList<SystemActionEmailTemplate> mSystemActionEmailTemplates;
        private IList<EmailMessage> mEmailMessages;

        public virtual IEnumerable<SystemActionEmailTemplate> SystemActionEmailTemplates => mSystemActionEmailTemplates;
        public virtual IEnumerable<EmailMessage> EmailMessages => mEmailMessages;
    }
}
