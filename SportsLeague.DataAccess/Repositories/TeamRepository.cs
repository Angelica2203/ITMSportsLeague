using Microsoft.EntityFrameworkCore;
using SportsLeague.DataAccess.Context;
using SportsLeague.Domain.Entities;
using SportsLeague.Domain.Interfaces.Repositories;

namespace SportsLeague.DataAccess.Repositories
{
    public class TeamRepository : GenericRepository<Team>, ITeamRepository
    {
        public TeamRepository(LeagueDbContext context) : base(context)
        {
        }

        public async Task<Team?> GetByNameAsync(string name)//aqui se devuelve un objeto de tipo Team
        {
            return await _dbSet
                .FirstOrDefaultAsync(t => t.Name.ToLower() == name.ToLower());//firstordefault devuelve el primer elemento que cumple la condición o null si no encuentra ninguno
        }

        public async Task<IEnumerable<Team>> GetByCityAsync(string city)//aqui devuelve una lista de objetos de tipo team
        {
            return await _dbSet
                .Where(t => t.City.ToLower() == city.ToLower())
                .ToListAsync();
        }
    }

}
