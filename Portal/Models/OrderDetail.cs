using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Portal.Models
{
    public class OrderDetail
    {
        public OrderDetail()
        {
            this.OrderCommodityDetails = new HashSet<CommodityInOrderDetail>();
        }

        [Key]
        public int OrderId { get; set; }
        public Nullable<double> Cost { get; set; }

        public virtual ICollection<CommodityInOrderDetail> OrderCommodityDetails { get; set; }
        public virtual Order Order { get; set; }
    }
}