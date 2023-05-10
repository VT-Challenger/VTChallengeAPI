using NugetVTChallenge.Models.Api;

namespace VTChallenge.Services
{
    public interface IServiceValorant {

        Task<UserApi> GetAccountAsync(string username, string tagline);
        Task<UserApi> GetAccountUidAsync(string uid);
        Task<string> GetRankAsync(string username, string tag); 

    }
}
