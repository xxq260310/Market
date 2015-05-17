using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Portal.Models
{
    public class Order
    {
        [Key]
        public int OrderId { get; set; }
        public Nullable<int> BuyerId { get; set; }
        public string Contact { get; set; }
        public Nullable<double> TotalCost { get; set; }
        public string Address { get; set; }
        public string State { get; set; }
        public string Delivery { get; set; }
        public string ConsigneeName { get; set; }
        public Nullable<int> SellerId { get; set; }
        public Nullable<System.DateTime> CreationDate { get; set; }
        public Nullable<System.DateTime> UpdateDate { get; set; }

        public virtual OrderDetail OrderDetail { get; set; }
    }
}