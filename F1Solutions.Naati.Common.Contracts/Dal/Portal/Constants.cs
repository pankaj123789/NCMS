namespace F1Solutions.Naati.Common.Contracts.Dal.Portal
{
    public static class Constants
    {
        // const arrays are not allowed in c# nor can you add a "readonly" member to a static class. The bellow syntax is a way around this
        public static string[] GENDER_STRING => new[] { "", "M", "F", "X" };
        public const string UNSPECIFIED_GENDER_INDEX = "X";
        public const string MALE_GENDER_INDEX = "M";
        public const string FEMALE_GENDER_INDEX = "F";
        public const int EMAIL_CONTACT_TYPE_HOME = 1;
        public const int ENTITY_TYPE_PERSON_NON_CANDIDATE = 2;
    }
}