namespace F1Solutions.Naati.Common.Contracts.Dal
{
    public class ApplicationFieldData
    {
        public int Id { get; set; }
        public int FieldDataId { get; set; }
        public int FieldTypeId { get; set; }
        public string Value { get; set; }
        public int FieldOptionId { get; set; }
    }
}