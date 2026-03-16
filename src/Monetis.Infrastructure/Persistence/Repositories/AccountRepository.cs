using Monetis.Domain.Entities;
using Monetis.Domain.Interfaces;
using Monetis.Infrastructure.Contexts;

namespace Monetis.Infrastructure.Persistence.Repositories;

public class AccountRepository(MonetisDataContext context) : BaseRepository<Account>(context),IAccountRepository
{
    
}