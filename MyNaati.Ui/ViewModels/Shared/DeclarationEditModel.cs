using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace MyNaati.Ui.ViewModels.Shared
{
    public class DeclarationEditModel
    {
        public DeclarationEditModel()
        {
            UserAgrees = false;
        }

        [UIHint("boolean"), DisplayName("I understand and agree to all of the above")]
        public bool UserAgrees { get; set; }
    }
}