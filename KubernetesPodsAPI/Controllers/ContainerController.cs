using KubernetesPodsAPI.Data;
using KubernetesPodsAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KubernetesPodsAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContainerController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ContainerController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost("uploadcontainer")]
        public async Task<ActionResult> UploadContainer(IFormFile file, int clusterId, string username, string containername)
        {

            if (file == null || file.Length == 0)
            {
                return Ok("File not selected");
            }

            var path = Directory.GetCurrentDirectory() + "\\Files\\" + username + "\\Containers\\" + containername + "\\";
            Directory.CreateDirectory(path);

            var filepath = path + file.FileName;

            using (var stream = new FileStream(filepath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            var container = new Container
            {
                Name = containername,
                ContainerUrl = path,
                ClusterId = clusterId
            };

            _context.Containers.Add(container);
            await _context.SaveChangesAsync();

            return Ok(container);
        }

        [HttpGet("getcontainers")]
        public async Task<ActionResult> GetContainers(int clusterId)
        {
            var containers = await _context.Containers.Where(x => x.ClusterId == clusterId).ToListAsync();

            return Ok(containers);
        }
    }
}
