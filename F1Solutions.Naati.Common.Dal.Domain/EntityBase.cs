namespace F1Solutions.Naati.Common.Dal.Domain
{
    public abstract class EntityBase : Entity, IAuditObject
    {
        public virtual IAuditObject RootAuditObject
        {
            get { return null; }
        }

        protected virtual string AuditName
        {
            get { return this.GetType().Name; }
        }

        string IAuditObject.AuditName
        {
            get { return this.AuditName; }
        }
    }

    public interface IAuditObject
    {
        int Id { get; }
        string AuditName { get; }
    }
}