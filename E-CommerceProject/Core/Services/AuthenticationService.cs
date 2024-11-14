﻿using Shared.AuthModels;

namespace Services
{
    public class AuthenticationService(UserManager<User> userManager ,
        IOptions<JwtOptions> options, IMapper mapper) : IAuthenticationService
    {

        public async Task<UserResultDTO> LoginAsync(LoginDTO loginModel)
        {
            // Check if there is user under this Email
            var user = await userManager.FindByEmailAsync(loginModel.Email);
            if (user == null) throw new UnAuthorizedException("Email Doesn't Exist");

            // Check if password is correct for this Email
            var result = await userManager.CheckPasswordAsync(user, loginModel.Password);
            if(!result == true) throw new UnAuthorizedException();

            // create Token and Return Response
            return new UserResultDTO(
                user.DisplayName,
                user.Email,
                await CreateTokenAsync(user));

        }

        public async Task<UserResultDTO> RegisterAsync(UserRegisterDTO registerModel)
        {
            var user = new User()
            {
                Email = registerModel.Email,
                DisplayName = registerModel.DisplayName,
                PhoneNumber = registerModel.PhoneNumber,
                UserName = registerModel.UserName,
            };

            var result = await userManager.CreateAsync(user , registerModel.Password);

            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(e => e.Description).ToList();
                
                throw new ValidationException(errors); 
            }

            return new UserResultDTO(
                user.DisplayName,
                user.Email,
                await CreateTokenAsync(user));

        }
        
        private async Task<string> CreateTokenAsync(User user)
        {
            var jwtOptions = options.Value;
            // private claims
            var authClaims = new List<Claim>
            {
                new Claim(ClaimTypes.Name ,user.UserName!),
                new Claim(ClaimTypes.Email ,user.Email!)
            };

            // Add Roles To claims if Exist
            var roles = await userManager.GetRolesAsync(user);

            foreach (var role in roles)
                authClaims.Add(new Claim(ClaimTypes.Role, role));

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.SecretKey));
            var signingCreds = new SigningCredentials(key , SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                audience: jwtOptions.Audience,
                issuer: jwtOptions.Issure,
                expires: DateTime.UtcNow.AddDays(jwtOptions.DurationInDays),
                claims: authClaims,
                signingCredentials: signingCreds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        
        public async Task<bool> CheckEmailExist(string email)
        {
            var user = await userManager.FindByEmailAsync(email);

            return user != null;
        }

        public async Task<AddressDTO> GetUserAddress(string email)
        {
            var user = await userManager.Users.Include(u=>u.Address)
                .FirstOrDefaultAsync(u=> u.Email == email)
                ?? throw new UserNotFoundException(email);

            return mapper.Map<AddressDTO>(user.Address);

            //return new AddressDTO
            //{
            //    City = user.Address.City,
            //    Country = user.Address.Country,
            //    FirstName = user.Address.FristName,
            //    LastName = user.Address.LastName,
            //    Street = user.Address.Street,
            //};
        }

        public async Task<UserResultDTO> GetUserByEmail(string email)
        {
            var user = await userManager.FindByEmailAsync (email)
                ?? throw new UserNotFoundException(email);

            return new UserResultDTO(
            user.DisplayName,
            user.Email,
            await CreateTokenAsync(user));
        }

        public async Task<AddressDTO> UpdateUserAddress(AddressDTO address , string email)
        {
            var user = await userManager.Users.Include(u => u.Address)
            .FirstOrDefaultAsync(u => u.Email == email)
            ?? throw new UserNotFoundException(email);

            if (user.Address != null)
            {
                user.Address.FirstName = address.FirstName;
                user.Address.LastName = address.LastName;
                user.Address.Street = address.Street;
                user.Address.City = address.City;
                user.Address.Country = address.Country;
            }
            else
            {
                var userAddress = mapper.Map<Address>(address);
                user.Address = userAddress;
            }
            await userManager.UpdateAsync(user);

            return address;
        }
    }
}
