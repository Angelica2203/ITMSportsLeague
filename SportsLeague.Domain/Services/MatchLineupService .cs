using Microsoft.Extensions.Logging;
using SportsLeague.Domain.Entities;
using SportsLeague.Domain.Enums;
using SportsLeague.Domain.Helpers;
using SportsLeague.Domain.Interfaces.Repositories;
using SportsLeague.Domain.Interfaces.Services;

namespace SportsLeague.Domain.Services
{
    public class MatchLineupService : IMatchLineupService
    {
        private readonly IMatchRepository _matchRepository;
        private readonly IMatchLineupRepository _matchLineupRepository;
        private readonly MatchValidationHelper _validationHelper;
        private readonly ILogger<MatchLineupService> _logger;

        public MatchLineupService(
            IMatchRepository matchRepository,
            IMatchLineupRepository matchLineupRepository,
            MatchValidationHelper validationHelper,
            ILogger<MatchLineupService> logger)
        {
            _matchRepository = matchRepository;
            _matchLineupRepository = matchLineupRepository;
            _validationHelper = validationHelper;
            _logger = logger;
        }


        public async Task<MatchLineup> RegisterLineupAsync(
            int matchId, MatchLineup lineup)
        {
            var match = await _matchRepository.GetByIdAsync(matchId);

            if (match == null)
                throw new KeyNotFoundException(
                    $"No se encontró el partido con ID {matchId}");

            if (match.Status != MatchStatus.Scheduled)
                throw new InvalidOperationException(
                    "Solo se pueden registrar alineaciones en partidos Scheduled");

            var player = await _validationHelper
                .ValidatePlayerInMatchAsync(lineup.PlayerId, match);

            var alreadyExists = await _matchLineupRepository
                .ExistsAsync(matchId, lineup.PlayerId);

            if (alreadyExists)
                throw new InvalidOperationException(
                    "El jugador ya está registrado en la alineación de este partido");

            if (lineup.IsStarter)
            {
                var startersCount = await _matchLineupRepository
                    .CountStartersByTeamAsync(matchId, player.TeamId);

                if (startersCount >= 11)
                    throw new InvalidOperationException(
                        "El equipo ya tiene 11 titulares registrados en este partido");
            }

            lineup.MatchId = matchId;

            _logger.LogInformation(
                "Registering lineup: Match {MatchId}, Player {PlayerId}, Starter {IsStarter}",
                matchId, lineup.PlayerId, lineup.IsStarter);

            return await _matchLineupRepository.CreateAsync(lineup);
        }

        public async Task<IEnumerable<MatchLineup>> GetLineupByMatchAsync(int matchId)
        {
            var match = await _matchRepository.GetByIdAsync(matchId);

            if (match == null)
                throw new KeyNotFoundException(
                    $"No se encontró el partido con ID {matchId}");

            return await _matchLineupRepository
                .GetByMatchWithDetailsAsync(matchId);
        }

        public async Task<IEnumerable<MatchLineup>> GetLineupByMatchAndTeamAsync(
            int matchId, int teamId)
        {
            var match = await _matchRepository.GetByIdAsync(matchId);

            if (match == null)
                throw new KeyNotFoundException(
                    $"No se encontró el partido con ID {matchId}");

            return await _matchLineupRepository
                .GetByMatchAndTeamAsync(matchId, teamId);
        }

        public async Task DeleteLineupAsync(int lineupId)
        {
            var exists = await _matchLineupRepository.ExistsAsync(lineupId);

            if (!exists)
                throw new KeyNotFoundException(
                    $"No se encontró la alineación con ID {lineupId}");

            await _matchLineupRepository.DeleteAsync(lineupId);
        }
    }
}
