namespace KubernetesPodsAPI.Models
{
    public class Container
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string ContainerUrl { get; set; }
        public int ClusterId { get; set; }
    }
}
