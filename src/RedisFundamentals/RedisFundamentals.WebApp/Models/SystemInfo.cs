namespace RedisFundamentals.WebApp.Models
{
    public class SystemInfo
    {
        public SystemInfo(string local, string kernel, string targetFramework)
        {
            Local = local;
            Kernel = kernel;
            TargetFramework = targetFramework;
        }

        public string Local { get; }
        public string Kernel { get; }
        public string TargetFramework { get; }
    }
}
