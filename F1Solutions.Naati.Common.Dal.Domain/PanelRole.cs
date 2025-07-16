
namespace F1Solutions.Naati.Common.Dal.Domain
{
    public class PanelRole : EntityBase
    {
        public virtual string Name { get; set; }
        public virtual string Description { get; set; }
        public virtual bool  Chair { get; set; }

    public virtual PanelRoleCategory PanelRoleCategory { get; set; }

        public override string ToString()
        {
            return Description;
        }
    }
}
