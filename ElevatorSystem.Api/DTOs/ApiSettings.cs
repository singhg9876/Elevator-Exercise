namespace ElevatorSystem.Api.DTOs
{
    public class ApiSettings
    {
        // get it from Appsettings.json or environment variable
        public string ApiKey { get; set; } = "SuperSecretKey123";
    }
}
