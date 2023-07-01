using KubernetesPodsAPI.Data;
using KubernetesPodsAPI.Models;
using KubernetesPodsAPI.Helpers;
using k8s;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KubernetesPodsAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PodsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IKubernetes _kubernetesClient;

        public PodsController(ApplicationDbContext context)
        {
            _context = context;
            var config = KubernetesClientConfiguration.BuildDefaultConfig();
            _kubernetesClient = new Kubernetes(config);
        }

        [HttpGet("getclusterinfo")]
        public async Task<ActionResult> GetClusterInfo()
        {
            var podList = await _kubernetesClient.CoreV1.ListNamespacedPodAsync("default");

            var clusterInfo = new List<IEnumerable<string>>
            {
                new string[] {"NAME:"},
                podList.Items.Select(pod => pod.Metadata.Name),
                new string[] {"STATUS:"},
                podList.Items.Select(pod => pod.Status.Phase),
                new string[] {"IP:"},
                podList.Items.Select(pod => pod.Status.PodIP)
            };

            return Ok(clusterInfo);
        }

        [HttpPost("uploadconfig")]
        public async Task<ActionResult> UploadConfigFile(IFormFile file, string type, int clusterId, string username)
        {
            var path = Directory.GetCurrentDirectory() + "\\Files\\" + username + "\\ConfigFiles\\";
            Directory.CreateDirectory(path);

            var filepath = path + file.FileName;

            using (var stream = new FileStream(filepath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            var config = new ConfigFile
            {
                Name = file.FileName,
                Type = type,
                ConfigfileUrl = filepath,
                ClusterId = clusterId
            };

            _context.ConfigFiles.Add(config);
            await _context.SaveChangesAsync();

            return Ok(config);
        }

        [HttpGet("getconfigfiles")]
        public async Task<ActionResult> GetConfigfiles(int clusterId)
        {
            var configfiles = await _context.ConfigFiles.Where(x => x.ClusterId == clusterId).ToListAsync();

            return Ok(configfiles);
        }

        [HttpPost("addpod")]
        public async Task<ActionResult> AddPod(string containername, string configfilename, int clusterId)
        {
            var container = await _context.Containers.Where(c => c.Name == containername).FirstOrDefaultAsync();
            var configfile = await _context.ConfigFiles.Where(x => x.Name == configfilename).FirstOrDefaultAsync();

            RunProcess.Make("docker", $"build -t {container.Name} {container.ContainerUrl}");
            RunProcess.Make("docker", $"run --name {container.Name} {container.Name}");

            RunProcess.Make("kubectl", $"set image {container.Name}");

            RunProcess.Make("kubectl", $"apply -f {configfile.ConfigfileUrl}");

            var pod = new Pod
            {
                ContainerId = container.Id,
                ConfigId = configfile.Id,
                ClusterId = clusterId
            };

            _context.Pods.Add(pod);
            await _context.SaveChangesAsync();

            return Ok(pod);
        }
    }
}
