using KubernetesPodsAPI.Models;

namespace KubernetesPodsAPI.Services
{
    public interface ITokenService
    {
        string CreateToken(User user);
    }
}
