using System;
using System.Collections.Generic;

namespace Library.Models
{
    public partial class Order
    {
        public Order()
        {
            OrderDetails = new HashSet<OrderDetail>();
        }

        public int Id { get; set; }
        public int UserId { get; set; }
        public string Status { get; set; } = null!;
        public string? Longitude { get; set; }
        public string? Latitude { get; set; }
        public int CourierId { get; set; }

        public virtual Courier Courier { get; set; } = null!;
        public virtual ICollection<OrderDetail> OrderDetails { get; set; }
    }
}
