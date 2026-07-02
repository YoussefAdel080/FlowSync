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
        }
    }
}
