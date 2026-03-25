using SportsLeague.Domain.Enums;

namespace SportsLeague.API.DTOs.Response
{
    public class TournamentResponseDTO
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Season { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public TournamentStatus Status { get; set; }
        public int TeamsCount { get; set; }// ad hoc para mostrar la cantidad de equipos registrados en el torneo
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

}
