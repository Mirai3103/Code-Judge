using NUnit.Framework;

namespace Code_Judge.Application.IntegrationTests;

using static Testing;

[TestFixture]
public abstract class BaseTestFixture
{
    // [SetUp]
    // public async Task TestSetUp()
    // {
    //     // await ResetState();
    // }
    // after all tests
    [OneTimeTearDown]
    public async Task RunAfterAnyTests()
    {
        await ResetState();
    }
  
}
