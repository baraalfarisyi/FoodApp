﻿using HotChocolate.AspNetCore.Authorization;
using Library.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace OrderService.GraphQL
{
    public class Query
    {
        [Authorize(Roles = new[] { "MANAGER", "BUYER" })]
        public IQueryable<Order> GetOrders([Service] IndividuProjContext context, ClaimsPrincipal claimsPrincipal)
        {
            var userName = claimsPrincipal.Identity.Name;

            // check manager role ?
            var managerRole = claimsPrincipal.Claims.Where(o => o.Type == ClaimTypes.Role && o.Value == "MANAGER").FirstOrDefault();
            var user = context.Users.Where(o => o.Username == userName).FirstOrDefault();
            if (user != null)
            {
                if (managerRole != null)
                    return context.Orders.Include(o => o.OrderDetails);

                var orders = context.Orders.Where(o => o.UserId == user.Id).Include(o => o.OrderDetails);
                return orders.AsQueryable();
            }


            return new List<Order>().AsQueryable();
        }

        //[Authorize]
        //public IQueryable<OrderDetail> GetBuyerOrder([Service] IndividuProjContext context, ClaimsPrincipal claimsPrincipal)
        //{
        //    var userName = claimsPrincipal.Identity.Name;

        //    var buyerRole = claimsPrincipal.Claims.Where(o => o.Type == ClaimTypes.Role && o.Value == "BUYER").FirstOrDefault();
        //    var user = context.Users.Where(p => p.Username == userName).FirstOrDefault();
        //    if (user != null)
        //    {
        //        var orders = context.Orders.Where(o => o.UserId == user.Id).ToList();
        //        List<OrderDetail> ordersDetail = new List<OrderDetail>();
        //        foreach (var order in orders)
        //        {
        //            var orderDetail = context.OrderDetails.Where(o => o.OrderId == order.Id).FirstOrDefault();
        //            ordersDetail.Add(orderDetail);
        //        }
        //        return ordersDetail.AsQueryable();

        //    }
        //    else
        //    {
        //        return new List<OrderDetail>().AsQueryable();
        //    }
        //}
    }
    }
