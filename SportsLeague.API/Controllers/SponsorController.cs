using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using SportsLeague.API.DTOs.Request;
using SportsLeague.API.DTOs.Response;
using SportsLeague.Domain.Entities;
using SportsLeague.Domain.Interfaces.Services;

namespace SportsLeague.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SponsorController : ControllerBase
    {
        private readonly ISponsorService _sponsorService;
        private readonly IMapper _mapper;

        public SponsorController(
            ISponsorService sponsorService,
            IMapper mapper)
        {
            _sponsorService = sponsorService;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<SponsorResponseDTO>>> GetAll()
        {
            var sponsors = await _sponsorService.GetAllAsync();
            return Ok(_mapper.Map<IEnumerable<SponsorResponseDTO>>(sponsors));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<SponsorResponseDTO>> GetById(int id)
        {
            var sponsor = await _sponsorService.GetByIdAsync(id);

            if (sponsor == null)
                return NotFound(new { message = $"Sponsor con ID {id} no encontrado" });

            return Ok(_mapper.Map<SponsorResponseDTO>(sponsor));
        }

        [HttpPost]
        public async Task<ActionResult<SponsorResponseDTO>> Create(SponsorRequestDTO dto)
        {
            try
            {
                var sponsor = _mapper.Map<Sponsor>(dto);
                var created = await _sponsorService.CreateAsync(sponsor);
                var responseDto = _mapper.Map<SponsorResponseDTO>(created);

                return CreatedAtAction(nameof(GetById), new { id = responseDto.Id }, responseDto);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Update(int id, SponsorRequestDTO dto)
        {
            try
            {
                var sponsor = _mapper.Map<Sponsor>(dto);
                await _sponsorService.UpdateAsync(id, sponsor);

                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            try
            {
                await _sponsorService.DeleteAsync(id);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        [HttpPost("{sponsorId}/tournaments")]
        public async Task<ActionResult<TournamentSponsorResponseDTO>> LinkTournament(
            int sponsorId,
            TournamentSponsorRequestDTO dto)
        {
            try
            {
                var relation = await _sponsorService.LinkTournamentAsync(
                    sponsorId,
                    dto.TournamentId,
                    dto.ContractAmount);

                var response = new TournamentSponsorResponseDTO
                {
                    Id = relation.Id,
                    TournamentId = relation.TournamentId,
                    TournamentName = relation.Tournament?.Name ?? string.Empty,
                    SponsorId = relation.SponsorId,
                    SponsorName = relation.Sponsor?.Name ?? string.Empty,
                    ContractAmount = relation.ContractAmount,
                    JoinedAt = relation.JoinedAt
                };

                return StatusCode(StatusCodes.Status201Created, response);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("{sponsorId}/tournaments")]
        public async Task<ActionResult<IEnumerable<TournamentResponseDTO>>> GetTournamentsBySponsor(int sponsorId)
        {
            try
            {
                var tournaments = await _sponsorService.GetTournamentsBySponsorAsync(sponsorId);

                return Ok(_mapper.Map<IEnumerable<TournamentResponseDTO>>(tournaments));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        [HttpDelete("{sponsorId}/tournaments/{tournamentId}")]
        public async Task<ActionResult> UnlinkTournament(int sponsorId, int tournamentId)
        {
            try
            {
                await _sponsorService.UnlinkTournamentAsync(sponsorId, tournamentId);

                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }
    }
}
