using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using NugetVTChallenge.Interfaces;
using NugetVTChallenge.Models;
using System.Security.Claims;

namespace VTChallengeAPI.Controllers {
    [Route("api/[controller]")]
    [ApiController]
    public class UsuarioController : ControllerBase {

        private IVtChallenge repo;

        public UsuarioController(IVtChallenge repo) {
            this.repo = repo;
        }

        [HttpGet]
        [Route("[action]")]
        [Authorize]
        public async Task<ActionResult<Usuario>> Profile() {
            Claim claim = HttpContext.User.Claims.SingleOrDefault(x => x.Type == "UserData");
            string jsonUsuario = claim.Value;
            Usuario usuario = JsonConvert.DeserializeObject<Usuario>(jsonUsuario);
            return usuario;
        }

        [HttpGet]
        [Route("[action]")]
        [Authorize]
        public async Task<ActionResult> Update() {
            Claim claim = HttpContext.User.Claims.SingleOrDefault(x => x.Type == "UserData");
            string jsonUsuario = claim.Value;
            Usuario usuario = JsonConvert.DeserializeObject<Usuario>(jsonUsuario);

            await this.repo.UpdateProfileAsync(usuario.Uid);

            return Ok("Debe volver a iniciar sesión para ver los cambios");
        }

    }
}
