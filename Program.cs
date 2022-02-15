using Microsoft.EntityFrameworkCore;
using TodoApi.Models;
using TodoApi;
using Middleware.TodoApi;
using Microsoft.OpenApi.Models;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddDbContext<TodoContext>(opt =>
    opt.UseInMemoryDatabase("TodoList"));

try {
    builder.Services.AddDbContext<AppDBContext>(
        opt => opt.UseSqlServer(@"Server=localhost;Database=coba;Trusted_Connection=True")
    );
} catch(Exception e) {
    Console.WriteLine(e.ToString());
}

builder.Services.AddSwaggerGen(c =>
{
  c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme {
    In = ParameterLocation.Header, 
    Description = "Please insert JWT with Bearer into field",
    Name = "Authorization",
    Type = SecuritySchemeType.ApiKey 
  });
  c.AddSecurityRequirement(new OpenApiSecurityRequirement {
   { 
     new OpenApiSecurityScheme 
     { 
       Reference = new OpenApiReference 
       { 
         Type = ReferenceType.SecurityScheme,
         Id = "Bearer" 
       } 
      },
      new string[] { } 
    } 
  });
  
  var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
  c.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
}
);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();
app.UseWhen(context => context.Request.Path.StartsWithSegments("/WeatherForecast"), app => {
    app.UseAuth();
});

app.MapControllers();

app.Run();
