﻿global using Services.Specifications;
using Stripe.Forwarding;

namespace Services
{
    internal class PaymentService(IBasketRepository basketRepository ,
        IUnitOfWork unitOfWork , IMapper mapper , IConfiguration configuration)
        : IPaymentService
    {
        public async Task<BasketDTO> CreateOrUpdatePaymentIntentAsync(string basketId)
        {
            // Get SecretKey
            StripeConfiguration.ApiKey = configuration.GetRequiredSection("StripeSettings")["SecretKey"];
            
            // Get Basket => ProductRepo => SubTotal
            var basket = await basketRepository.GetBasketAsync(basketId)
            ?? throw new BasketNotFoundException(basketId);

            foreach (var item in basket.Items)
            {
                var product = await unitOfWork.GetRepository<Product, int>()
                    .GetAsync(item.Id)
                    ?? throw new ProductNotFoundException(item.Id);

                item.Price = product.Price;
            }

            if (!basket.DeliveryMethodId.HasValue) throw new DeliveryMethodIdNotFoundException();

            var method = await unitOfWork.GetRepository<DeliveryMethod, int>()
                .GetAsync(basket.DeliveryMethodId.Value)
                ?? throw new DeliveryMethodIdNotFoundException(basket.DeliveryMethodId.Value);

            basket.ShippingPrice = method.Price;

            var amount = (long)(basket.Items.Sum(item => item.Quantity * item.Price) + basket.ShippingPrice) * 100;

            var service = new PaymentIntentService();

            if (string.IsNullOrWhiteSpace(basket.PaymentIntentId))
            {
                // Create
                var createOptions = new PaymentIntentCreateOptions
                {
                    Amount = amount,
                    Currency = "USD",
                    PaymentMethodTypes = new List<string> { "card" }
                };

                var paymentIntent =  await service.CreateAsync(createOptions);

                basket.PaymentIntentId = paymentIntent.Id;

                basket.ClientSecret = paymentIntent.ClientSecret;
            }
            else 
            {
                // Update
                var updateOptions = new PaymentIntentUpdateOptions
                {
                    Amount = amount,
                };

               await service.UpdateAsync(basket.PaymentIntentId, updateOptions);
            }

            await basketRepository.UpdateBasketAsync(basket);

            return mapper.Map<BasketDTO>(basket);
        }

        public async Task UpdateOrderPaymentStatus(string request, string stripHeader)
        {
            var endPointSecret = configuration.GetRequiredSection("StripeSettings")["EndPointSecret"];

            var stripeEvent = EventUtility.ConstructEvent(request,stripHeader,endPointSecret);

            var paymentIntent = stripeEvent.Data.Object as PaymentIntent;

            switch (stripeEvent.Type)
            {
                case EventTypes.PaymentIntentPaymentFailed:
                    await UpdatePaymentStatusFailed(paymentIntent.Id);
                    break;
                case EventTypes.PaymentIntentSucceeded:
                    await UpdatePaymentStatusReceived(paymentIntent.Id);
                    break;
            }
        }

        private async Task UpdatePaymentStatusFailed(string paymentIntentId)
        {
            var order = await unitOfWork.GetRepository<Order, Guid>()
                .GetAsync(new OrderWithPaymentIntentIdSpecifications(paymentIntentId))
                ?? throw new Exception();

            order.PaymentStatus = OrderPaymentStatus.PaymentFailed;

            unitOfWork.GetRepository<Order,Guid>().Update(order);

            await unitOfWork.SaveChangesAsync();
        }
        private async Task UpdatePaymentStatusReceived(string paymentIntentId)
        {
            var order = await unitOfWork.GetRepository<Order, Guid>()
                .GetAsync(new OrderWithPaymentIntentIdSpecifications(paymentIntentId))
                ?? throw new Exception();

            order.PaymentStatus = OrderPaymentStatus.PaymentReceived;

            unitOfWork.GetRepository<Order, Guid>().Update(order);

            await unitOfWork.SaveChangesAsync();
        }
    }
}