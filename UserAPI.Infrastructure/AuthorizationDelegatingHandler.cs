using System.Net.Http.Headers;
using Microsoft.AspNetCore.Http;

namespace UserAPI.Infrastructure
{
    public class AuthorizationDelegatingHandler : DelegatingHandler
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public AuthorizationDelegatingHandler(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    protected override Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        var authorization = _httpContextAccessor
            .HttpContext?
            .Request
            .Headers["Authorization"]
            .ToString();

        if (!string.IsNullOrWhiteSpace(authorization))
        {
            request.Headers.Authorization =
                AuthenticationHeaderValue.Parse(authorization);
        }

        return base.SendAsync(request, cancellationToken);
    }
}
}