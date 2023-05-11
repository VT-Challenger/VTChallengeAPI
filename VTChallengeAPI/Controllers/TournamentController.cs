using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NugetVTChallenge.Interfaces;

namespace VTChallengeAPI.Controllers {
    [Route("api/[controller]")]
    [ApiController]
    public class TournamentController : ControllerBase {

        private IVtChallenge repo;

        public TournamentController(IVtChallenge repo) {
            this.repo = repo;
        }


    }
}
