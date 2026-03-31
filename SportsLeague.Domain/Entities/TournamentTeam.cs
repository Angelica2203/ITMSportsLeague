namespace SportsLeague.Domain.Entities
{
    public class TournamentTeam : AuditBase
    {
        public int TournamentId { get; set; }
        public int TeamId { get; set; }
        public DateTime RegisteredAt { get; set; } = DateTime.UtcNow;//utcnow sirve para registrar la fecha y hora en que un equipo se registró en un torneo.

        // Navigation Properties
        public Tournament Tournament { get; set; } = null!;
        public Team Team { get; set; } = null!;
    }

}
