using System;

namespace NDExApi.model
{
    public class Membership
    {
        public Permissions permissions;
        public MembershipType membershipType;
        public Guid memberUUID;
        public Guid resourceUUID;
        public string memberAccountName;
        public string resourceName;
    }
}