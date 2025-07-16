using MyNaati.Ui.LINQtoCSV;

namespace MyNaati.Ui.Reports.OnlineOrderDetails
{
    public class RegistrationUserRequestCsv
    {
        [CsvColumn(FieldIndex = 1, Name = "Customer Number")]
        public int NaatiNumber { get; set; }

        [CsvColumn(FieldIndex = 2, Name = "Name")]
        public string FullName { get; set; }

        [CsvColumn(FieldIndex = 3, Name = "Date of Birth")]
        public string DateOfBirth { get; set; }

        [CsvColumn(FieldIndex = 4, Name = "Email address provided")]
        public string Email { get; set; }

        [CsvColumn(FieldIndex = 5, Name = "Requested")]
        public string DateRequested { get; set; }

    }
}