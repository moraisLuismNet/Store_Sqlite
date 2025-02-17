using Store.Models;

namespace Store.Services
{
    public class ActionsService
    {
        private readonly StoreContext _context;
        private readonly IHttpContextAccessor _accessor;

        public ActionsService(StoreContext context, IHttpContextAccessor accessor)
        {
            _context = context;
            _accessor = accessor;
        }

        public async Task AddAction(string actionName, string controller)
        {
            Store.Models.Action newAction = new()
            {
                ActionDate = DateTime.Now,
                ActionName = actionName,
                Controller = controller,
                Ip = _accessor.HttpContext.Connection.RemoteIpAddress.ToString()
            };

            await _context.Actions.AddAsync(newAction);
            await _context.SaveChangesAsync();

            Task.FromResult(0);
        }
    }
}

