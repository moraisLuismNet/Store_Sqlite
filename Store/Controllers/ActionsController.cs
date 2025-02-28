using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Store.Models;
using Microsoft.AspNetCore.Authorization;

namespace Store.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ActionsController : Controller
    {
        private readonly StoreContext _context;

        public ActionsController(StoreContext context)
        {
            _context = context;
        }

        [Authorize]
        [HttpGet]
        public async Task<ActionResult> GetActions()
        {
            var actions = await _context.Actions.ToListAsync();
            return Ok(actions);
        }
    }
}
