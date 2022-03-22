using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace branch_hero
{
    public class GitHubWebhookAuthenticationMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly string _webhookSignature;

        public GitHubWebhookAuthenticationMiddleware(RequestDelegate next, IOptions<GitHubSettings> githubSettings)
        {
            _next = next;
            _webhookSignature = Sha256_hash(githubSettings.Value.WebhookSecret);
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (context.Request.Headers.TryGetValue("X-Hub-Signature-256", out var incomingSignature))
            {
                // https://vcsjones.dev/fixed-time-equals-dotnet-core/
                if (CryptographicOperations.FixedTimeEquals(Encoding.ASCII.GetBytes(incomingSignature), Encoding.ASCII.GetBytes("sha256=" + _webhookSignature)))
                {
                    await _next(context);
                }
            }
        }

        // https://stackoverflow.com/a/17001289/15507446
        private static string Sha256_hash(string value)
        {
            var Sb = new StringBuilder();

            using (var hash = SHA256.Create())
            {
                Encoding enc = Encoding.UTF8;
                byte[] result = hash.ComputeHash(enc.GetBytes(value));

                foreach (byte b in result)
                    Sb.Append(b.ToString("x2"));
            }

            return Sb.ToString();
        }
    }
}
