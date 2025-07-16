namespace MyNaati.Ui.ViewModels.UnraisedInvoices
{
    public class UnraisedInvoicesQuestionLogicModel
    {
        public int Id { get; set; }

        public int AnswerId { get; set; }
     
        public bool Not { get; set; }

        public bool And { get; set; }

        public int Type { get; set; }

        public int Group { get; set; }
        public int Order { get; set; }

        public int SkillId { get; set; }

    }
}