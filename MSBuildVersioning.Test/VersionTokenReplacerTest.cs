using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace MSBuildVersioning.Test
{
    [TestFixture]
    public class VersionTokenReplacerTest
    {
        [Test]
        public void ReplaceTest()
        {
            string content = "It is now: $DATETIME$";

            content = new VersionTokenReplacer().Replace(content);

            DateTime dt = DateTime.Parse(content.Substring("It is now: ".Length));

            long difference = Math.Abs(DateTime.Now.Ticks - dt.Ticks);

            Assert.IsTrue(difference < TimeSpan.TicksPerSecond);
        }

        [Test]
        public void EnvironmentReplacement_WithValidEnvironmentVariable()
        {
            var content = "Username: $ENVIRONMENT(\"USERNAME\",\"UnknownUser\")$";

            content = new VersionTokenReplacer().Replace(content);

            var expected = "Username: " + Environment.UserName;

            Assert.That(content, Is.EqualTo(expected));
        }

        [Test]
        public void EnvironmentReplacement_WithInvalidEnvironmentVariable()
        {
            var content = "Environment: $ENVIRONMENT(\"NO_SUCH_VARIABLE_SHOULD_EXIST\",\"DefaultValue\")$";

            content = new VersionTokenReplacer().Replace(content);

            Assert.That(content, Is.EqualTo("Environment: DefaultValue"));
        }
    }
}
