using KubernetesPodsAPI.Data;
using KubernetesPodsAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KubernetesPodsAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClusterController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ClusterController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("getclusters")]
        public async Task<ActionResult> GetClusters(string username)
        {
            var userId = await _context.Users.Where(n => n.Login == username).Select(n => n.Id).FirstOrDefaultAsync();

            var clusters = await _context.Clusters.Where(x => x.UserId == userId).ToListAsync();

            return Ok(clusters);
        }

        [HttpPost("add")]
        public async Task<ActionResult> Add(string username, string name)
        {
            var userId = await _context.Users.Where(n => n.Login == username).Select(n => n.Id).FirstOrDefaultAsync();

            var cluster = new Cluster
            {
                Name = name,
                UserId = userId
            };

            _context.Clusters.Add(cluster);
            await _context.SaveChangesAsync();
            return Ok(cluster);
        }
    }
}
