using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MyNaati.Ui.ViewModels.UnraisedInvoices
{
    public class UnraisedInvoicesSectionModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int DisplayOrder { get; set; }
        public List<UnraisedInvoicesQuestionModel> Questions { get; set; }
    }
}