using GymManagement.Domain.Entities;
using GymManagement.Domain.Interfaces;
using GymManagement.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace GymManagement.Infrastructure.Repositories;

/// <summary>
/// EF Core implementation of IClientRepository. (DIP - implements abstraction)
/// (SRP - Only handles client data persistence)
/// (LSP - can substitute IClientRepository anywhere)
/// </summary>
public class ClientRepository : IClientRepository
{
    private readonly GymDbContext _context;

    public ClientRepository(GymDbContext context)
    {
        _context = context;
    }

    public async Task<Client?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        => await _context.Clients
            .Include(c => c.Memberships)
            .Include(c => c.WorkoutSessions)
                .ThenInclude(ws => ws.Exercises)
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);

    public async Task<IEnumerable<Client>> GetAllAsync(CancellationToken cancellationToken = default)
        => await _context.Clients.ToListAsync(cancellationToken);

    public async Task<bool> ExistsByEmailAsync(string email, CancellationToken cancellationToken = default)
        => await _context.Clients.AnyAsync(c => c.Email == email, cancellationToken);

    public async Task AddAsync(Client client, CancellationToken cancellationToken = default)
        => await _context.Clients.AddAsync(client, cancellationToken);

    public async Task UpdateAsync(Client client, CancellationToken cancellationToken = default)
    {
        _context.Clients.Update(client);
        await Task.CompletedTask;
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        => await _context.SaveChangesAsync(cancellationToken);
}
