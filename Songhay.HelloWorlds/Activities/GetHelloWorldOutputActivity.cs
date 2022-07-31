namespace Songhay.HelloWorlds.Activities;

public class GetHelloWorldOutputActivity : GetHelloWorldActivity, IActivityWithTask<string, string>
{
    public async Task<string?> StartAsync(string? input)
    {
        var message = GetHelloWorldMessage(input);
        TraceSource?.WriteLine(message);

        return await Task.FromResult(message);
    }
}
