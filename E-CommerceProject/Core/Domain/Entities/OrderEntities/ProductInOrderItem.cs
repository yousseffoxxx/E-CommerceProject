﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities.OrderEntities
{
    public class ProductInOrderItem
    {
        public int Productid { get; set; }
        public string ProductName { get; set; }
        public string PictureUrl { get; set; }

    }
}
