using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace MyNaati.Ui.ViewModels.PersonalDetails
{
    public class WebsiteEditModel
    {
        public WebsiteEditModel()
        { }

        public string LastUpdated { get; set; }

        public bool IsCurrentlyListed { get; set; }

        // For information on website url vaidation see:
        // http://www.dotnetspider.com/forum/ViewForum.aspx?ForumId=95
        // http://www.ietf.org/rfc/rfc2396.txt
        [DisplayName("Web site URL")]
        //                     (protocol)?       domain.blah.blah.blah           (:port)?      (/dir/dir)?       (/filename.ext)?
        [RegularExpression(@"^((http|https)://)?[a-zA-Z0-9\-]+(\.[a-zA-Z0-9\-]+)+(:[0-9]{1,5})?(/[a-zA-Z0-9\-]+)*(/[a-zA-Z0-9\-\.~]*)?$", ErrorMessage = "The web site URL does not have a valid web site URL structure.")]
        [StringLength(200, ErrorMessage = "{0} must be at most {1} characters long.")]
        public string Url { get; set; }
    }
}
