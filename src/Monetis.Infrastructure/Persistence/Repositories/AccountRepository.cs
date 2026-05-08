using Monetis.Domain.Entities;
using Monetis.Application.Abstractions.Persistence;
using Monetis.Infrastructure.Persistence.Contexts;

namespace Monetis.Infrastructure.Persistence.Repositories;

public class AccountRepository(MonetisDataContext context) : BaseRepository<Account>(context),IAccountRepository
{
    
}