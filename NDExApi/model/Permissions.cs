namespace NDExApi.model
{
    /// <summary>
    /// Represents different kinds of permissions for groups and users.
    /// Not all values are applicable for all endpoint permission parameters, read more in the specific webservice documentation.
    /// </summary>
    public enum Permissions
    {
        READ,
        WRITE,
        ADMIN,
        MEMBER,
        GROUPADMIN
    }
}