using System;

namespace NDExApiTests.utils
{
    public static class SharedIds
    {
        public static readonly Guid NetworkId1 = new Guid("1ca8c710-95d0-11e9-b7e4-0660b7976219"); // Network on user 1's account
        public static readonly Guid NetworkId2 = new Guid("325f2757-85f1-11e9-b14c-0660b7976219"); // Network on user 1's account
        public static readonly Guid NetworkId3 = new Guid("341ac93d-70db-11e9-b14c-0660b7976219"); // Network on user 2's account in a private set
        public const string Network3AccessKey = "5291f517113aa6659087a3257e4ee771c554e982789695b3a29645fa8e22618f"; // Key for private network 3 from user 2
        public static readonly Guid GroupId1 = new Guid("b49a3430-699c-11e9-831d-0660b7976219"); // user 1 + 2
        public static readonly Guid GroupId2 = new Guid("c97c45f1-699c-11e9-831d-0660b7976219"); // user 1 only
        public static readonly Guid UserId1 = new Guid("9f762cdf-699b-11e9-831d-0660b7976219");
        public static readonly Guid UserId2 = new Guid("8876a95b-70da-11e9-b14c-0660b7976219");
    }
}