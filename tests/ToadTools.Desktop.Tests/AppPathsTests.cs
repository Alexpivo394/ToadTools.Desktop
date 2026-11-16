using Shouldly;
using ToadTools.Desktop.Core;

namespace ToadTools.Desktop.Tests;

public class AppPathsTests
{
    [Test]
    public void AppPaths_BuildsFoldersUnderRoots()
    {
        var paths = new AppPaths(@"C:\L", @"C:\R", @"C:\A");

        paths.AppDir.ShouldBe(@"C:\L\app");
        paths.StagingDir.ShouldBe(@"C:\L\staging");
        paths.DownloadsDir.ShouldBe(@"C:\L\downloads");
        paths.LogsDir.ShouldBe(@"C:\L\logs");
        paths.StateFile.ShouldBe(@"C:\L\state.json");
        paths.SettingsFile.ShouldBe(@"C:\R\settings.json");
        paths.AddinsRoot.ShouldBe(@"C:\A");
        paths.AddinsYear(2025).ShouldBe(@"C:\A\2025");
    }

    [Test]
    [NotInParallel]
    public void Log_WritesDailyFile_DeletesOldOnes()
    {
        using var temp = new TempFolder();
        var old = temp.Combine("ToadTools-2000-01-01.log");
        File.WriteAllText(old, "old");
        File.SetLastWriteTimeUtc(old, DateTime.UtcNow.AddDays(-30));

        Log.Configure(temp.Path);
        Log.Info("hello");
        Log.Error("bad", new InvalidOperationException("boom"));

        File.Exists(old).ShouldBeFalse();
        var today = temp.Combine($"ToadTools-{DateTime.Now:yyyy-MM-dd}.log");
        using var reader = new StreamReader(new FileStream(today, FileMode.Open, FileAccess.Read, FileShare.ReadWrite));
        var text = reader.ReadToEnd();
        text.ShouldContain("hello");
        text.ShouldContain("boom");
    }
}
