using Monetis.Domain.Entities;
using Monetis.Domain.Interfaces;
using Monetis.Infrastructure.Data;

namespace Monetis.Infrastructure.Repository;

public class AccountRepository(MonetisDataContext context) : BaseRepository<Account>(context),IAccountRepository
{
    
}