namespace RedisFundamentals.WebApp.Config
{
    using Microsoft.Extensions.Options;

    public sealed class ConnectionStringOptions : IOptions<ConnectionStringOptions>
    {
        public string AppConfiguration { get; set; }
        public string Redis { get; set; }
        public ConnectionStringOptions Value => this;
    }
}
