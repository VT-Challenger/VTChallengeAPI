using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using NugetVTChallenge.Interfaces;
using NugetVTChallenge.Models;
using System.Security.Claims;
using VTChallengeAPI.Helpers;

namespace VTChallengeAPI.Controllers {
    [Route("api/[controller]")]
    [ApiController]
    public class TournamentController : ControllerBase {

        private IVtChallenge repo;
        private HelperUserToken helper;

        public TournamentController(IVtChallenge repo, HelperUserToken helper) {
            this.repo = repo;
            this.helper = helper;
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<ActionResult<List<TournamentComplete>>> GetAllTournaments() {
            return await this.repo.GetTournaments();
        }

        [HttpGet]
        [Route("[action]/{tid}")]
        public async Task<ActionResult<TournamentComplete>> GetTournament(int tid) {
            return await this.repo.GetTournamentComplete(tid);
        }

        [HttpGet]
        [Authorize]
        [Route("[action]")]
        public async Task<ActionResult<List<TournamentComplete>>> GetMyTournaments() {
            return await this.repo.GetTournamentsUser(this.helper.GetUserToken().Name);
        }

        [HttpGet]
        [Authorize]
        [Route("[action]/{filtro}")]
        public async Task<ActionResult<List<TournamentComplete>>> GetMyTournamentsFiltereds(string filtro) {
            return await this.repo.GetTournamentsUserFindAsync(this.helper.GetUserToken().Name, filtro);
        }

        [HttpGet]
        [Authorize]
        [Route("[action]")]
        public async Task<ActionResult<List<TournamentComplete>>> GetTournamentsAvailableRank() {
            return await this.repo.GetTournamentsByRankAsync(this.helper.GetUserToken().Rank);
        }

        [HttpGet]
        [Authorize]
        [Route("[action]/{filtro}")]
        public async Task<ActionResult<List<TournamentComplete>>> FindTournamentsAvailableRank(string filtro) {
            return await this.repo.GetTournamentCompletesFindAsync(filtro, this.helper.GetUserToken().Rank);
        }


        [HttpGet]
        [Route("[action]/{tid}")]
        public async Task<ActionResult<List<TournamentPlayers>>> GetPlayers(int tid) {
            return await this.repo.GetPlayersTournament(tid);
        }

        [HttpGet]
        [Route("[action]/{tid}")]
        public async Task<ActionResult<List<Round>>> GetRounds(int tid) {
            return await this.repo.GetRounds(tid);
        }

        [HttpGet]
        [Route("[action]/{tid}")]
        public async Task<ActionResult<List<MatchRound>>> GetMatches(int tid) {
            return await this.repo.GetMatchesTournament(tid);
        }

        [HttpGet]
        [Route("[action]/{tid}")]
        public async Task<ActionResult<List<TournamentPlayers>>> GetWinners(int tid) {
            return await this.repo.GetTournamentWinner(tid);
        }

        [HttpGet]
        [Authorize]
        [Route("[action]/{tid}")]
        public async Task<ActionResult<bool>> ValidateInscription(int tid) {
            Usuario user = this.helper.GetUserToken();
            bool res = await this.repo.ValidateInscription(tid, user.Uid);

            return res;
        }

        [HttpPost]
        [Authorize]
        [Route("[action]/{tid}")]
        public async Task<ActionResult<bool>> Inscription(int tid) {
            Usuario user = this.helper.GetUserToken();
            List<TournamentPlayers> players = await this.repo.GetPlayersTournament(tid);

            if (players.FirstOrDefault(x => x.Uid == user.Uid) == null) {
                await this.repo.InscriptionPlayerTeamAle(tid, user.Uid);
                return true;
            } else {
                return false;
            }
        }


        [HttpDelete]
        [Authorize]
        [Route("[action]/{tid}")]
        public async Task<ActionResult> DeleteTournament(int tid) {
            await this.repo.DeleteTournament(tid, this.helper.GetUserToken().Uid);
            return Ok();
        }



        [HttpPost]
        [Authorize]
        [Route("[action]")]
        public async Task<ActionResult> CreateTournament(InsertTournament objects) {
            int tid = this.repo.GetMaxIdTournament();

            Tournament tournament = JsonConvert.DeserializeObject<Tournament>(objects.JsonTournament);
            await this.repo.InsertTournamentAsync(
                tournament.Tid = tid + 1,
                tournament.Name,
                tournament.Rank,
                tournament.DateInit,
                tournament.Description,
                tournament.Platform,
                tournament.Players,
                this.helper.GetUserToken().Name,
                tournament.Image
            );

            List<Round> rounds = JsonConvert.DeserializeObject<List<Round>>(objects.JsonRounds);
            foreach (Round round in rounds) {
                await this.repo.InsertRoundAsync(
                    round.Name,
                    round.Date,
                    tid + 1
                );
            }

            int roundMatch = this.repo.GetMinIdRoundTournament(tid + 1);
            Round r = await this.repo.FindRoundAsync(roundMatch);
            List<Match> matches = JsonConvert.DeserializeObject<List<Match>>(objects.JsonMatches);
            foreach (Match match in matches) {
                await this.repo.InsertMatchAsync(
                    match.Tblue,
                    match.Tred,
                    r.Date,
                    roundMatch
                );
            }

            return Ok();
        }

        [HttpPut]
        [Authorize]
        [Route("[action]/{tid}")]
        public async Task<ActionResult> UpdateTournament(int tid, List<Match> partidas) {
            int rid = partidas[partidas.Count - 1].Rid;
            foreach (Match match in partidas) {
                await this.repo.UpdateMatchesTournamentAsync(match.Mid, match.Rblue, match.Rred);
            }

            if (this.repo.TotalMatchesRoundWinner(rid) == this.repo.TotalMatchesRound(rid) && await this.repo.TotalMatchesRound(rid) != 0) {
                await this.repo.InsertMatchesNextRoundAsync(rid);
            }
            return Ok();
        }




    }
}
