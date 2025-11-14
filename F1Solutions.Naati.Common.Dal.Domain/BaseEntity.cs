// F1Solutions.Naati.Common.Dal\BaseEntity.cs

using System;

namespace F1Solutions.Naati.Common.Dal.Domain
{
    public abstract class BaseEntity : EntityBase
    {
        // Soft-delete audit fields (virtual for NHibernate proxying)
        public virtual bool IsDeleted { get; set; }
        public virtual DateTimeOffset? DeletedOn { get; set; }
        public virtual string DeletedBy { get; set; }

        // Helpers to mark/unmark soft-delete
        public virtual void SoftDelete(string deletedBy)
        {
            IsDeleted = true;
            DeletedOn = DateTimeOffset.UtcNow;
            DeletedBy = deletedBy;
        }

        public virtual void Restore()
        {
            IsDeleted = false;
            DeletedOn = null;
            DeletedBy = null;
        }
    }
}
