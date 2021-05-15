using BuildingMaterialShop.ApiModels.CustomerViewModels;
using BuildingMaterialShop.Models;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace BuildingMaterialShop.Controllers
{
    [Route("[controller]")]
    [ApiController]

    class AuthController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly BuildingMaterialsShopContext _context;

        public AuthController(IConfiguration config, BuildingMaterialsShopContext context)
        {
            _config = config;
            _context = context;
        }
        // GET: api/<AuthController>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<AuthController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<AuthController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<AuthController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<AuthController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }

        [HttpPost]
        [Route("Login")]
        public IActionResult Login([FromBody] CustomerLoginViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return Unauthorized();
            }
            else
            {
                Customer existUser = CustomerExists(viewModel.Email);

                bool validPassword = CheckPassword(existUser, viewModel.PassWord);

                if (existUser != null && validPassword)
                {

                    var authClaims = new List<Claim>
                    {
                        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    };


                    var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JWT:Secret"]));

                    var token = new JwtSecurityToken(
                        issuer: _config["JWT:ValidIssuer"],
                        audience: _config["JWT:ValidAudience"],
                        expires: DateTime.Now.AddHours(1.5),
                        claims: authClaims,
                        signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
                        );

                    return Ok(new
                    {
                        token = new JwtSecurityTokenHandler().WriteToken(token),
                        expiration = token.ValidTo,
                    });
                }
                else
                {
                    return Unauthorized();
                }
            }
        }

        private Customer CustomerExists(string email)
        {
            return _context.Customers.FirstOrDefault(c => c.Email == email);
        }

        private bool CheckPassword(Customer customer, string password)
        {
            return customer.PassWord == password;
        }
    }
}
