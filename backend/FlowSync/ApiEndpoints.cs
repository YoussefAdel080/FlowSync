namespace FlowSync
{
    public static class ApiEndpoints
    {
        private const string ApiBase = "api";

        public static class Auth
        {
            private const string Base = $"{ApiBase}/Auth";

            public const string Register = $"{Base}/Register";
            public const string Login = $"{Base}/Login";
            public const string Refresh = $"{Base}/Refresh";
            public const string VerifyEmail = $"{Base}/Verify-Email";
            public const string ChangePassword = $"{Base}/Change-Password";
            public const string ForgotPassword = $"{Base}/Forgot-Password";
            public const string ResetPassword = $"{Base}/Reset-Password";
            public const string Logout = $"{Base}/Logout";
            public const string Profile = $"{Base}/Profile";
        }
    }
}
