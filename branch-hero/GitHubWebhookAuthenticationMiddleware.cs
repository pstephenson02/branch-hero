using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace branch_hero
{
    public class GitHubWebhookAuthenticationMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly string _webhookSecret;
        private const string _sha256Prefix = "sha256=";

        public GitHubWebhookAuthenticationMiddleware(RequestDelegate next, IOptions<GitHubSettings> githubSettings)
        {
            _next = next;
            _webhookSecret = githubSettings.Value.WebhookSecret;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (context.Request.Headers.TryGetValue("X-Hub-Signature-256", out var incomingSignature))
            {
                // https://devblogs.microsoft.com/dotnet/re-reading-asp-net-core-request-bodies-with-enablebuffering/
                context.Request.EnableBuffering();

                // Leave the body open so the next middleware can read it.
                using var reader = new StreamReader(
                    context.Request.Body,
                    encoding: Encoding.UTF8,
                    detectEncodingFromByteOrderMarks: false,
                    leaveOpen: true);

                var body = await reader.ReadToEndAsync();

                // https://stackoverflow.com/a/12253723/15507446
                var hmacSha256 = new HMACSHA256(Encoding.UTF8.GetBytes(_webhookSecret));
                var hash = hmacSha256.ComputeHash(Encoding.UTF8.GetBytes(body));
                var hashAsString = Convert.ToHexString(hash).ToLower();

                var headerWithoutSha256Prefix = incomingSignature.ToString()[_sha256Prefix.Length..];
                // https://vcsjones.dev/fixed-time-equals-dotnet-core/
                if (CryptographicOperations.FixedTimeEquals(Encoding.UTF8.GetBytes(headerWithoutSha256Prefix), Encoding.UTF8.GetBytes(hashAsString)))
                {
                    // Reset the request body stream position so the next middleware can read it
                    context.Request.Body.Position = 0;
                    await _next(context);
                }
            }
        }
    }
}
