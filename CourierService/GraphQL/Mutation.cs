using HotChocolate.AspNetCore.Authorization;
using Library.Models;

namespace CourierService.GraphQL
{
    public class Mutation
    {
        [Authorize(Roles = new[] { "MANAGER" })]
        public async Task<Courier> DeleteCourierByIdAsync(
            int id,
            [Service] IndividuProjContext context)
        {
            var courier = context.Couriers.Where(o => o.Id == id).FirstOrDefault();
            if (courier != null)
            {
                context.Couriers.Remove(courier);
                await context.SaveChangesAsync();
            }


            return await Task.FromResult(courier);
        }

        [Authorize(Roles = new[] { "MANAGER" })]
        public async Task<Courier> UpdateCourierAsync(
            CourierInput input,
            [Service] IndividuProjContext context)
        {
            var courier = context.Couriers.Where(o => o.Id == input.Id).FirstOrDefault();
            if (courier != null)
            {
                courier.Nama = input.Nama;
                courier.Email = input.Email;

                context.Couriers.Update(courier);
                await context.SaveChangesAsync();
            }
            return await Task.FromResult(courier);
        }
    }
}
