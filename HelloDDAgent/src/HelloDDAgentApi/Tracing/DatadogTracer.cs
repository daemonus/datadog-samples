using System;
using Datadog.Trace;
using Microsoft.Extensions.Logging;

namespace HelloDDAgentApi.Tracing
{
    public interface IMicroserviceTracer 
        {
            /// <summary>
            /// Adds an exception to the current active trace span (if one exists).
            /// </summary>
            void SetSpanException(Exception ex);

            /// <summary>
            /// Adds a tag (string) to the current active trace span (if one exists).
            /// </summary>
            void SetSpanTag(string key, string value);
        }
    
        public class DatadogTracer : IMicroserviceTracer
        {
            private readonly ILogger<DatadogTracer> _logger;

            public DatadogTracer(ILogger<DatadogTracer> logger)
            {
                _logger = logger;
            }

            public void SetSpanException(Exception ex)
            {
                Tracer.Instance.ActiveScope.Span.SetException(ex);
            }

            public void SetSpanTag(string key, string value)
            {
                Tracer.Instance.ActiveScope.Span.SetTag(key, value);
            }
        }
    }