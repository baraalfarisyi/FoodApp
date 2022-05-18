using HotChocolate.AspNetCore.Authorization;
using Library.Models;

namespace FoodService.GraphQL
{
    public class Query
    {
        [Authorize(Roles = new[] { "MANAGER", "BUYER" })]
        public IQueryable<Food>GetFoods([Service] IndividuProjContext context) =>
            context.Foods;
    }
}
