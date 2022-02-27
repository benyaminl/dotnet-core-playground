using Microsoft.EntityFrameworkCore;
using TodoApi.Models;
using TodoApi;
using Middleware.TodoApi;
using Microsoft.OpenApi.Models;
using System.Reflection;
using TodoApi.Filters;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Minio.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddDbContext<TodoContext>(opt =>
    opt.UseInMemoryDatabase("TodoList"));

builder.Services.AddMinio(opt => {
    opt.Endpoint = "172.18.51.72:9000";
    opt.AccessKey = "api-coba";
    opt.SecretKey = "cUQ4A6ytTPrY5jt";
});

try {
    builder.Services.AddDbContext<AppDBContext>(
        opt => opt.UseSqlServer(@"Server=localhost;Database=coba;Trusted_Connection=True")
    );
} catch(Exception e) {
    Console.WriteLine(e.ToString());
}

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, 
    options => {
        // builder.Configuration.Bind("JwtSettings", options);

        // options.RequireHttpsMetadata = false;
        options.SaveToken = false;
        options.TokenValidationParameters = new TokenValidationParameters()
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidAudience = "LocalDev",
            ValidIssuer = "LocalDev",
            // Secret key nya
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("971923712907391273921"))
        };

    });

builder.Services.AddSwaggerGen(c =>
{
  c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme {
    In = ParameterLocation.Header, 
    Description = "Please insert JWT with Bearer into field",
    Name = "Authorization",
    Type = SecuritySchemeType.ApiKey 
  });
  // c.AddSecurityRequirement(new OpenApiSecurityRequirement {
  //  { 
  //    new OpenApiSecurityScheme 
  //    { 
  //      Reference = new OpenApiReference 
  //      { 
  //        Type = ReferenceType.SecurityScheme,
  //        Id = "Bearer" 
  //      } 
  //     },
  //     new string[] { } 
  //   } 
  // });

  // This is for Making Auto Swagger Filter for Auth and non Auth
  c.OperationFilter<AuthOperationFilter>();
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
// global cors policy
app.UseCors(x => x
    .AllowAnyOrigin()
    .AllowAnyMethod()
    .AllowAnyHeader());

app.UseHttpsRedirection();
app.UseStatusCodePages("application/json","{{\"status\":{0}, \"message\": \"Oops, are you sure resource/end point really here? Or probably bad requests?\" }}");

app.UseAuthentication();
app.UseAuthorization();
app.UseWhen(context => context.Request.Path.StartsWithSegments("/WeatherForecast"), app => {
    app.UseAuth();
});

app.MapControllers();

app.Run();
