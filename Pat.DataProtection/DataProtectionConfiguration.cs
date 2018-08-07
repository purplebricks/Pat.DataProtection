namespace Pat.DataProtection
{
    /// <summary>
    /// The configuration applied to the DataProtectionProvider
    /// </summary>
    public class DataProtectionConfiguration
    {
        /// <summary>
        /// The unique name of this application within the data protection system.
        /// </summary>
        public string ApplicationName { get; set; }
        /// <summary>
        /// The cloud storage account used to store key in the data protection system.
        /// </summary>
        public string AccountName { get; set; }
        /// <summary>
        /// The key to access the cloud storage account used to store key in the data protection system.
        /// </summary>
        public string AccountKey { get; set; }
        /// <summary>
        /// The thumbprint of the certificate used to protect the keys.
        /// </summary>
        public string Thumbprint { get; set; }
    }
}