namespace F1Solutions.Naati.Common.Contracts.Dal.Enum
{
    public enum PractitionerFilterType
    {
        Language1IntList,
        Language2IntList,
        CredentialTypeAndSkillIntList,
        CountryIntList,
        StateIntList,
        PostcodeString,
        FamilyNameString,
        PersonIdIntList,
        SkillIntList
    }

    public enum ApiPublicPractitionerFilterType
    {
        CountryIntList,
        StateIntList,
        PostcodeIntList,
        FamilyNameStringList,
        PersonIdIntList,
        SkillIntList,
        CredentialTypeIntList
    }
}