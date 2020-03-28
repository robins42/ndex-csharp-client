namespace NDExApi.model
{
    /// <summary>
    /// Defines authentication to an NDEx account, via login/password or OAuth token
    /// </summary>
    public class Authentication
    {
        internal readonly string Username;
        internal readonly string Password;
        internal readonly string OAuth;

        public Authentication(string username, string password)
        {
            Username = username;
            Password = password;
        }
        
        public Authentication(string oAuthToken)
        {
            OAuth = oAuthToken;
        }
    }
}