namespace Dhbw.ThesisManager.Api.Configuration
{
    public class KeycloakOptions
    {
        public const string Section = "Keycloak";
        
        public string AdminUrl { get; set; }
        public string Realm { get; set; }
        public string AdminClientId { get; set; }
        public string AdminClientSecret { get; set; }
    }
}
