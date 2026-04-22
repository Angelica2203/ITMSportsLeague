using Microsoft.EntityFrameworkCore;
using SportsLeague.DataAccess.Context;
using SportsLeague.Domain.Interfaces.Repositories;
using SportsLeague.Domain.Entities;

namespace SportsLeague.DataAccess.Repositories
{
    public class MatchRepository : GenericRepository<Match>, IMatchRepository
    {
        public MatchRepository(LeagueDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Match>> GetByTournamentAsync(int tournamentId)
        {
            return await _dbSet
                .Where(m => m.TournamentId == tournamentId)
                .OrderBy(m => m.Matchday)
                .ThenBy(m => m.MatchDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Match>> GetByTeamAsync(int teamId)
        {
            return await _dbSet
                .Where(m => m.HomeTeamId == teamId || m.AwayTeamId == teamId)
                .Include(m => m.HomeTeam)
                //.ThenInclude(ht => ht.Players)// est es para agregar los jugadores del equipo local
                .Include(m => m.AwayTeam)
                //.ThenInclude(ht => ht.Players)
                .OrderBy(m => m.MatchDate)
                .ToListAsync();
        }

        public async Task<Match?> GetByIdWithDetailsAsync(int id)
        {
            return await _dbSet
                .Where(m => m.Id == id)
                .Include(m => m.Tournament)
                .Include(m => m.HomeTeam)
                .Include(m => m.AwayTeam)
                .Include(m => m.Referee)
                .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<Match>> GetByTournamentWithDetailsAsync(
            int tournamentId)
        {
            return await _dbSet
                .Where(m => m.TournamentId == tournamentId)
                .Include(m => m.HomeTeam)
                .Include(m => m.AwayTeam)
                .Include(m => m.Referee)
                .OrderBy(m => m.Matchday)
                .ThenBy(m => m.MatchDate)
                .ToListAsync();
        }
    }

}
