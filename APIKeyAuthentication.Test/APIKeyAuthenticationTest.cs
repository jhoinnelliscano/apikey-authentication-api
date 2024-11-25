using APIKeyAuthentication.API.Config;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Moq;

namespace APIKeyAuthentication.Test
{
    public class APIKeyAuthenticationTest
    {
        private readonly string _validApiKey = "d290f1ee-6c54-4b01-90e6-d701748f0851";

        [Fact]
        public async Task InvokeAsync_WithValidApiKey_ProceedToNextMiddleware()
        {

            var configurationMock = new Mock<IConfiguration>();
            var sectionMock = new Mock<IConfigurationSection>();

            // Mock de los hijos de la sección
            var children = new List<IConfigurationSection>
            {
                Mock.Of<IConfigurationSection>(x => x.Value == _validApiKey)
            };

            sectionMock.Setup(x => x.GetChildren()).Returns(children);
            configurationMock.Setup(x => x.GetSection("ApiKeys")).Returns(sectionMock.Object);

            var middleware = new ApiKeyValidationMiddleware((innerHttpContext) =>
            {
                innerHttpContext.Response.StatusCode = 200; // OK
                return Task.CompletedTask;
            }, configurationMock.Object);

            var httpContext = new DefaultHttpContext();
            httpContext.Request.Headers["x-api-key"] = _validApiKey;

            await middleware.InvokeAsync(httpContext);

            Assert.Equal(200, httpContext.Response.StatusCode);
        }

        [Fact]
        public async Task InvokeAsync_WithMissingApiKey_ReturnsUnauthorized()
        {
            var configurationMock = new Mock<IConfiguration>();
            var middleware = new ApiKeyValidationMiddleware((innerHttpContext) =>
            {
                innerHttpContext.Response.StatusCode = 200; // OK
                return Task.CompletedTask;
            }, configurationMock.Object);

            var httpContext = new DefaultHttpContext();

            await middleware.InvokeAsync(httpContext);

            Assert.Equal(401, httpContext.Response.StatusCode); // Unauthorized
        }

        [Fact]
        public async Task InvokeAsync_WithInvalidApiKey_ReturnsForbidden()
        {
            var invalidApiKey = "wrong-api-key-6c54-4b01-90e6-d701748f0851";
            var configurationMock = new Mock<IConfiguration>();
            var sectionMock = new Mock<IConfigurationSection>();

            // Mock de los hijos de la sección
            var children = new List<IConfigurationSection>
            {
                Mock.Of<IConfigurationSection>(x => x.Value == _validApiKey)
            };

            sectionMock.Setup(x => x.GetChildren()).Returns(children);
            configurationMock.Setup(x => x.GetSection("ApiKeys")).Returns(sectionMock.Object);

            var middleware = new ApiKeyValidationMiddleware((innerHttpContext) =>
            {
                innerHttpContext.Response.StatusCode = 200; // OK
                return Task.CompletedTask;
            }, configurationMock.Object);

            var httpContext = new DefaultHttpContext();
            httpContext.Request.Headers["x-api-key"] = invalidApiKey;

            await middleware.InvokeAsync(httpContext);

            Assert.Equal(403, httpContext.Response.StatusCode); // Forbidden
        }
    }
}