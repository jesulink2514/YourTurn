namespace Techies.YourTurn.Security
{
    public class AzureADB2COptions
    {
        public string Tenant { get; set; }
        public string B2CHostName { get; set; }
        public string ClientId { get; set; }
        public string PolicySignUpSignIn { get; set; }
        public string PolicyResetPassword { get; set; }
        public string[] Scopes { get;set; }
        public string AuthorityBase { get; set; }
        public string iOSKeyChainGroup { get; set; }
    }
}