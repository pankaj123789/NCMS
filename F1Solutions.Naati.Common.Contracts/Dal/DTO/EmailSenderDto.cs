namespace F1Solutions.Naati.Common.Contracts.Dal.DTO
{
    public class EmailSenderDto
    {
        public string Name { get; set; }
        public string EmailAddress { get; set; }

        public string NameEmailConcat
        {
            get { return string.Format(@"{0} <{1}>", Name, EmailAddress); }
        }
    }
}