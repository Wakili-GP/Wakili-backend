using Xunit;
using Wakiliy.API.IntegrationTests.Infrastructure;

namespace Wakiliy.API.IntegrationTests;

[CollectionDefinition("IntegrationTests")]
public class SharedTestCollection : ICollectionFixture<CustomWebApplicationFactory>
{
    // This class has no code, and is never created. Its purpose is simply
    // to be the place to apply [CollectionDefinition] and all the
    // ICollectionFixture<> interfaces.
}
