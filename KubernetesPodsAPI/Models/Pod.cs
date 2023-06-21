namespace KubernetesPodsAPI.Models
{
    public class Pod
    {
        public int Id { get; set; }
        public int ContainerId { get; set; }
        public int ConfigId { get; set; }
        public int ClusterId { get; set; }
    }
}
