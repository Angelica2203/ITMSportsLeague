using SportsLeague.Domain.Entities;

namespace SportsLeague.Domain.Interfaces.Repositories
{
    public interface ITournamentTeamRepository : IGenericRepository<TournamentTeam>
    {
        Task<TournamentTeam?> GetByTournamentAndTeamAsync(int tournamentId, int teamId);//para obtener la relación entre un torneo y un equipo específico
        Task<IEnumerable<TournamentTeam>> GetByTournamentAsync(int tournamentId);//para obtener todos los equipos registrados en un torneo específico
    }

}
