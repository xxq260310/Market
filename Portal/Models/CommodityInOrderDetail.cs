using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Portal.Models
{
    public class CommodityInOrderDetail
    {
        [Key]
        public int OrderId { get; set; }
        [Key]
        public int CommodityId { get; set; }
        public Nullable<double> UnitPrice { get; set; }
        public Nullable<int> Quantity { get; set; }

        public virtual Commodity Commodity { get; set; }
        public virtual OrderDetail OrderDetail { get; set; }
    }
}