using HotChocolate.AspNetCore.Authorization;
using Library.Models;

namespace CourierService.GraphQL
{
    public class Query
    {
        [Authorize(Roles = new[] { "MANAGER" })]
        public IQueryable<Courier> GetCourier([Service] IndividuProjContext context) =>
            context.Couriers;

    }
}
