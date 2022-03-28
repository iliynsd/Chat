using Microsoft.Extensions.Configuration;

namespace Chat
{
    public class Configuration
    {
        private readonly IConfiguration _configuration;

        public Configuration(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string GetSection(string name) => _configuration.GetSection(name).Value;
    }
}