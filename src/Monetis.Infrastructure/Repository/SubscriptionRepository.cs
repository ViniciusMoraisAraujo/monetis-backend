using Microsoft.EntityFrameworkCore;
using Monetis.Domain.Entities;
using Monetis.Domain.Interfaces;
using Monetis.Infrastructure.Data;

namespace Monetis.Infrastructure.Repository;

public class SubscriptionRepository(MonetisDataContext context) : BaseRepository<Subscription>(context), ISubscriptionRepository
{
    
}