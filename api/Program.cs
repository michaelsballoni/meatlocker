var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

var app = builder.Build();
app.UseHttpsRedirection();
app.UseFileServer();
app.UseAuthorization();
app.MapControllers();
var thread = new Thread(HandleCmds);
thread.Start();
app.Run();

void HandleCmds()
{
    while (true)
    {
        var cmd = Console.ReadLine();
        if (cmd == null)
            break;

        if (string.IsNullOrWhiteSpace(cmd))
            continue;

        if (cmd == "quit")
        {
            app.StopAsync();
            break;
        }

        Console.WriteLine("Unknown command: {0}", cmd);
        Console.WriteLine("Commands: quit", cmd);
    }
}