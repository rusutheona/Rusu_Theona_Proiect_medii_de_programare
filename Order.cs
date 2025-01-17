namespace ChloeShopModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Order")]
    public partial class Order
    {
        public int OrderId { get; set; }

        public int? CustId { get; set; }

        public int? ProductId { get; set; }

        public virtual Customer Customer { get; set; }

        public virtual Inventory Inventory { get; set; }
    }
}
