namespace Ncms.Contracts.Models.Letter
{
    public class LetterModel
    {
        public int StandardLetterId { get; set; }
        public string Description { get; set; }
        public string FullDescription { get; set; }
        public bool? LetterVisible { get; set; }
        public int StandardLetterCategoryId { get; set; }
    }
}
