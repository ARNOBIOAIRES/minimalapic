using minimalApi.Domain.DTOs;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/", () => "Heddllo World!");

app.MapPost("/login", (LoginDTO loginDTO) => 
{
    if (loginDTO.email == "admin@test.com" && loginDTO.password == "123456")
    
        return Results.Ok("Login com sucesso");
    else
        return Results.Unauthorized();
});

app.Run();
