using ApiClient.Example.Services;
using ApiClient.Factories;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Net.Http.Json;
using System.Text.Json;

var endpoint = "https://google.com";
//var endpoint = "https://jsonplaceholder.typicode.com/comments";

var client = new HttpClient();
var messageInvoker = new HttpMessageInvoker(new HttpClientHandler(), true);
//using var request = new HttpRequestMessage(HttpMethod.Get, endpoint);

for (int i = 0; i < 100; i++)
{
    using var request = new HttpRequestMessage(HttpMethod.Get, endpoint);
    client.Timeout = TimeSpan.FromSeconds(.1);
    using var response = await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, CancellationToken.None);
    Console.WriteLine(response.IsSuccessStatusCode);
}

for (int i = 0; i < 100; i++)
{
    using var request = new HttpRequestMessage(HttpMethod.Get, endpoint);
    using var response2 = await messageInvoker.SendAsync(request, CancellationToken.None);
    Console.WriteLine(response2.IsSuccessStatusCode);
    //Console.WriteLine((await response2.Content.ReadFromJsonAsync<IEnumerable<Payload>>()));
}



//Console.ReadKey();


//var host = Host
//    .CreateDefaultBuilder()
//    .ConfigureServices((context,services) => {
//        RegisterRestClients(context,services);
//    }).Build();

//using var scope = host.Services.CreateAsyncScope();

//var service = scope.ServiceProvider.GetRequiredService<TestService>();




//void RegisterRestClients(HostBuilderContext context,IServiceCollection services)
//{
//    //var serializerOptions = new JsonSerializerOptions(JsonSerializerDefaults.Web);
//    //var httpClient = new ApiClient.Http.HttpClient();


//    services.AddSingleton<ApiClient.Http.HttpClient>();
//    services.AddSingleton<DefaultHttpRequestMessageFactory>();
//    services.AddScoped<TestService>();
//}

record Payload
{
    public int UserId { get; set; }
    public int Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public string Body { get; set; }
}