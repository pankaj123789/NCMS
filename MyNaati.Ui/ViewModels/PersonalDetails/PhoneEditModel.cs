using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace MyNaati.Ui.ViewModels.PersonalDetails
{
    public class PhoneEditModel
    {
        public PhoneEditModel()
        { }

        public int Id { get; set; }

        public bool IsCurrentlyListed { get; set; }

        [DisplayName("Number")]
        [Required(ErrorMessage = "A phone number is required.")]
        [RegularExpression(@"^([0-9\(\)\+ \-]*)$", ErrorMessage = "Phone number can only contain (,),+,- and digits.")]
        public string Number { get; set; }

        [DisplayName("Preferred Phone")]
        public bool IsPreferred { get; set; }

        public bool Success { get; set; }

        public bool ExaminerCorrespondence { get; set; }

        public IList<string> Errors { get; set; }
    }
}
