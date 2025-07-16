namespace F1Solutions.Naati.Common.Dal.Domain
{
    public class TestSittingTestMaterial : EntityBase
    {
        public virtual TestSitting TestSitting { get; set; }
        public virtual TestMaterial TestMaterial { get; set; }
        public virtual TestComponent TestComponent { get; set; }
    }
}
