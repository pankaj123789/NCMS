namespace F1Solutions.Naati.Common.Dal.Domain
{
    public class TestMaterialLink : EntityBase
    {
        public virtual  TestMaterialLinkType TestMaterialLinkType { get; set; }
        public virtual  TestMaterial FromTestMaterial { get; set; }
        public virtual  TestMaterial ToTestMaterial { get; set; }
    }
}
