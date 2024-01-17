using Amazon.Lambda.Core;
using OpenTelemetry;
using OpenTelemetry.Trace;
using OpenTelemetry.Instrumentation.AWSLambda;
using Dynatrace.OpenTelemetry;
using Dynatrace.OpenTelemetry.Instrumentation.AwsLambda;
using Amazon.Lambda.APIGatewayEvents;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace myfunction;

public class Function
{        
        public static TracerProvider TracerProvider;

            static Function()
           {
                DynatraceSetup.InitializeLogging();
                TracerProvider = Sdk.CreateTracerProviderBuilder()
                .AddDynatrace()
                .AddAWSLambdaConfigurations(c => c.DisableAwsXRayContextExtraction = true)
                .AddDynatraceAwsSdkInjection()
                .Build();
            }
        
        public APIGatewayHttpApiV2ProxyResponse FunctionHandler(APIGatewayHttpApiV2ProxyRequest request, ILambdaContext context)
        {
            return AWSLambdaWrapper.Trace(TracerProvider, FunctionHandlerInternal, request, context);
        }
        
        private APIGatewayHttpApiV2ProxyResponse  FunctionHandlerInternal(APIGatewayHttpApiV2ProxyRequest request, ILambdaContext context)
        {
            return new APIGatewayHttpApiV2ProxyResponse
            {
                StatusCode = 200,
                Body = "Example function result",
            };
        }
}
