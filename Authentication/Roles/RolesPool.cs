namespace TodoApi.Authentication.Roles;

public static class RolesPool
{
    public static IServiceCollection AddAuthorizationRoles(this IServiceCollection services)
    {
        services.AddAuthorization(o => {
            o.AddPolicy("RequireAdminRole", p => p.RequireRole("admin"));
        });
        
        return services;
    }
}