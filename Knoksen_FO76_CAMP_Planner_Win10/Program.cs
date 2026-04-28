namespace FO76CampPlanner;

using Microsoft.Extensions.DependencyInjection;

internal static class Program
{
    [STAThread]
    static void Main()
    {
        try
        {
            ApplicationConfiguration.Initialize();

            var services = new ServiceCollection();
            services.AddSingleton<IProjectService, ProjectService>();
            services.AddSingleton<IBlueprintService, BlueprintService>();
            services.AddSingleton<MainForm>();

            using var provider = services.BuildServiceProvider();
            var mainForm = provider.GetRequiredService<MainForm>();
            Application.Run(mainForm);
        }
        catch (Exception ex)
        {
            var errorMessage = $"""
                {DateTime.Now:yyyy-MM-dd HH:mm:ss} - Fatal Error
                Type: {ex.GetType().Name}
                Message: {ex.Message}
                StackTrace:
                {ex.StackTrace}

                Inner Exception:
                {(ex.InnerException?.ToString() ?? "None")}
                {new string('=', 80)}
                """;

            AppDiagnostics.TryAppend(errorMessage);
            var logPath = AppDiagnostics.GetLogPath();

            try
            {
                MessageBox.Show(
                    $"Fatal Error:\n\n{ex.GetType().Name}\n\n{ex.Message}\n\nDetails written to:\n{logPath}",
                    "FO76 CAMP Planner - Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
            catch { }
        }
    }
}
