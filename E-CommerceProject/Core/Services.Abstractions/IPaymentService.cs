﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared.BasketModels;

namespace Services.Abstractions
{
    public interface IPaymentService
    {
        public Task<BasketDTO> CreateOrUpdatePaymentIntentAsync(string basketId);

        public Task UpdateOrderPaymentStatus(string request , string stripHeader);
    }
}
