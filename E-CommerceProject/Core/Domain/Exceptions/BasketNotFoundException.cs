﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Exceptions
{
    public sealed class BasketNotFoundException (string id)
        : NotFoundException ($"Basket with Id {id} Not Found")
    {
    }
}
