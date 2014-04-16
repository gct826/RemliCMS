using System;
using System.Globalization;

namespace ExtendedMongoMembership
{
    /// <summary>
    /// Represents an OpenAuth and OpenID account.
    /// </summary>
    public class OAuthAccountDataEmbedded
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OAuthAccountDataEmbedded"/> class.
        /// </summary>
        /// <param name="provider">The provider.</param>
        /// <param name="providerUserId">The provider user id.</param>
        public OAuthAccountDataEmbedded(string provider, string providerUserId)
        {
            if (String.IsNullOrEmpty(provider))
            {
                throw new ArgumentException(
                    String.Format(CultureInfo.CurrentCulture, "Argument_Cannot_Be_Null_Or_Empty", "provider"),
                    "provider");
            }

            if (String.IsNullOrEmpty(providerUserId))
            {
                throw new ArgumentException(
                    String.Format(CultureInfo.CurrentCulture, "Argument_Cannot_Be_Null_Or_Empty", "providerUserId"),
                    "providerUserId");
            }

            Provider = provider;
            ProviderUserId = providerUserId;
        }

        /// <summary>
        /// Gets the provider name.
        /// </summary>
        public string Provider { get; private set; }

        /// <summary>
        /// Gets the provider user id.
        /// </summary>
        public string ProviderUserId { get; private set; }
    }
}
