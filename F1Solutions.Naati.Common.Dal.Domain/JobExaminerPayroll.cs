namespace F1Solutions.Naati.Common.Dal.Domain
{
    public class JobExaminerPayroll : EntityBase
    {
        public virtual JobExaminer JobExaminer { get; set; }
        public virtual Payroll Payroll { get; set; }
        public virtual string AccountingReference { get; set; }
    }
}