namespace EheathBlockChain
{
    internal static class Constants
    {
        public const string CookieName = "EhealthCookie";
        public const string ClientId = "EhealthClientId";
        public const string ClientSecret = "EhealthClientSecret";

        public static class AuthenticateDtoNames
        {
            public const string Login = "login";
            public const string Password = "password";
        }

        public static class ErrorResponseNames
        {
            public const string Code = "error";
            public const string Message = "error_description";
        }

        public static class ErrorCodes
        {
            public const string IncompleteRequest = "incomplete_request";
            public const string InvalidConfiguration = "invalid_configuration";
        }

        public static class GrantedTokenNames
        {
            public const string AccessToken = "access_token";
            public const string IdToken = "id_token";
            public const string TokenType = "token_type";
            public const string ExpiresIn = "expires_in";
            public const string RefreshToken = "refresh_token";
            public const string Scope = "scope";
        }
    }
}
