namespace NDExApi.model
{
    public class User : Account
    {
        public string emailAddress;
        public string firstName;
        public string lastName;
        public long diskQuota;
        public long diskUsed;
        public string displayName;
        public bool isIndividual;
        public string userName;
        public string password;
        public bool isVerified;
    }
}