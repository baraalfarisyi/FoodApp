using System;
using System.Collections.Generic;

namespace Library.Models
{
    public partial class OrderDetail
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public int FoodsId { get; set; }
        public int Quantity { get; set; }

        public virtual Food Foods { get; set; } = null!;
        public virtual Order Order { get; set; } = null!;
    }
}
