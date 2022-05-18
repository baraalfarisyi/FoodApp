using HotChocolate.AspNetCore.Authorization;
using Library.Models;

namespace FoodService.GraphQL
{
    public class Mutation
    {
        [Authorize(Roles = new[] {  "MANAGER" })]
        public async Task<Food> AddFoodAsync(
            FoodInput input,
            [Service] IndividuProjContext context)
        {
            //EF
            var food = new Food
            {
                Name = input.Name,
                Price = input.Price,
                Created = DateTime.Now
            };

            var ret = context.Foods.Add(food);
            await context.SaveChangesAsync();

            return ret.Entity;
        }

        [Authorize(Roles = new[] {  "MANAGER" })]
        public async Task<Food> UpdateFoodAsync(
            FoodInput input,
            [Service] IndividuProjContext context)
        {
            var food = context.Foods.Where(o => o.Id == input.Id).FirstOrDefault();
            if (food != null)
            {
                food.Name = input.Name;

                food.Price = input.Price;

                context.Foods.Update(food);
                await context.SaveChangesAsync();
            }


            return await Task.FromResult(food);
        }

        [Authorize(Roles = new[] { "MANAGER" })]
        public async Task<Food> DeleteFoodByIdAsync(
            int id,
            [Service] IndividuProjContext context)
        {
            var food = context.Foods.Where(o => o.Id == id).FirstOrDefault();
            if (food != null)
            {
                context.Foods.Remove(food);
                await context.SaveChangesAsync();
            }


            return await Task.FromResult(food);
        }
    }
}
