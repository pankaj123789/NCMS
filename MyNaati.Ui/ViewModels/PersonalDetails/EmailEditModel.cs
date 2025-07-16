using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace MyNaati.Ui.ViewModels.PersonalDetails
{
    public class EmailEditModel
    {
        public EmailEditModel()
        { }

        public int Id { get; set; }

        public bool IsCurrentlyListed { get; set; }

        [DisplayName("Email")]
        [Required(ErrorMessage = "The email address does not have a valid email address structure.")]
        [RegularExpression(@"^([a-zA-Z0-9_'+*$%\^&!\.\-])+\@(([a-zA-Z0-9\-])+\.)+([a-zA-Z0-9:]{2,4})+$",
                           ErrorMessage = "The email address does not have a valid email address structure.")]
        [StringLength(200, ErrorMessage = "Email address must be at most {1} characters long.")]
        public string Email { get; set; }

        [DisplayName("Preferred email")]
        public bool IsPreferred { get; set; }


        [DisplayName("Examiner Correspondence")]
        public bool ExaminerCorrespondence { get; set; }

        public bool Success { get; set; }

        public List<string> Errors { get; set; }

        public string PrimaryEmail { get; set; }
    }
}
