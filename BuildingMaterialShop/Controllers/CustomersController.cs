using BuildingMaterialShop.ApiModels.CustomerViewModels;
using BuildingMaterialShop.Auth;
using BuildingMaterialShop.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace BuildingMaterialShop.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class CustomersController : ControllerBase
    {
        private readonly BuildingMaterialsShopContext _context;
        private readonly JWTSettings _jwtsettings;

        public CustomersController(IOptions<JWTSettings> jwtsetting, BuildingMaterialsShopContext context)
        {
            _context = context;
            _jwtsettings = jwtsetting.Value;
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Customer>>> GetCustomers()
        {
            return await _context.Customers.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Customer>> GetUser(int id)
        {
            var user = await _context.Customers.FindAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            user.PassWord = null;

            return user;
        }

        [Authorize]
        [HttpPut("ChangePassword")]
        public async Task<IActionResult> ChangePassword(int customerId, string password, string newPassword)
        {
            if (!CustomerExists(customerId))
            {
                return BadRequest();
            }
            var customer = _context.Customers.FirstOrDefault(c => c.CustomerId == customerId && c.PassWord == password);
            if (customer == null)
            {
                return Ok("Mật khẩu cũ không hợp lệ.");
            }

            customer.PassWord = newPassword;
            _context.Entry(customer).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                return Ok("Đổi mật khẩu thất bại.");
            }

            return Ok("Đổi mật khẩu thành công.");

        }

        [HttpPost("Register")]
        public async Task<ActionResult<CustomerViewModel>> Register([FromBody] Customer customer)
        {
            if (customer.Email == null || customer.Email.Length < 10)
            {
                return Ok("Email không hợp lệ.");
            }

            if (EmailExists(customer.Email))
            {
                return Ok("Email đã tồn tại.");
            }
            if (customer.PassWord == null || customer.PassWord.Length < 10)
            {
                return Ok("Mật khẩu không hợp lệ.");
            }

            _context.Customers.Add(customer);

            await _context.SaveChangesAsync();

            customer.PassWord = null;

            return CreatedAtAction("GetUser", new { id = customer.CustomerId }, customer);

        }

        [HttpPost("Login")]
        public async Task<ActionResult<CustomerViewModel>> Login([FromBody] Customer customer)
        {
            customer = await _context.Customers
                                .Where(u => u.Email == customer.Email
                                && u.PassWord == customer.PassWord)
                                .FirstOrDefaultAsync();

            CustomerViewModel customerViewModel = null;

            if (customer != null)
            {
                RefreshTokenCustomer refreshToken = GenerateRefreshToken();
                customer.RefreshTokenCustomers.Add(refreshToken);
                await _context.SaveChangesAsync();

                customerViewModel = new CustomerViewModel(customer);
                customerViewModel.RefreshToken = refreshToken.Token;
            }


            if (customerViewModel == null)
            {
                return NotFound();
            }

            //sign token here
            customerViewModel.AccessToken = GenerateAccessToken(customer.CustomerId);

            return customerViewModel;
        }

        [HttpPost("RefreshToken")]
        public async Task<ActionResult<CustomerViewModel>> RefreshToken([FromBody] RefreshRequest refreshRequest)
        {
            Customer customer = GetUserFromAccessToken(refreshRequest.AccessToken);

            if (customer != null && ValidateRefresh(customer, refreshRequest.RefreshToken))
            {
                CustomerViewModel customerViewModel = new CustomerViewModel(customer);
                customerViewModel.AccessToken = GenerateAccessToken(customer.CustomerId);


                return customerViewModel;
            }

            return null;
        }
        private bool ValidateRefresh(Customer customer, string refreshToken)
        {

            RefreshTokenCustomer refreshTokenCustomer = _context.RefreshTokenCustomers.Where(rt => rt.Token == refreshToken)
                                        .OrderByDescending(rt => rt.ExpiryDate)
                                        .FirstOrDefault();
            if (refreshTokenCustomer != null && refreshTokenCustomer.CustomerId == customer.CustomerId
                && refreshTokenCustomer.ExpiryDate > DateTime.UtcNow)
            {
                return true;
            }
            return false;
        }

        private Customer GetUserFromAccessToken(string accessToken)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_jwtsettings.SecretKey);

            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = false,
                ValidateAudience = false,
                ClockSkew = TimeSpan.Zero,
                ValidateLifetime = false
            };

            SecurityToken securityToken;
            var principle = tokenHandler.ValidateToken(accessToken, tokenValidationParameters, out securityToken);

            JwtSecurityToken jwtSecurityToken = securityToken as JwtSecurityToken;

            if (jwtSecurityToken != null && jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            {
                var customerId = principle.FindFirst(ClaimTypes.Name)?.Value;

                return _context.Customers.Where(c => c.CustomerId == Convert.ToInt32(customerId)).FirstOrDefault();
            }
            return null;
        }

        private string GenerateAccessToken(int customerId)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_jwtsettings.SecretKey);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name,Convert.ToString(customerId))
                }),
                Expires = DateTime.UtcNow.AddSeconds(20),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        private RefreshTokenCustomer GenerateRefreshToken()
        {
            RefreshTokenCustomer refreshToken = new RefreshTokenCustomer();
            var randomNumber = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
                refreshToken.Token = Convert.ToBase64String(randomNumber);
            }
            refreshToken.ExpiryDate = DateTime.UtcNow.AddMinutes(1);

            return refreshToken;

        }
        private bool CustomerExists(int id)
        {
            return _context.Customers.Any(e => e.CustomerId == id);
        }

        private bool EmailExists(string email)
        {
            return _context.Customers.Any(e => e.Email == email) && _context.Employees.Any(e => e.Email == email);
        }


    }
}
