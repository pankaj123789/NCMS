namespace F1Solutions.Naati.Common.Dal.Domain
{
    public class TestComponentBaseType : EntityBase
    {
        public virtual string Name { get; set; }
        public virtual string DisplayName { get; set; }
    }

    public enum TestComponentBaseTypeName
    {
        Language = 1,
        Skill = 2
    }
}
