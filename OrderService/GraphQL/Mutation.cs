using HotChocolate.AspNetCore.Authorization;
using Library.Models;
using System.Security.Claims;

namespace OrderService.GraphQL
{
    public class Mutation
    {
        [Authorize(Roles = new[] { "BUYER" })]
        public async Task<OrderData> AddOrderAsync(
            OrderData input,
            ClaimsPrincipal claimsPrincipal,
            [Service] IndividuProjContext context)
        {
            using var transaction = context.Database.BeginTransaction();

            var userName = claimsPrincipal.Identity.Name;
            List<OrderDetailData> orderDetail = new List<OrderDetailData>();
            try
            {
                var user = context.Users.Where(u => u.Username == userName).FirstOrDefault();

                if (user != null)
                {
                    Order order = new Order
                    {
                        UserId = user.Id,
                        CourierId = input.courierId,
                        Status = "Makanan Sedang Di Proses"
                    };
                    //List<OrderDetailData> orderDetail = new List<OrderDetailData>();
                    foreach (var item in input.OrderDetailDatas)
                    {
                        OrderDetail detail = new OrderDetail
                        {
                            OrderId = order.Id,
                            FoodsId = item.FoodsId,
                            Quantity = item.Quantity,
                        };
                        order.OrderDetails.Add(detail);
                        orderDetail.Add(new OrderDetailData(item.FoodsId, item.Quantity));
                        //{
                        //    FoodsId = item.FoodsId,
                        //    Quantity = item.Quantity
                        //});
                    }
                    context.Orders.Add(order);
                    context.SaveChanges();

                    await transaction.CommitAsync();
                    return new OrderData(input.courierId, orderDetail);
                    //{
                    //    courierId = input.courierId,
                    //    OrderDetailDatas = orderDetail
                    //};
                    //return new OrderOutput
                    //{
                    //    TransactionDate = DateTime.Now.ToString(),
                    //    Message = "Berhasil Membuat Order!"
                    //};
                }
                else
                {
                    throw new Exception("User Tidak Ada!");
                }
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                return new OrderData(0, orderDetail);
            }
        }

        [Authorize(Roles = new[] { "COURIER" })]
        public async Task<OrderTrackingData> AddTrackingOrderAsync(
            OrderTrackingData input,
            [Service] IndividuProjContext context)
        {
            var order = context.Orders.FirstOrDefault(x => x.Id == input.OrderId);
            if (order == null)
                return new OrderTrackingData("", "", 0);

            order.Longitude = input.Longitude;
            order.Latitude = input.Latitude;
            order.Status = "Makanan Sedang Diperjalanan";

            context.Orders.Update(order);
            await context.SaveChangesAsync();

            return input;
        }

        [Authorize(Roles = new[] { "COURIER" })]
        public async Task<string> OrderCompleteAsync(
            int id,
            [Service] IndividuProjContext context)
        {
            var order = context.Orders.FirstOrDefault(x => x.Id == id);
            if (order == null)
                return "Order Tidak Ada";

            
            order.Status = "Order Selesai";

            context.Orders.Update(order);
            await context.SaveChangesAsync();

            return "Order Selesai";
        }
    }
}
