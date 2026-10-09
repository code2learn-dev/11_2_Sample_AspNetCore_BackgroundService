using BackgroundServiceSample.WorkerServiceSample.Models;
using BackgroundServiceSample.WorkerServiceSample.RateServices;
using Microsoft.EntityFrameworkCore;

/*
 * 
Program.cs—As in a typical ASP.NET Core application, this contains the entry
point for your application, and it’s where the IHost is built and run. By contrast
with a typical .NET 7 ASP.NET Core app, it uses the generic host instead of the
minimal hosting WebApplication and WebApplicationBuilder.


The most notable difference between the worker service template and an ASP.NET
Core template is that Program.cs doesn’t use the WebApplicationBuilder and WebApplication
APIs for minimal hosting. Instead, it uses the Host.CreateDefaultBuilder()

This method takes a lambda method, which
takes two arguments:
	1 A HostBuilderContext object. This context object exposes the IConfiguration for
	your app as the property Configuration, and the IHostEnvironment as the property
	HostingEnvironment.
	
	2 An ISeviceCollection object. You add your services to this collection in the
	same way you add them to WebApplicationBuilder.Services in typical ASP.NET
	Core apps.
 */

/*
 How to call worker service as windows service
	1. install package Microsoft.Extensions.Hosting.WindowsServices
	2. call UseWindowsService() extension method on your IHostBuilder
	3. publish the application with enter the following command in CLI
			dotnet publish -c Release
	4. Open a command prompt as Administrator and install the application using
	   the Windows sc utility. You need to provide the path to your published project’s
	   .exe file and a name to use for the service, such as My Test Service:
			
			sc create "My Test Service" BinPath="C:\path\to\MyService.exe"
 */
IHost host = Host.CreateDefaultBuilder(args)
	.ConfigureServices(async (httpContext, services) =>
	{
		// to add http client service we must add this package :
		// Microsoft.Extensions.Http
		var rateClient = services.AddHttpClient<RateHttpClientService>(client =>
		{
			client.BaseAddress = new Uri("https://localhost:7032/");
		});

		// adding EF Core DbContext service with in-memory sql server provider
		var rateDbContext = services.AddDbContext<RateWorkerServiceDbContext>(options =>
							{
								options.UseInMemoryDatabase("rate_db");
							});
		
	})
	// install Microsoft.Extensions.Hosting.WindowsServices package 
	// add the foloowing extension method to run worker service as windows service
	.UseWindowsService()
	.Build();

host.Run();
