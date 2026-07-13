using System.IO;
using DotNetEnv;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using VehicleServiceBooking.Notification.Functions.Configuration;
using VehicleServiceBooking.Notification.Functions.Services;
using VehicleServiceBooking.Observability;

static string? FindEnvFile(params string[] startPaths)
{
    foreach (var startPath in startPaths)
    {
        var directory = new DirectoryInfo(Path.GetFullPath(startPath));
        while (directory is not null)
        {
            var candidate = Path.Combine(directory.FullName, ".env");
            if (File.Exists(candidate))
            {
                return candidate;
            }

            directory = directory.Parent;
        }
    }

    return null;
}

var envFilePath = FindEnvFile(Directory.GetCurrentDirectory(), AppContext.BaseDirectory);
if (!string.IsNullOrWhiteSpace(envFilePath))
{
    Env.Load(envFilePath);
}

static string? FirstNonEmpty(params string[] keys)
{
    foreach (var key in keys)
    {
        var value = Environment.GetEnvironmentVariable(key);
        if (!string.IsNullOrWhiteSpace(value))
        {
            return value;
        }
    }

    return null;
}

static void NormalizeSetting(string targetKey, params string[] sourceKeys)
{
    var resolved = FirstNonEmpty(sourceKeys);
    if (!string.IsNullOrWhiteSpace(resolved))
    {
        // Force canonical key so Configuration binding resolves predictably.
        Environment.SetEnvironmentVariable(targetKey, resolved);
    }
}

// Normalize all Notification and EmailSender settings declared across local.settings and .env.
NormalizeSetting("FUNCTIONS_WORKER_RUNTIME", "FUNCTIONS_WORKER_RUNTIME");
NormalizeSetting("Notification__Enabled", "Notification__Enabled", "NOTIFICATION__ENABLED");
NormalizeSetting("NotificationQueueName", "NotificationQueueName", "NOTIFICATION__QUEUENAME");
NormalizeSetting("NotificationPoisonQueueName", "NotificationPoisonQueueName", "NOTIFICATION__POISONQUEUENAME");
NormalizeSetting("AzureWebJobsStorage", "AzureWebJobsStorage", "NOTIFICATION__QUEUECONNECTIONSTRING");
NormalizeSetting("EmailSender__Provider", "EmailSender__Provider", "EMAILSENDER__PROVIDER");
NormalizeSetting("EmailSender__FromAddress", "EmailSender__FromAddress", "EMAILSENDER__FROMADDRESS");
NormalizeSetting("EmailSender__FromName", "EmailSender__FromName", "EMAILSENDER__FROMNAME");
NormalizeSetting("EmailSender__SendGridEndpoint", "EmailSender__SendGridEndpoint", "EMAILSENDER__SENDGRIDENDPOINT");
NormalizeSetting("EmailSender__SendGridApiKey", "EmailSender__SendGridApiKey", "EMAILSENDER__SENDGRIDAPIKEY");
NormalizeSetting("EmailSender__GoogleApiEndpoint", "EmailSender__GoogleApiEndpoint", "EMAILSENDER__GOOGLEAPIENDPOINT");
NormalizeSetting("EmailSender__GoogleTokenEndpoint", "EmailSender__GoogleTokenEndpoint", "EMAILSENDER__GOOGLETOKENENDPOINT");
NormalizeSetting("EmailSender__GoogleUserId", "EmailSender__GoogleUserId", "EMAILSENDER__GOOGLEUSERID");
NormalizeSetting("EmailSender__GoogleClientId", "EmailSender__GoogleClientId", "EMAILSENDER__GOOGLECLIENTID");
NormalizeSetting("EmailSender__GoogleClientSecret", "EmailSender__GoogleClientSecret", "EMAILSENDER__GOOGLECLIENTSECRET");
NormalizeSetting("EmailSender__GoogleRefreshToken", "EmailSender__GoogleRefreshToken", "EMAILSENDER__GOOGLEREFRESHTOKEN");
NormalizeSetting("EmailSender__GoogleAccessToken", "EmailSender__GoogleAccessToken", "EMAILSENDER__GOOGLEACCESSTOKEN");

var resolvedNotificationQueueName = Environment.GetEnvironmentVariable("NotificationQueueName");
if (!string.IsNullOrWhiteSpace(resolvedNotificationQueueName)
    && string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("NotificationPoisonQueueName")))
{
    Environment.SetEnvironmentVariable("NotificationPoisonQueueName", $"{resolvedNotificationQueueName}-poison");
}

var emailProvider = Environment.GetEnvironmentVariable("EmailSender__Provider");
if (string.IsNullOrWhiteSpace(emailProvider))
{
    emailProvider = "Logging";
}

var hasSendGridApiKey = !string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("EmailSender__SendGridApiKey"));
var hasGoogleAccessToken = !string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("EmailSender__GoogleAccessToken"));
var hasGoogleRefreshFlowConfig =
    !string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("EmailSender__GoogleClientId"))
    && !string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("EmailSender__GoogleClientSecret"))
    && !string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("EmailSender__GoogleRefreshToken"));

Console.WriteLine(
$"Notification Functions startup config: Queue={Environment.GetEnvironmentVariable("NotificationQueueName") ?? "<unset>"}, PoisonQueue={Environment.GetEnvironmentVariable("NotificationPoisonQueueName") ?? "<unset>"}, EmailProvider={emailProvider}, SendGridApiKeyConfigured={hasSendGridApiKey}, GoogleAccessTokenConfigured={hasGoogleAccessToken}, GoogleRefreshFlowConfigured={hasGoogleRefreshFlowConfig}");

var host = new HostBuilder()
    .AddLoggingAndTracingForFunctions()
    .ConfigureFunctionsWorkerDefaults()
    .ConfigureServices((context, services) =>
    {
        services.Configure<EmailSenderOptions>(context.Configuration.GetSection("EmailSender"));

        var provider = context.Configuration["EmailSender:Provider"];
        if (string.Equals(provider, "SendGrid", StringComparison.OrdinalIgnoreCase))
        {
            services.AddHttpClient();
            services.AddSingleton<IEmailSender, SendGridEmailSender>();
        }
        else if (string.Equals(provider, "Google", StringComparison.OrdinalIgnoreCase))
        {
            services.AddHttpClient();
            services.AddSingleton<IEmailSender, GoogleEmailSender>();
        }
        else
        {
            services.AddSingleton<IEmailSender, LoggingEmailSender>();
        }
    })
    .Build();

host.Run();
