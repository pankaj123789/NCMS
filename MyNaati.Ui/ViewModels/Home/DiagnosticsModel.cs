using System.Collections.Generic;
using MyNaati.Contracts.Portal;

namespace MyNaati.Ui.ViewModels.Home
{
    public class DiagnosticsModel
    {
        public bool IsFullyFunctional { get; set; }
        public IEnumerable<MessageNode> ErrorMessages { get; set; }
    }
}