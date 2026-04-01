using System.Net.Http;
using System.Text;
using Xunit;

namespace digiEasy_backend.Tests;

public class HttpApiTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public HttpApiTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Get_root_returns_ok_and_lists_endpoints()
    {
        var response = await _client.GetAsync("/");
        response.EnsureSuccessStatusCode();
        var body = await response.Content.ReadAsStringAsync();
        Assert.Contains("digiEasy", body, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("swagger", body, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task Swagger_openapi_json_is_available_in_development()
    {
        var response = await _client.GetAsync("/swagger/v1/swagger.json");
        response.EnsureSuccessStatusCode();
        var body = await response.Content.ReadAsStringAsync();
        Assert.Contains("openapi", body, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task Register_returns_ok_for_valid_payload()
    {
        var email = $"reg.{Guid.NewGuid():N}@example.com";
        var json =
            $"{{\"firstName\":\"Jane\",\"lastName\":\"Doe\",\"email\":\"{email}\",\"password\":\"Abcd1234\",\"passwordConfirmation\":\"Abcd1234\",\"gender\":\"F\",\"birthday\":\"1999-06-01\",\"mobilePhoneNumber\":\"87654321\"}}";
        using var content = new StringContent(json, Encoding.UTF8, "application/json");
        var response = await _client.PostAsync("/api/users/register", content);
        response.EnsureSuccessStatusCode();
        var body = await response.Content.ReadAsStringAsync();
        Assert.Contains("Registration successful", body, StringComparison.Ordinal);
    }

    [Fact]
    public async Task Verify_password_reports_valid_for_correct_password()
    {
        var email = $"verify.{Guid.NewGuid():N}@example.com";
        var registerJson =
            $"{{\"firstName\":\"V\",\"lastName\":\"U\",\"email\":\"{email}\",\"password\":\"Abcd1234\",\"passwordConfirmation\":\"Abcd1234\",\"gender\":\"M\",\"birthday\":\"1995-01-01\",\"mobilePhoneNumber\":\"11112222\"}}";
        using (var reg = new StringContent(registerJson, Encoding.UTF8, "application/json"))
        {
            (await _client.PostAsync("/api/users/register", reg)).EnsureSuccessStatusCode();
        }

        var verifyJson = $"{{\"email\":\"{email}\",\"password\":\"Abcd1234\"}}";
        using var verify = new StringContent(verifyJson, Encoding.UTF8, "application/json");
        var response = await _client.PostAsync("/api/users/verify-password", verify);
        response.EnsureSuccessStatusCode();
        var body = await response.Content.ReadAsStringAsync();
        Assert.Contains("\"valid\":true", body, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task Delete_user_removes_user_when_password_matches()
    {
        var email = $"del.{Guid.NewGuid():N}@example.com";
        var registerJson =
            $"{{\"firstName\":\"D\",\"lastName\":\"L\",\"email\":\"{email}\",\"password\":\"Abcd1234\",\"passwordConfirmation\":\"Abcd1234\",\"gender\":\"M\",\"birthday\":\"1994-01-01\",\"mobilePhoneNumber\":\"22334455\"}}";
        using (var reg = new StringContent(registerJson, Encoding.UTF8, "application/json"))
        {
            (await _client.PostAsync("/api/users/register", reg)).EnsureSuccessStatusCode();
        }

        var deleteJson = $"{{\"email\":\"{email}\",\"password\":\"Abcd1234\"}}";
        using var deleteContent = new StringContent(deleteJson, Encoding.UTF8, "application/json");
        var deleteRequest = new HttpRequestMessage(HttpMethod.Delete, "/api/users") { Content = deleteContent };
        var deleteResponse = await _client.SendAsync(deleteRequest);
        deleteResponse.EnsureSuccessStatusCode();
        var deleteBody = await deleteResponse.Content.ReadAsStringAsync();
        Assert.Contains("deleted successfully", deleteBody, StringComparison.OrdinalIgnoreCase);

        var verifyJson = $"{{\"email\":\"{email}\",\"password\":\"Abcd1234\"}}";
        using var verify = new StringContent(verifyJson, Encoding.UTF8, "application/json");
        var verifyResponse = await _client.PostAsync("/api/users/verify-password", verify);
        verifyResponse.EnsureSuccessStatusCode();
        var verifyBody = await verifyResponse.Content.ReadAsStringAsync();
        Assert.Contains("user_not_found", verifyBody, StringComparison.OrdinalIgnoreCase);
    }
}
