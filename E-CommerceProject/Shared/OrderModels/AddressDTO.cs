﻿namespace Shared.OrderModels
{
    public record AddressDTO
    {
        public string FirstName { get; init; }
        public string LastName { get; init; }
        public string Street { get; init; }
        public string City { get; init; }
        public string Country { get; init; }

    }
}
