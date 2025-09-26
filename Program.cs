using minimalApi.Domain.DTOs;
using minimalApi.Infraestrutura.Database;
using Microsoft.EntityFrameworkCore;
using minimalApi.Domain.Interfaces;
using minimalApi.Domain.Services;
using Microsoft.AspNetCore.Mvc;
using minimalApi.Domain.Models;
using minimalApi.Domain.Entities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.OpenApi.Models;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;


#region Builder
var builder = WebApplication.CreateBuilder(args);

var key = builder.Configuration.GetSection("Jwt").ToString();
if (string.IsNullOrEmpty(key))
    key = "1234567890123456"; // chave padrão para desenvolvimento


builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateLifetime = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
    };
});

builder.Services.AddAuthorization();

builder.Services.AddScoped<IAdministradorService, AdministradorService>();
builder.Services.AddScoped<IVeiculoService, VeiculoService>();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        Description = "Insira o token JWT {seu token}"
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

  var app = builder.Build();
#endregion

#region Home
app.MapGet("/", () => Results.Json(new Home())).AllowAnonymous().WithTags("Home");
#endregion


#region Administradores
string GerarTokenJwt(Administrador administrador)
{
    var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
    var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

    var claims = new List<Claim>
    {
        new Claim("Email", administrador.Email),
        new Claim("Perfil", administrador.Perfil),
        new Claim(ClaimTypes.Role, administrador.Perfil)
    };

    var token = new JwtSecurityToken(
        claims: claims,
        expires: DateTime.Now.AddHours(1),
        signingCredentials: credentials
    );

    return new  JwtSecurityTokenHandler().WriteToken(token);
}

// 


app.MapPost("/administradores/login", ([FromBody] LoginDTO loginDTO, IAdministradorService administradorService) =>
{
var adm =  administradorService.Login(loginDTO);
if (adm != null) {
    string token = GerarTokenJwt(adm);

    return Results.Ok(new AdmLogado
    {
        Email = adm.Email,
        Perfil = adm.Perfil,
        Token = token
    });
}
else
    return Results.Unauthorized();
}).AllowAnonymous().WithTags("Administradores");


// buscar todos os administradores
app.MapGet("/administradores", ([FromQuery] int? page, [FromQuery] int pageSize, IAdministradorService administradorService) =>
{
    var adms = new List<AdministradorModelView>();

    var administradores = administradorService.GetAll(page, pageSize);
    foreach (var adm in administradores)
    {
        adms.Add(new AdministradorModelView
        {
            Id = adm.Id,
            Email = adm.Email,
            Perfil = adm.Perfil
        });
    }
    return Results.Ok(adms);
})
.RequireAuthorization()
.RequireAuthorization(new AuthorizeAttribute { Roles = "Adm" })
.WithTags("Administradores");


// buscar administrador por id
app.MapGet("/administradores/{id}", (int id, IAdministradorService administradorService) =>
{
    var administrador = administradorService.GetById(id);
    return administrador != null ? Results.Ok(new AdministradorModelView
        {
            Id = administrador.Id,
            Email = administrador.Email,
            Perfil = administrador.Perfil
        }) : Results.NotFound();
})
.RequireAuthorization()
.RequireAuthorization(new AuthorizeAttribute { Roles = "Adm" })
.WithTags("Administradores");

app.MapPost("/administradores", ([FromBody] AdministradorDTO administradorDTO, IAdministradorService administradorService) =>
{
    
    var validations = new Validations
    {
        Mensagens = new List<string>()
    };
  
    if (string.IsNullOrWhiteSpace(administradorDTO.Email))
        validations.Mensagens.Add("O email é obrigatório.");
    if (string.IsNullOrWhiteSpace(administradorDTO.Senha))

        validations.Mensagens.Add("A senha é obrigatória.");

    if (administradorDTO.Perfil == null)
        validations.Mensagens.Add("O perfil é obrigatório.");

    if (validations.Mensagens.Count > 0)
        return Results.BadRequest(validations);

    var administrador = new Administrador
{
    Email = administradorDTO.Email,
    Senha = administradorDTO.Senha,
    Perfil = administradorDTO.Perfil.ToString() ?? "Editor"
};
    administradorService.Incluir(administrador);

return Results.Created($"/administradores/{administrador.Id}", new AdministradorModelView
        {
            Id = administrador.Id,
            Email = administrador.Email,
            Perfil = administrador.Perfil
        });
})
.RequireAuthorization()
.RequireAuthorization(new AuthorizeAttribute { Roles = "Adm" })
.WithTags("Administradores");


#endregion



#region Veiculos 
Validations validaDTO(VeiculoDTO veiculoDTO)
{
    var validations = new Validations();
    validations.Mensagens = new List<string>();

    if (string.IsNullOrWhiteSpace(veiculoDTO.Nome))
        validations.Mensagens.Add("O nome do veículo é obrigatório.");
    if (string.IsNullOrWhiteSpace(veiculoDTO.Marca))
        validations.Mensagens.Add("A marca do veículo é obrigatória.");
    if (veiculoDTO.Ano < 1886 || veiculoDTO.Ano > DateTime.Now.Year + 1) // O primeiro carro foi inventado em 1886
        validations.Mensagens.Add("O ano do veículo é inválido.");

    return validations;
}

app.MapPost("/veiculos",  ([FromBody] VeiculoDTO veiculoDTO, IVeiculoService veiculoService) =>
{

    var validations = validaDTO(veiculoDTO);
    if (validations.Mensagens.Count > 0)
        return Results.BadRequest(validations);

    var veiculo = new Veiculo
{
    Nome = veiculoDTO.Nome,
    Marca = veiculoDTO.Marca,
    Ano = veiculoDTO.Ano
};
veiculoService.AddVeiculo(veiculo);
return Results.Created($"/veiculos/{veiculo.Id}", veiculo);
})
.RequireAuthorization()
.RequireAuthorization(new AuthorizeAttribute { Roles = "Adm,Editor" })
.RequireAuthorization().WithTags("Veículos");


// Listar todos os veículos
app.MapGet("/veiculos", (IVeiculoService veiculoService) =>
{
    var veiculos = veiculoService.GetAllVeiculos();
    return Results.Ok(veiculos);
}).WithTags("Veículos");

app.MapGet("/veiculos/{id}", (int id, IVeiculoService veiculoService) =>
{
    var veiculo = veiculoService.GetVeiculoById(id);
    return veiculo != null ? Results.Ok(veiculo) : Results.NotFound();
})
.RequireAuthorization()
.RequireAuthorization(new AuthorizeAttribute { Roles = "Adm,Editor" })
.WithTags("Veículos");



//atualizar veiculo
app.MapPut("/veiculos/{id}", (int id, [FromBody] VeiculoDTO veiculoDTO, IVeiculoService veiculoService) =>
{
    var veiculo = veiculoService.GetVeiculoById(id);
    if (veiculo == null)
        return Results.NotFound();

    var validations = validaDTO(veiculoDTO);
    if (validations.Mensagens.Count > 0)
        return Results.BadRequest(validations);

    veiculo.Nome = veiculoDTO.Nome;
    veiculo.Marca = veiculoDTO.Marca;
    veiculo.Ano = veiculoDTO.Ano;

    veiculoService.UpdateVeiculo(veiculo);
    return Results.Ok(veiculo);
}).RequireAuthorization()
.RequireAuthorization(new AuthorizeAttribute { Roles = "Adm" })
.WithTags("Veículos");

// Deletar veiculo
app.MapDelete("/veiculos/{id}", (int id, IVeiculoService veiculoService) =>
{
    var veiculo = veiculoService.GetVeiculoById(id);
    if (veiculo == null)
        return Results.NotFound();

    veiculoService.DeleteVeiculo(veiculo);
    return Results.NoContent();
}).RequireAuthorization()
.RequireAuthorization(new AuthorizeAttribute { Roles = "Adm" })
.WithTags("Veículos");
#endregion



#region App
app.UseSwagger();
app.UseSwaggerUI();

app.UseAuthentication();
app.UseAuthorization();

app.Run();
#endregion