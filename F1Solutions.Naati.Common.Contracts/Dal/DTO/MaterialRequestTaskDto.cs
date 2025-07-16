namespace F1Solutions.Naati.Common.Contracts.Dal.DTO
{
    public class MaterialRequestTaskDto
    {
        public  int Id { get; set; }
        public int MaterialRequestTaskTypeId { get; set; }
        public double HoursSpent { get; set; }
        public string MaterialRequestTaskTypeDisplayName { get; set; }
    }
}
