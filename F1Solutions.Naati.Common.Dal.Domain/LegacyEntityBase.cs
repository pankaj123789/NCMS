namespace F1Solutions.Naati.Common.Dal.Domain
{
    public abstract class LegacyEntityBase : EntityBase
    {
        protected LegacyEntityBase(int id)
        {
            // To allow for assigned ids.
            base.Id = id;
        }

        protected LegacyEntityBase()
        {
        }
    }
}
