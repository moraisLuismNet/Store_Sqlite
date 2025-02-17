namespace Store.Middlewares
{
    public class RegisterAndControlMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IWebHostEnvironment _env;

        public RegisterAndControlMiddleware(RequestDelegate next, IWebHostEnvironment env)
        {
            _next = next;
            _env = env;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            var IP = httpContext.Connection.RemoteIpAddress?.ToString();
            var route = httpContext.Request.Path.ToString();
            var method = httpContext.Request.Method;
            var dateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

            var path = Path.Combine(_env.ContentRootPath, "wwwroot", "log.txt");

            using (StreamWriter writer = new StreamWriter(path, append: true))
            {
                writer.WriteLine($"{dateTime} | IP: {IP} | Ruta: {route} | Método: {method}");
            }

            await _next(httpContext);
        }
    }
}

