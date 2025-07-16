namespace F1Solutions.Naati.Common.Dal.Domain
{
    public interface IDynamicLookupType : IAuditObject
    {
        string DisplayName { get; set; }
    }

    public interface IDynamicLookupTypeOrderBy : IDynamicLookupType
    {
        int DisplayOrder { get; set; }
    }
}
