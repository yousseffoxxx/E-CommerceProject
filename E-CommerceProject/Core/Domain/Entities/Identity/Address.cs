﻿using System.Data;

namespace Domain.Entities.UserEntities
{
    public class Address
    {
        public int Id { get; set; }
        public string FristName { get; set; }
        public string LastName { get; set; }
        public string Street { get; set; }
        public string City { get; set; }
        public string Country { get; set; }

        public User User { get; set; }
        public string UserId { get; set; }
    }
}