using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Datadog.Trace;
using Datadog.Trace.Configuration;
using Newtonsoft.Json;

namespace HellDDAgentApi
{
    public static class PokeApiDatadogTracer
    {
        private const string NO_METADATA_ERROR_MESSAGE =
            "Could not retrieve metadata endpoint. Datadog APM and Tracing is disabled";

        private const string NO_IPADDRESS_ERROR_MESSAGE =
            "Could not retrieve Host IP Address from metadata. Datadog APM and Tracing for AWS is disabled";
        
        public static async Task<Tracer> CreateTracer()
        {
            var tracerSettings = TracerSettings.FromDefaultSources();
            OutputDebugInfo(tracerSettings);

            if (tracerSettings.Environment != "Development")
            {
                await LoadTracerSettingsFromAws(tracerSettings);
            }
            var tracer = new Tracer(tracerSettings);
            
            tracer.StartActive("Setup", serviceName: "YOLO");

            return tracer;
        }

        private static async Task LoadTracerSettingsFromAws(TracerSettings tracerSettings)
        {
            
            // Get the metadata endpoint
            var metadataEndpoint = Environment.GetEnvironmentVariable("ECS_CONTAINER_METADATA_URI_V4");
            if(string.IsNullOrEmpty(metadataEndpoint))
            {
                Console.WriteLine("WARNING: " +NO_METADATA_ERROR_MESSAGE);
                return;
            } 
                
            // Process metadata json
            Console.WriteLine("METADATA_ENDPOINT = " + metadataEndpoint);
            var response = await new HttpClient().GetStringAsync($"{metadataEndpoint}");
            if (string.IsNullOrEmpty(response))
            {
                Console.WriteLine("WARNING: " +NO_METADATA_ERROR_MESSAGE);
                return;
            }
            
            var metadata = JsonConvert.DeserializeObject<EcsContainerMetadata>(response);
            Console.WriteLine(metadata?.ToString() ?? "No metadata");
            
            // Get the private IPv4 address
            // Note: This only handles a single network and ip address configuration, we should improve
            // this in the future but should be fine for now.
            var hostIpAddress = metadata?.Networks.FirstOrDefault()?.IPv4Addresses.FirstOrDefault() ?? String.Empty;
            if (string.IsNullOrEmpty(hostIpAddress))
            {
                Console.WriteLine("WARNING: " +NO_IPADDRESS_ERROR_MESSAGE);
                return;
            }

            tracerSettings.Environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            tracerSettings.AgentUri = new Uri($"http://{hostIpAddress}:8126/");

            OutputDebugInfo(tracerSettings);
        }

        private static void OutputDebugInfo(TracerSettings tracerSettings)
        {
            Console.WriteLine("WARNING: " +nameof(tracerSettings.ServiceName)+ ": " + tracerSettings.ServiceName);
            Console.WriteLine("WARNING: " +nameof(tracerSettings.Environment)+ ": " + tracerSettings.Environment);
            Console.WriteLine("WARNING: " +nameof(tracerSettings.AgentUri)+ ": " + tracerSettings.AgentUri.ToString());
            Console.WriteLine("WARNING: " +nameof(tracerSettings.AnalyticsEnabled)+ ": " + tracerSettings.AnalyticsEnabled.ToString());
            Console.WriteLine("WARNING: " +nameof(tracerSettings.TraceEnabled)+ ": " + tracerSettings.TraceEnabled.ToString());
        }

        public class EcsContainerMetadata
        {
            public List<Network> Networks { get; set; }

            public override string ToString()
            {
                var sb = new StringBuilder();

                if (Networks != null)
                {
                    sb.AppendLine("Networks:");
                    foreach (var network in Networks)
                    {
                        sb.AppendLine(network.ToString()+",");
                    }
                }
                
                return sb.ToString();
            }
        }

        public class Network
        {
            public List<string> IPv4Addresses { get; set; }

            public override string ToString()
            {
                var sb = new StringBuilder();

                if (IPv4Addresses != null)
                {
                    sb.AppendLine("IPv4Addresses:");
                    foreach (var ipAddress in IPv4Addresses)
                    {
                        sb.AppendLine(ipAddress + ",");
                    }
                }

                return sb.ToString();
            }
        }
    }
}