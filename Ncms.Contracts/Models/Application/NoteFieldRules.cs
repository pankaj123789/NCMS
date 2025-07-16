namespace Ncms.Contracts.Models.Application
{
    public class NoteFieldRules
    {
        public bool ShowPrivateNote { get; set; }
        public bool ShowPublicNote { get; set; }
        public bool RequirePublicNote { get; set; }
        public bool RequirePrivateNote { get; set; }
    }
}