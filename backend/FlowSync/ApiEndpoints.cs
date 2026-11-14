namespace FlowSync
{
    public static class ApiEndpoints
    {
        private const string ApiBase = "api";

        public static class Auth
        {
            private const string Base = $"{ApiBase}/auth";

            public const string Register = $"{Base}/register";
            public const string Login = $"{Base}/login";
            public const string Refresh = $"{Base}/refresh";
            public const string VerifyEmail = $"{Base}/verify-email";
            public const string ChangePassword = $"{Base}/change-password";
            public const string ForgotPassword = $"{Base}/forgot-password";
            public const string ResetPassword = $"{Base}/reset-password";
            public const string Logout = $"{Base}/logout";
            public const string Profile = $"{Base}/profile";
        }
        public static class Workspace
        {
            private const string Base = $"{ApiBase}/workspace";

            public const string Create = $"{Base}";
            public const string Update = $"{Base}";
            public const string Delete = $"{Base}";
            public const string MyWorkspaces = $"{Base}/my-workspaces";
            public const string GetById = $"{Base}/{{id:guid}}";
        }
        public static class WorkspaceInvitation
        {
            private const string Base = $"{ApiBase}/workspace-invitation";
            public const string GetPendingInvetations = $"{Base}/pending";
            public const string Invite = $"{Base}";
            public const string Accept = $"{Base}/accept";
            public const string Decline = $"{Base}/decline";
            public const string Cancel = $"{Base}/cancel";
        }
        public static class WorkspaceMembers
        {
            private const string Base = $"{ApiBase}/{{workspaceId:guid}}/workspace-member";
            public const string GetWorkspaceMembers = $"{Base}";
            public const string GetWorkspaceMember = $"{Base}/{{memberId:guid}}";
            public const string ChangeRole = $"{Base}/{{memberId:guid}}/role";
            public const string Remove = $"{Base}/{{memberId:guid}}/remove";
            public const string Leave = $"{Base}/leave";
        }
    }
}
