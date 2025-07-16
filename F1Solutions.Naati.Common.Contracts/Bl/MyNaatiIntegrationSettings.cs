namespace F1Solutions.Naati.Common.Contracts.Bl
{
    public static class MyNaatiIntegrationSettings
    {
        public const string MyNaatiRoutePrefix = "api/private";

        public const string DeleteUser = "deleteUser";
        public const string RenameUser = "renameUser";
        public const string UnlockUser = "unlockUser";
        public const string GetUser = "getUser";
        public const string RefreshUserCache = "refreshUserCache";
        public const string ExecuteRefreshPendingUsersTask = "executeRefreshPendingUsersTask";
        public const string RefreshAllUsersCache = "refreshAllUsersCache";
        public const string RefreshAllInvalidCookies = "refreshAllInvalidCookies";
        public const string RefreshSystemCache = "refreshSystemCache";
        public const string InvalidateCacheCookie = "invalidateCacheCookie";
    }
}
