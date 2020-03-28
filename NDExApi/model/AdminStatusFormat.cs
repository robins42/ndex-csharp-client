namespace NDExApi.model
{
    /// <summary>
    /// Determines the server information to be returned.
    /// </summary>
    public enum AdminStatusFormat
    {
        /// <summary>
        /// Returns properties such as server version and build.
        /// </summary>
        Short,

        /// <summary>
        /// Returns standard properties as well as additional properties.
        /// </summary>
        Full
    }
}