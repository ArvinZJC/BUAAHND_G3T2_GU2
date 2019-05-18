// csharp file that contains data properties for email settings

namespace NewEraFlowerStore.Data
{
    /// <summary>
    /// The class <see cref="EmailSettings"/> contains data properties for email settings.
    /// </summary>
    public class EmailSettings
    {
        /// <summary>
        /// A mail server.
        /// </summary>
        public string MailServer { get; set; }
        /// <summary>
        /// A mail port.
        /// </summary>
        public int MailPort { get; set; }
        /// <summary>
        /// A sender name.
        /// </summary>
        public string SenderName { get; set; }
        /// <summary>
        /// A sender.
        /// </summary>
        public string Sender { get; set; }
        /// <summary>
        /// A password.
        /// </summary>
        public string Password { get; set; }
    } // end class EmailSettings
} // end namespace NewEraFlowerStore.Data