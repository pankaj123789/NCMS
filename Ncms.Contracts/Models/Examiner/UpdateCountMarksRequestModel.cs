namespace Ncms.Contracts.Models.Examiner
{
    public class UpdateCountMarksRequestModel
    {
        public int TestResultId { get; set; }
        public int[] JobExaminersId { get; set; }
        public bool IncludePreviousMarks { get; set; }
    }
}
