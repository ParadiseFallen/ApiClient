using ApiClient.Example.Services;
using ApiClient.Factories;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Net.Http.Json;
using System.Text.Json;

var endpoint = "https://google.com";

var client = new HttpClient();
var messageInvoker = new HttpMessageInvoker(new HttpClientHandler(), true);
var request = new HttpRequestMessage(HttpMethod.Post, endpoint) { Content = JsonContent.Create<dynamic>(new {  foo = "bar"})};
var response = await client.SendAsync(request, CancellationToken.None);
var response2 = await messageInvoker.SendAsync(request, CancellationToken.None);
Console.WriteLine(response);
Console.WriteLine(response2);
Console.ReadKey();


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