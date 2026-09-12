using NUnit.Framework;

namespace ForgeBench.Tests
{
    public sealed class CrashTelemetryTests
    {
        [Test]
        public void Sanitizer_RemovesCredentialLikeLines()
        {
            string input="ordinary line\nAuthorization: Bearer secret-value\nnext line\ntoken=abc123";
            string clean=CrashTelemetryLogger.Sanitize(input,1000);
            Assert.IsTrue(clean.Contains("ordinary line"));
            Assert.IsTrue(clean.Contains("next line"));
            Assert.IsFalse(clean.ToLowerInvariant().Contains("bearer secret"));
            Assert.IsFalse(clean.ToLowerInvariant().Contains("token=abc"));
        }

        [Test]
        public void Sanitizer_EnforcesBoundedLength()
        {
            string clean=CrashTelemetryLogger.Sanitize(new string('x',9000),512);
            Assert.LessOrEqual(clean.Length,512);
        }
    }
}
