using Xunit;
using OhAuthToo.ConcreteClients;
using System.Net.Http;
using Moq;
using Microsoft.Extensions.DependencyInjection;

namespace OhAuthToo.Tests.ConcreteClients;

public class VkontakteClientTests
{
    private readonly VkontakteClient _client;
    private readonly Mock<IHttpClientFactory> _mockHttpClientFactory;

    public VkontakteClientTests()
    {
        _mockHttpClientFactory = new Mock<IHttpClientFactory>();
        _client = new VkontakteClient(_mockHttpClientFactory.Object, "test_client_id", "test_client_secret");
    }

    [Fact]
    public void Constructor_WithValidParameters_CreatesInstance()
    {
        // Act
        var client = new VkontakteClient(_mockHttpClientFactory.Object, "client_id", "client_secret");

        // Assert
        Assert.NotNull(client);
    }

    [Fact]
    public void Constructor_WithNullHttpClientFactory_ThrowsArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new VkontakteClient(null, "client_id", "client_secret"));
    }

    [Fact]
    public void Constructor_WithNullClientId_ThrowsArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => 
            new VkontakteClient(_mockHttpClientFactory.Object, null, "client_secret"));
    }

    [Fact]
    public void Constructor_WithNullClientSecret_ThrowsArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => 
            new VkontakteClient(_mockHttpClientFactory.Object, "client_id", null));
    }

    [Fact]
    public void GetAuthorizationUrl_WithValidParameters_ReturnsCorrectUrl()
    {
        // Arrange
        var redirectUri = "https://example.com/callback";
        var state = "test_state";
        var scope = "email,profile";

        // Act
        var url = _client.GetAuthorizationUrl(redirectUri, state, scope);

        // Assert
        Assert.Contains("https://oauth.vk.com/authorize", url);
        Assert.Contains("client_id=test_client_id", url);
        Assert.Contains($"redirect_uri={Uri.EscapeDataString(redirectUri)}", url);
        Assert.Contains($"state={state}", url);
        Assert.Contains($"scope={scope}", url);
        Assert.Contains("response_type=code", url);
    }

    [Fact]
    public void GetAuthorizationUrl_WithNullRedirectUri_ThrowsArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => 
            _client.GetAuthorizationUrl(null, "state", "scope"));
    }

    [Fact]
    public void GetAuthorizationUrl_WithNullState_ThrowsArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => 
            _client.GetAuthorizationUrl("https://example.com", null, "scope"));
    }
} 