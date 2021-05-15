using BuildingMaterialShop.ApiModels.CustomerViewModels;
using BuildingMaterialShop.Auth;
using BuildingMaterialShop.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
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
    [Authorize]
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

        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Customer>>> GetCustomers()
        {
            return await _context.Customers.ToListAsync();
        }

        [AllowAnonymous]
        [HttpGet("{customerId}")]
        public async Task<ActionResult<Customer>> GetCustomerInfo(int customerId)
        {
            var user = await _context.Customers.FindAsync(customerId);

            if (user == null)
            {
                return NotFound();
            }

            user.PassWord = null;

            return user;
        }
        [AllowAnonymous]
        [HttpPost("ChangePassword")]
        public async Task<IActionResult> ChangePassword(CustomerChangePasswordViewModel model)
        {
            if (!CustomerExists(model.CustomerId))
            {
                return BadRequest();
            }
            var customer = _context.Customers.FirstOrDefault(e => e.CustomerId == model.CustomerId && e.PassWord == Auth.MD5.CreateMD5(model.OldPassword));

            if (customer == null)
            {
                return Ok(new { error_message = "Mật khẩu cũ không hợp lệ." });
            }


            if (string.IsNullOrEmpty(model.NewPassword) || model.NewPassword.Contains(' ') || model.NewPassword.Length < 6)
            {
                return Ok("Mật khẩu mới không hợp lệ.");
            }

            if (!model.NewPassword.Equals(model.ConfirmPassword))
            {
                return Ok("Mật khẩu mới không trùng khớp.");
            }

            if (model.NewPassword.Equals(model.OldPassword))
            {
                return Ok("Mật khẩu mới không được giống với mật khẩu cũ.");
            }

            customer.PassWord = Auth.MD5.CreateMD5(model.NewPassword);

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

        [AllowAnonymous]
        [HttpPost("Register")]
        public async Task<ActionResult<CustomerViewModel>> Register([FromBody] CustomerRegisterViewModel customerRegister)
        {
            if (customerRegister.Email == null || customerRegister.Email.Length < 10)
            {
                return Ok("Email không hợp lệ.");
            }

            if (EmailCustomerExists(customerRegister.Email) || EmailEmployeeExists(customerRegister.Email))
            {
                return Ok("Email đã tồn tại.");
            }
            if (customerRegister.PassWord == null || customerRegister.PassWord.Length < 6)
            {
                return Ok("Mật khẩu không hợp lệ.");
            }

            var customer = customerRegister.ToCustomer();

            _context.Customers.Add(customer);

            await _context.SaveChangesAsync();

            customer.PassWord = null;

            return CreatedAtAction("GetCustomerInfo", new { customerId = customer.CustomerId }, customer);

        }
        [AllowAnonymous]
        [HttpPost("Login")]
        public async Task<ActionResult<CustomerViewModel>> Login([FromBody] CustomerLoginViewModel customerLoginViewModel)
        {
            var customer = await _context.Customers
                                .Where(u => u.Email == customerLoginViewModel.Email
                                && u.PassWord == customerLoginViewModel.PassWord)
                                .FirstOrDefaultAsync();

            CustomerViewModel customerViewModel = null;
            if (customer == null)
            {
                return Ok("Email hoặc mật khẩu không chính xác.");
            }
            if (customer.IsBlocked)
            {
                return Ok("Tài khoản đang tạm khóa.");
            }
            if (customer != null)
            {
                RefreshTokenCustomer refreshToken = GenerateRefreshToken();
                customer.RefreshTokenCustomers.Add(refreshToken);
                await _context.SaveChangesAsync();

                customerViewModel = new CustomerViewModel(customer);
                customerViewModel.RefreshToken = refreshToken.Token;
            }


            

            //sign token here
            customerViewModel.AccessToken = GenerateAccessToken(customer.CustomerId);

            return customerViewModel;
        }
        [AllowAnonymous]
        [HttpPost("ChangeInfo/{customerId}")]
        public async Task<IActionResult> PutCustomer(int customerId, [FromBody] Customer customer)
        {

            if (customerId != customer.CustomerId)
            {
                return BadRequest();
            }
            if (!CustomerExists(customerId))
            {
                return NotFound();
            }

            var _customer = await _context.Customers.FindAsync(customerId);
            _customer.Address = customer.Address;
            _customer.BirthDay = customer.BirthDay;
            _customer.FullName = customer.FullName;
            _customer.Gender = customer.Gender;
            _customer.PhoneNumber = customer.PhoneNumber;



            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CustomerExists(customerId))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }
        [AllowAnonymous]
        [HttpPost("Block")]
        public ActionResult Block([FromBody] int customerId)
        {
            if (!CustomerExists(customerId))
                return NotFound();

            var customer = _context.Customers.Find(customerId);
            if (!customer.IsBlocked)
            {
                customer.IsBlocked = true;
            }
            else
            {
                return Ok("Khách hàng đã bị khóa rồi.");
            }
            try
            {
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                return Ok("Khóa khách hàng không thành công.");

            }

            return Ok("Khóa khách hàng thành công.");

        }
        [AllowAnonymous]
        [HttpPost("UnBlock")]
        public async Task<ActionResult> UnBlock([FromBody] int customerId)
        {
            if (!CustomerExists(customerId))
                return NotFound();

            var customer = _context.Customers.Find(customerId);

            if (customer.IsBlocked)
            {
                customer.IsBlocked = false;
            }
            else
            {
                return Ok("Khách hàng chưa bị khóa");
            }

            try
            {
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                return Ok("Mở khóa khách hàng không thành công.");

            }

            return Ok("Mở khóa khách hàng thành công.");

        }
        [AllowAnonymous]
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
                Expires = DateTime.UtcNow.AddHours(2),
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
            refreshToken.ExpiryDate = DateTime.UtcNow.AddDays(1);

            return refreshToken;

        }
        private bool CustomerExists(int id)
        {
            return _context.Customers.Any(e => e.CustomerId == id);
        }

        private bool EmailCustomerExists(string email)
        {
            return _context.Customers.Any(e => e.Email == email);
        }
        private bool EmailEmployeeExists(string email)
        {
            return _context.Employees.Any(e => e.Email == email);
        }
        private string GetCustomerPassword(int id)
        {
            return _context.Customers.Find(id).PassWord;
        }


    }
}
