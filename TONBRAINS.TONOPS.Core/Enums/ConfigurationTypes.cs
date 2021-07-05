namespace TONBRAINS.TONOPS.Core.Handlers
{
    /// <summary>
    /// Types configuration
    /// </summary>
    public enum ConfigurationTypes
    {
        /// <summary>
        /// Parse to <see cref=" Models.ConfigurationTemplate.UrlTemplate"/>
        /// </summary>
        ElasticSearch = 0,

        /// <summary>
        /// Parse to <see cref=" Models.ConfigurationTemplate.UrlTemplate"/>
        /// </summary>
        Kibana = 1,

        /// <summary>
        /// Parse to <see cref=" Models.ConfigurationTemplate.ServerTemplate"/>
        /// </summary>
        Logger = 2,

        DeployRoot = 3,

        PostgresConnectionString = 4,

        WebAppAddress = 6,
    }
}
