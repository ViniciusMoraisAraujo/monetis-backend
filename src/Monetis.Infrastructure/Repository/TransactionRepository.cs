using Microsoft.EntityFrameworkCore;
using Monetis.Domain.Entities;
using Monetis.Domain.Interfaces;
using Monetis.Infrastructure.Data;

namespace Monetis.Infrastructure.Repository;

public class TransactionRepository(MonetisDataContext context) : BaseRepository<Transaction>(context),ITransactionRepository
{
    
}