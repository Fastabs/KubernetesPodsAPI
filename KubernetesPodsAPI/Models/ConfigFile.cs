namespace KubernetesPodsAPI.Models
{
    public class ConfigFile
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public string ConfigfileUrl { get; set; }
        public int ClusterId { get; set; }
    }
}
