namespace F1Solutions.Naati.Common.Dal.Domain
{
    public class JobExaminerRubricTestComponentResult : EntityBase 
    {
        public virtual JobExaminer JobExaminer { get; set; }
        public virtual RubricTestComponentResult RubricTestComponentResult { get; set; }
    }
}
