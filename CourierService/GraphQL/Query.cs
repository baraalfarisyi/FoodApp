using HotChocolate.AspNetCore.Authorization;
using Library.Models;
using Microsoft.EntityFrameworkCore;

namespace CourierService.GraphQL
{
    public class Query
    {
        //[Authorize(Roles = new[] { "MANAGER" })]
        //public IQueryable<Courier> GetCourier([Service] IndividuProjContext context) =>
        //    context.Couriers;

        //[Authorize(Roles = new[] { "MANAGER" })]
        //public IQueryable<User> GetCourier([Service] IndividuProjContext context) =>
        //    context.Users.Include(x => x.Profiles).Include(o => o.Couriers);

        [Authorize(Roles = new[] { "MANAGER" })]
        public IQueryable<CourierData> GetCourierss([Service] IndividuProjContext context)
        {
            var users = context.Users.Include(x => x.Profiles).Include(o => o.Couriers).ToList();
            List<CourierData> courierDatas = new List<CourierData>();
            foreach (var user in users)
            {
                if (user.Profiles.Count == 0 || user.Couriers.Count == 0) continue;
                CourierData courierData = new CourierData();
                foreach (var profile in user.Profiles)
                {
                    courierData.Nama = profile.Name;
                    courierData.phone = profile.Phone;
                }
                foreach (var courier in user.Couriers)
                {
                    courierData.Id = courier.Id;
                }
                if (courierData.Id != null)
                {
                    courierDatas.Add(courierData);
                }
            }
            return courierDatas.AsQueryable();
        }
    }
}
