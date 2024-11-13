using Spydersoft.Platform.Exceptions;

namespace Spydersoft.Platform.UnitTests.ExceptionTests
{
    public class Tests
    {
        [Test]
        public void Validate_Constructor()
        {
            var message = "My Configuration Message;";
            var exception = new ConfigurationException(message);
            Assert.That(exception.Message, Is.EqualTo(message));
        }
    }
}
