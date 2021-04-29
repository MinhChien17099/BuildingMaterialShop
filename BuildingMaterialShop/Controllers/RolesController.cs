using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BuildingMaterialShop.Models;

namespace BuildingMaterialShop.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class RolesController : ControllerBase
    {
        private readonly BuildingMaterialsShopContext _context;

        public RolesController(BuildingMaterialsShopContext context)
        {
            _context = context;
        }

        // GET: api/Roles
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Role>>> GetRoles()
        {
            return await _context.Roles.Where(role => role.RoleId != "ADMIN").ToListAsync();
        }

        // GET: api/Roles/5
        [HttpGet("{roleId}")]
        public async Task<ActionResult<Role>> GetRole(string id)
        {
            var role = await _context.Roles.FindAsync(id);

            if (role == null)
            {
                return NotFound();
            }

            return role;
        }

        private bool RoleExists(string id)
        {
            return _context.Roles.Any(e => e.RoleId == id);
        }
    }
}
