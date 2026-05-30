namespace CoachingMVC.Middleware
{
    public class SessionAuthMiddleware
    {
        private readonly RequestDelegate _next;

        private static readonly string[] PublicPrefixes =
        {
            "/account", "/home", "/"
        };

        public SessionAuthMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var path = context.Request.Path.Value?.ToLower() ?? "/";

            // Allow public routes through without auth check
            bool isPublic = path == "/"
                || path.StartsWith("/account")
                || path.StartsWith("/home")
                || path == "/admission/apply"
                || path == "/admission/success";

            if (!isPublic)
            {
                var token = context.Session.GetString("Token");
                var role = context.Session.GetString("Role");

                // Not logged in at all
                if (string.IsNullOrEmpty(token))
                {
                    context.Response.Redirect($"/Account/Login?returnUrl={Uri.EscapeDataString(context.Request.Path)}");
                    return;
                }

                // Admin-only paths
                if (path.StartsWith("/admin") && role != "Admin")
                {
                    context.Response.Redirect("/Account/Login");
                    return;
                }

                // Teacher paths
                if (path.StartsWith("/teacher") && role != "Teacher" && role != "Admin")
                {
                    context.Response.Redirect("/Account/Login");
                    return;
                }

                // Student paths
                if (path.StartsWith("/student") && role != "Student")
                {
                    context.Response.Redirect("/Account/Login");
                    return;
                }
            }

            await _next(context);
        }
    }
}