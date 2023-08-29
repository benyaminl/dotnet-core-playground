using Microsoft.EntityFrameworkCore;
using EntityFrameworkCore.UseRowNumberForPaging;
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
using Minio;
using TodoApi.Middleware;
using Probst.AspNetCore.Nats;
using TodoApi.Authentication.Roles;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddDbContext<TodoContext>(opt =>
    opt.UseInMemoryDatabase("TodoList"));

// @see https://docs.microsoft.com/en-us/aspnet/core/fundamentals/configuration/?view=aspnetcore-6.0#environment-variables
// Also see file appsettings.json or appsettings.<X>.json @more https://docs.microsoft.com/en-us/aspnet/core/fundamentals/configuration/?view=aspnetcore-6.0#default-configuration
string host = builder.Configuration.GetValue<string>("MSSQL_HOST");
string user = builder.Configuration.GetValue<string>("MSSQL_USER");
string pass = builder.Configuration.GetValue<string>("MSSQL_PASS");
int port = builder.Configuration.GetValue<int>("MSSQL_PORT");
string db   = builder.Configuration.GetValue<string>("MSSQL_DB");
builder.Services.AddDbContext<AppDBContext>(
    opt => opt.UseSqlServer(@"Server="+host+","+port.ToString()
        +";Database="+db+";User Id="+user+";Password="+pass, 
            i => i.UseRowNumberForPaging())
);

builder.Services.AddNats(o => {
    o.Url = "nats://" + builder.Configuration.GetValue<string>("NATS_HOST") + ":" 
      + builder.Configuration.GetValue<string>("NATS_PORT");
    o.User = builder.Configuration.GetValue<string>("NATS_USER");
    o.Password = builder.Configuration.GetValue<string>("NATS_PASS");
});

builder.Services.AddMinio(opt => {
    opt.Endpoint = builder.Configuration.GetValue<string>("MINIO_ENDPOINT");
    opt.AccessKey = builder.Configuration.GetValue<string>("MINIO_ACCESS_KEY");
    opt.SecretKey = builder.Configuration.GetValue<string>("MINIO_SECRET_KEY");
});

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

// For Roles example, to be clean hidden as extened function for a class
builder.Services.AddAuthorizationRoles();

// For Builtin Well.. Use...
// Uncoment this and comment the custom JWT Builder above
// builder.Services.AddAuthentication()
//     .AddJwtBearer("Bearer", o => {
//         o.SaveToken = false;
//         var secret = builder.Configuration.GetValue<string>("Authentication:Schemes:Bearer:IssuerSigningKey") ?? "";
        
//         o.TokenValidationParameters
//             .IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
//         Console.WriteLine(o.TokenValidationParameters.IssuerSigningKey.ToString());
//     });

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
    app.UseSwagger(options =>
    {
        // @see https://github.com/domaindrivendev/Swashbuckle.AspNetCore/issues/2004#issuecomment-848332122
        //Workaround to use the Swagger UI "Try Out" functionality when deployed behind a reverse proxy (APIM) with API prefix /sub context configured
        options.PreSerializeFilters.Add((swagger, httpReq) =>
        {
            if (httpReq.Headers.ContainsKey("X-Forwarded-Host"))
            {
                //The httpReq.PathBase and httpReq.Headers["X-Forwarded-Prefix"] is what we need to get the base path.
                //For some reason, they returning as null/blank. Perhaps this has something to do with how the proxy is configured which we don't have control.
                //For the time being, the base path is manually set here that corresponds to the APIM API Url Prefix.
                //In this case we set it to 'sample-app'. 
        
                var basePath = "~ben/work";
                var serverUrl = $"{httpReq.Scheme}://{httpReq.Headers["X-Forwarded-Host"]}/{basePath}";
                swagger.Servers = new List<OpenApiServer> { new OpenApiServer { Url = serverUrl } };
            }
        });
    });
    app.UseSwaggerUI();
}
// global cors policy
app.UseCors(x => x
    .AllowAnyOrigin()
    .AllowAnyMethod()
    .AllowAnyHeader());

app.UseHttpsRedirection();
app.UseStatusCodePages("application/json","{{\"status\":{0}, \"message\": \"Oops, are you sure resource/end point really here? Or probably bad requests?\" }}");
// Global Error Middleware
app.UseMiddleware<ErrorCatchMiddleware>();

app.UseAuthentication();
app.UseAuthorization();
app.UseWhen(context => context.Request.Path.StartsWithSegments("/WeatherForecast"), app => {
    app.UseAuth();
});

app.MapControllers();

app.Run();
