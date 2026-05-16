namespace SportsLeague.Domain.Interfaces.Services
{
    public interface IStandingsService
    {
        Task<object> GetStandingsAsync(int tournamentId);//Obtener la tabla de posiciones del torneo
        Task<object> GetTopScorersAsync(int tournamentId); //Obtener los máximos goleadores del torneo
        Task<object> GetCardStatsAsync(int tournamentId); //Obtener las estadísticas de tarjetas (amarillas y rojas) por jugador en el torneo
    }

}
