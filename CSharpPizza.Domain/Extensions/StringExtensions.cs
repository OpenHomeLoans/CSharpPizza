namespace CSharpPizza.Domain.Extensions;

public static class StringExtensions
{
    public static async Task<string> FetchDataAsync(this string url)
    {
        using var client = new HttpClient();
        return await client.GetStringAsync(url);
    }
}