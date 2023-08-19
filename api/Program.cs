using api;
using Microsoft.EntityFrameworkCore;
using repo_client;
using repo_client.Data;
using repo_client.Models;
using repo_document;
using repo_email;
using FluentValidation;
using System.Diagnostics.Eventing.Reader;

var builder = WebApplication.CreateBuilder(args);

var services = builder.Services;

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
services.AddEndpointsApiExplorer();
services.AddSwaggerGen();

// cors
services.AddCors(options =>
{
    options.AddDefaultPolicy(builder => builder
        .SetIsOriginAllowedToAllowWildcardSubdomains()
        .WithOrigins("http://localhost:3000")
        .AllowAnyHeader()
        .AllowAnyMethod()
        .AllowCredentials()
        .Build());
});

// ioc
services.AddDbContext<DataContext>(options => options.UseInMemoryDatabase(databaseName: "Test"));

services.AddScoped<DataSeeder>();
services.AddScoped<IClientRepository, ClientRepository>();
services.AddScoped<IEmailRepository, EmailRepository>();
services.AddScoped<IDocumentRepository, DocumentRepository>();
services.AddScoped<IValidator<Client>, ClientValidator>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.MapGet("/clients", async (string? name, IClientRepository clientRepository) =>
{
    if(String.IsNullOrEmpty(name))
        return await clientRepository.Get();
    else
        return await clientRepository.Search(name);
})
.WithName("GetClients");


app.MapGet("/clients/{id}", async (string id, IClientRepository clientRepository) =>
{
    return await clientRepository.Get(id);
})
.WithName("GetClient");




//app.MapGet("/clients", async (string name, IClientRepository clientRepository) =>
//{
//    return await clientRepository.Search(name);
//})
//.WithName("SearchClient");





app.MapPost("/clients", async (IValidator<Client> validator, IClientRepository clientRepository, Client client) =>
{
    var validationResult = await validator.ValidateAsync(client);
    if (!validationResult.IsValid)
    {
        return Results.ValidationProblem(validationResult.ToDictionary());
    }

    if(await clientRepository.Create(client))
        return Results.Created($"/clients/{client.Id}", client);
    else
        return Results.Problem();        
})
.WithName("CreateClient");


app.MapPut("/clients/{id}", async (string id, IValidator<Client> validator, IClientRepository clientRepository, Client inputClient) =>
{
    var validationResult = await validator.ValidateAsync(inputClient);
    if (!validationResult.IsValid) 
        return Results.ValidationProblem(validationResult.ToDictionary());


    if (await clientRepository.Update(inputClient))
        return Results.Accepted($"/clients/{inputClient.Id}", inputClient);
    else
        return Results.Problem();
});



app.UseCors();

// seed data
using (var scope = app.Services.CreateScope())
{
    var dataSeeder = scope.ServiceProvider.GetRequiredService<DataSeeder>();

    dataSeeder.Seed();
}

// run app
app.Run();