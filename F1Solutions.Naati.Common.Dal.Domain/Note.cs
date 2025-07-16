using System;
using System.Collections.Generic;

namespace F1Solutions.Naati.Common.Dal.Domain
{
    public class Note : EntityBase
    {
        public Note()
        {
            mNoteAttachments = new List<NoteAttachment>();
        }

        public virtual User User { get; set; }
        public virtual string Description { get; set; }
        public virtual DateTime CreatedDate { get; set; }
        public virtual DateTime? ModifiedDate { get; set; }
        public virtual bool Highlight { get; set; }
        public virtual bool ReadOnly { get; set; }

      
        private IList<NoteAttachment> mNoteAttachments;

        public virtual IEnumerable<NoteAttachment> NoteAttachments
        {
            get { return mNoteAttachments; }
        }
    }
}
