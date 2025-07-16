using MyNaati.Ui.LINQtoCSV;

namespace MyNaati.Ui.Reports.OnlineOrderDetails
{
    public class UserCsv
    {
        [CsvColumn(FieldIndex = 1, Name = "Customer number")]
        public int NaatiNumber { get; set; }

        [CsvColumn(FieldIndex = 2, Name = "Name")]
        public string FullName { get; set; }

        [CsvColumn(FieldIndex = 3, Name = "Email address")]
        public string Email { get; set; }

        [CsvColumn(FieldIndex = 4, Name = "Date Created")]
        public string CreationDate { get; set; }

        //[CsvColumn(FieldIndex = 4, Name = "Deceased")]
        //public bool Deceased { get; set; }

    }
}