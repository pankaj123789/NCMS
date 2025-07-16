namespace Ncms.Contracts.Models.Person
{
    public class EntitySearchRequest : QueryRequest
    {
        public EntitySearchType Type { get; set; }
    }

    public enum EntitySearchType
    {
        None = 0,
        Person = 1,
        Institution = 2
    }
}
