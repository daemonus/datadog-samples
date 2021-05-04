using System;
using System.Threading.Tasks;
using HelloDDAgentApi.Tracing;
using Microsoft.AspNetCore.Http;

namespace HellDDAgentApi.Middleware
{
    public class DatadogTracingMiddleware
    {
        private readonly RequestDelegate _next;

        public DatadogTracingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext httpContext, IMicroserviceTracer tracer)
        {
            tracer.SetSpanTag("UserName", "microserviceContext.GetUsername()");
            tracer.SetSpanTag("OrgId", Guid.NewGuid().ToString());
            
            await _next(httpContext);
        }
    }
}