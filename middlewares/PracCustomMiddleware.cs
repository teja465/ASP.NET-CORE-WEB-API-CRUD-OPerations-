using System.Threading.Tasks;
using JWTAuthenticationRestAPI;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Net.Http.Headers;
using JWTAuthenticationRestAPI.middlewares;

namespace JWTAuthenticationRestAPI.middlewares
{
    public class MyMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger _logger;

    public MyMiddleware(RequestDelegate next, ILoggerFactory logFactory, IJWTAuthenticationManager _auth)
    {
        _next = next;

        _logger = logFactory.CreateLogger("MyMiddleware");
    }

    public async Task Invoke(HttpContext httpContext)
    {
        _logger.LogInformation("MyMiddleware executing.. done ");
        // if (!httpContext.User.Identity.IsAuthenticated){
        //         var jwtToken =  httpContext.Request.Headers[HeaderNames.Authorization].ToString();
        //         _logger.LogInformation("JWT token is  "+jwtToken);

        //         await httpContext.Response.WriteAsJsonAsync("middle ware output here");
        // }
        // else{
        //     _logger.LogInformation("User  logged in  ");
        // }
        var jwtToken =  httpContext.Request.Headers[HeaderNames.Authorization].ToString();
        _logger.LogInformation("JWT token is  "+jwtToken);
        if(jwtToken.Trim() ==""){
            await httpContext.Response.WriteAsJsonAsync("JWT token is empty ");
        }

        // csrf check
        var baseURL = httpContext.Request.Host.ToUriComponent().ToString()+httpContext.Request.Path.ToString();
        
        _logger.LogInformation("Base URL is "+baseURL);
        await _next(httpContext); // calling next middleware

    }
}

// Extension method used to add the middleware to the HTTP request pipeline.
public static class MyMiddlewareExtensions
{
    public static IApplicationBuilder UseMyPracCustomMiddleware(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<MyMiddleware>();
    }
} 
}