using System.Net.Mail;
using Microsoft.Extensions.Logging;
using SportsLeague.Domain.Entities;
using SportsLeague.Domain.Interfaces.Repositories;
using SportsLeague.Domain.Interfaces.Services;

namespace SportsLeague.Domain.Services
{
    public class SponsorService : ISponsorService
    {
        private readonly ISponsorRepository _sponsorRepository;
        private readonly ITournamentRepository _tournamentRepository;
        private readonly ITournamentSponsorRepository _tournamentSponsorRepository;
        private readonly ILogger<SponsorService> _logger;

        public SponsorService(
            ISponsorRepository sponsorRepository,
            ITournamentRepository tournamentRepository,
            ITournamentSponsorRepository tournamentSponsorRepository,
            ILogger<SponsorService> logger)
        {
            _sponsorRepository = sponsorRepository;
            _tournamentRepository = tournamentRepository;
            _tournamentSponsorRepository = tournamentSponsorRepository;
            _logger = logger;
        }

        public async Task<IEnumerable<Sponsor>> GetAllAsync()
        {
            _logger.LogInformation("Retrieving all sponsors");
            return await _sponsorRepository.GetAllAsync();
        }

        public async Task<Sponsor?> GetByIdAsync(int id)
        {
            _logger.LogInformation("Retrieving sponsor with ID: {SponsorId}", id);

            var sponsor = await _sponsorRepository.GetByIdAsync(id);

            if (sponsor == null)
                _logger.LogWarning("Sponsor with ID {SponsorId} not found", id);

            return sponsor;
        }

        public async Task<Sponsor> CreateAsync(Sponsor sponsor)
        {
            _logger.LogInformation("Creating sponsor: {SponsorName}", sponsor.Name);

            if (await _sponsorRepository.ExistsByNameAsync(sponsor.Name))
                throw new InvalidOperationException("Ya existe un sponsor con ese nombre");

            try
            {
                var email = new MailAddress(sponsor.ContactEmail);
            }
            catch
            {
                throw new InvalidOperationException("El correo electrónico no tiene un formato válido");
            }

            return await _sponsorRepository.CreateAsync(sponsor);
        }

        public async Task UpdateAsync(int id, Sponsor sponsor)
        {
            var existing = await _sponsorRepository.GetByIdAsync(id);

            if (existing == null)
                throw new KeyNotFoundException($"No se encontró el sponsor con ID {id}");

            if (!existing.Name.Equals(sponsor.Name, StringComparison.OrdinalIgnoreCase))
            {
                if (await _sponsorRepository.ExistsByNameAsync(sponsor.Name))
                    throw new InvalidOperationException("Ya existe un sponsor con ese nombre");
            }

            try
            {
                var email = new MailAddress(sponsor.ContactEmail);
            }
            catch
            {
                throw new InvalidOperationException("El correo electrónico no tiene un formato válido");
            }

            existing.Name = sponsor.Name;
            existing.ContactEmail = sponsor.ContactEmail;
            existing.Phone = sponsor.Phone;
            existing.WebsiteUrl = sponsor.WebsiteUrl;
            existing.Category = sponsor.Category;

            _logger.LogInformation("Updating sponsor with ID: {SponsorId}", id);

            await _sponsorRepository.UpdateAsync(existing);
        }

        public async Task DeleteAsync(int id)
        {
            var exists = await _sponsorRepository.ExistsAsync(id);

            if (!exists)
                throw new KeyNotFoundException($"No se encontró el sponsor con ID {id}");

            _logger.LogInformation("Deleting sponsor with ID: {SponsorId}", id);

            await _sponsorRepository.DeleteAsync(id);
        }

        public async Task<TournamentSponsor> LinkTournamentAsync(int sponsorId, int tournamentId, decimal contractAmount)
        {
            var sponsor = await _sponsorRepository.GetByIdAsync(sponsorId);

            if (sponsor == null)
                throw new KeyNotFoundException($"No se encontró el sponsor con ID {sponsorId}");

            var tournament = await _tournamentRepository.GetByIdAsync(tournamentId);

            if (tournament == null)
                throw new KeyNotFoundException($"No se encontró el torneo con ID {tournamentId}");

            var existingLink = await _tournamentSponsorRepository
                .GetByTournamentAndSponsorAsync(tournamentId, sponsorId);

            if (existingLink != null)
                throw new InvalidOperationException("El sponsor ya está vinculado a este torneo");

            if (contractAmount <= 0)
                throw new InvalidOperationException("El monto del contrato debe ser mayor a 0");

            var tournamentSponsor = new TournamentSponsor
            {
                TournamentId = tournamentId,
                SponsorId = sponsorId,
                ContractAmount = contractAmount,
                JoinedAt = DateTime.UtcNow
            };

            _logger.LogInformation(
                "Linking sponsor ID {SponsorId} to tournament ID {TournamentId}",
                sponsorId, tournamentId);

            var created = await _tournamentSponsorRepository.CreateAsync(tournamentSponsor);

            created.Sponsor = sponsor;
            created.Tournament = tournament;

            return created;
        }

        public async Task<IEnumerable<Tournament>> GetTournamentsBySponsorAsync(int sponsorId)
        {
            var sponsor = await _sponsorRepository.GetByIdAsync(sponsorId);

            if (sponsor == null)
                throw new KeyNotFoundException($"No se encontró el sponsor con ID {sponsorId}");

            var relations = await _tournamentSponsorRepository.GetBySponsorAsync(sponsorId);

            return relations.Select(ts => ts.Tournament);
        }

        public async Task UnlinkTournamentAsync(int sponsorId, int tournamentId)
        {
            var relation = await _tournamentSponsorRepository
                .GetByTournamentAndSponsorAsync(tournamentId, sponsorId);

            if (relation == null)
                throw new KeyNotFoundException("No existe la relación entre sponsor y torneo");

            _logger.LogInformation(
                "Removing sponsor ID {SponsorId} from tournament ID {TournamentId}",
                sponsorId, tournamentId);

            await _tournamentSponsorRepository.DeleteAsync(relation.Id);
        }
    }
}
