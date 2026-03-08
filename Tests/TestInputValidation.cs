using NUnit.Framework;
using SafeVault.Security;

namespace Tests
{
    [TestFixture]
    public class TestInputValidation
    {
        [Test]
        public void TestForSQLInjection()
        {
            var dangerous = "admin' OR 1=1; --";
            var sanitized = InputValidator.SanitizeUsername(dangerous);
            Assert.IsNotNull(sanitized);
            Assert.That(sanitized, Does.Not.Contain("'"));
            Assert.That(sanitized, Does.Not.Contain(";"));
            Assert.That(sanitized, Is.EqualTo("adminOR11"));
        }

        [Test]
        public void TestForXSS()
        {
            var raw = "<script>alert('xss')</script>";
            var encoded = InputValidator.HtmlEncode(raw);
            Assert.IsNotNull(encoded);
            Assert.That(encoded, Does.Not.Contain("<script>"));
            Assert.That(encoded, Does.Contain("&lt;script&gt;"));
        }

        [Test]
        public void TestSanitizeEmailValid()
        {
            var email = "user@example.com";
            var sanitized = InputValidator.SanitizeEmail(email);
            Assert.AreEqual(email, sanitized);
        }

        [Test]
        public void TestSanitizeEmailInvalid()
        {
            var email = "not-an-email";
            var sanitized = InputValidator.SanitizeEmail(email);
            Assert.IsNull(sanitized);
        }
    }
}
