using System;
using System.Collections.Generic;

namespace F1Solutions.Naati.Common.Dal.Domain
{
    public class EmailMessage : EntityBase
    {
        public EmailMessage()
        {
            mAttachments = new List<EmailMessageAttachment>();
        }

        public virtual NaatiEntity RecipientEntity { get; set; }
        public virtual User RecipientUser { get; set; }

        public virtual string RecipientEmail { get; set; }
        public virtual string Bcc { get; set; }
        public virtual string Cc { get; set; }
        public virtual string Subject { get; set; }
        public virtual string Body { get; set; }
        public virtual string LastSendResult { get; set; }
        public virtual DateTime? LastSendAttemptDate { get; set; }
        public virtual User CreatedUser { get; set; }
        public virtual DateTime? CreatedDate { get; set; }
        public virtual string FromAddress { get; set; }

        public virtual EmailSendStatusType EmailSendStatusType { get; set; }

        public override IAuditObject RootAuditObject => RecipientEntity;

        private IList<EmailMessageAttachment> mAttachments;

        public virtual IEnumerable<EmailMessageAttachment> Attachments => mAttachments;
        public virtual EmailTemplate EmailTemplate { get; set; }

        public virtual void AddAttachment(EmailMessageAttachment attachment)
        {
            attachment.EmailMessage = this;
            mAttachments.Add(attachment);
        }
    }
}
