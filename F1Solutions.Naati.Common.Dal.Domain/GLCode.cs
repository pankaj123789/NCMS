using System;

namespace F1Solutions.Naati.Common.Dal.Domain
{
    public class GLCode : LegacyEntityBase
    {
        public GLCode(int id) : base(id)
        {
        }

        public GLCode()
        {
        }

        public virtual string Code { get; set; }
        public virtual string AccountName { get; set; }
        public virtual string Description { get; set; }
        public virtual Guid? ExternalReferenceAccountId { get; set; }

    }
}
