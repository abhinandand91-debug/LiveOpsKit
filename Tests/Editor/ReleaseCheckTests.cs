using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using LiveOpsKit.Editor;
using NUnit.Framework;

namespace LiveOpsKit.Tests
{
    public class ReleaseCheckTests
    {
        [TestCase("1.2.1", "1.2.0", Severity.Pass)]
        [TestCase("1.2.0", "1.2.0", Severity.Error)]
        [TestCase("1.1.9", "1.2.0", Severity.Error)]
        [TestCase("1.2.0", "", Severity.Warning)]
        [TestCase("beta", "1.0", Severity.Warning)]
        public void VersionName_IsCompared(string current, string released, Severity expected)
        {
            Assert.AreEqual(expected, VersionCheck.CheckVersionName(current, released).Severity);
        }

        [Test]
        public void ForbiddenText_ReportsFirstMatchingLine()
        {
            string file = Path.GetTempFileName();
            File.WriteAllLines(file, new[] { "{", "\"url\": \"https://staging.mygame.com\"", "}" });

            var findings = ForbiddenTextCheck.Scan(file, new[] { new Regex(@"staging\.") }).ToList();
            File.Delete(file);

            Assert.AreEqual(1, findings.Count);
            StringAssert.Contains("line 2", findings[0]);
        }

        [Test]
        public void ForbiddenText_CleanFileHasNoFindings()
        {
            string file = Path.GetTempFileName();
            File.WriteAllText(file, "{ \"url\": \"https://api.mygame.com\" }");

            var findings = ForbiddenTextCheck.Scan(file, new[] { new Regex("localhost") }).ToList();
            File.Delete(file);

            Assert.IsEmpty(findings);
        }

        [Test]
        public void HasErrors_OnlyCountsErrors()
        {
            Assert.IsFalse(ReleaseCheckRunner.HasErrors(new[] { CheckResult.Pass("ok"), CheckResult.Warning("hm") }));
            Assert.IsTrue(ReleaseCheckRunner.HasErrors(new[] { CheckResult.Pass("ok"), CheckResult.Error("bad") }));
        }
    }
}
