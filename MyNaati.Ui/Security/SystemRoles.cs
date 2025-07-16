namespace MyNaati.Ui.Security
{
    public class SystemRoles
    {
        public const string ADMINISTRATORS = "Administrators";
        public const string NONCANDIDATE = "NonCandidate";
        public const string TEMPORARYPASSWORDUSERS = "TemporaryPasswordUsers";
        public const string EXAMINER = "Examiner";
        public const string CHAIR = "Chair";
        public const string AUTHENTICATED_EXAMINER = "AuthenticatedExaminer";
        public const string UNAUTHENTICATED_EXAMINER = "UnauthenticatedExaminer";
        public const string PRACTITIONER = "Practitioner";
        public const string FORMERPRACTITIONER = "FormerPractitioner";
        public const string FUTUREPRACTITIONER = "FuturePractitioner";
        public const string RECERTIFICATION = "Recertification";
    }

    public enum PanelManagementRoleTypeId
    {
        Chair = 1,
        Examiners = 4,
        ActingChair = 107
    }
}