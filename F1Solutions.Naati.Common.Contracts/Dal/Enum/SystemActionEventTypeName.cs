namespace F1Solutions.Naati.Common.Contracts.Dal.Enum
{
    public enum SystemActionEventTypeName
    {
        None = 1,
        InvoiceCreatedToApplicant = 2,
        InvoiceCreatedToUntrustedSponsor = 3,
        CreditCardPaymentReceived = 4,
        SupplementaryTestOffered = 5,
        ConcededPassOffered = 6,
        FailedTestReverted = 7,
        PassedTestReverted = 8,
        IssuedCredentialReverted = 9,
        InvalidatedTestReverted = 10,
        TestSessionConfirmed = 11,
        CoordinatorChanged = 12,
        CollaboratorAdded = 13,
        CollaboratorRemoved = 14,
        CreditCardRefundIssuedToApplicant = 15,
        PayPalPaymentReceived = 16
    }
}