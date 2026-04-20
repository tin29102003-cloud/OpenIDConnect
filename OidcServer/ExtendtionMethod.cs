namespace OidcServer
{
     public static class ExtendtionMethod
    {
         public static void RegisterServiceDI(this IServiceCollection services)
        {
            services.AddSingleton<Repositories.IUserRepository, Repositories.InMemoryUserRepository>();
            services.AddSingleton<Repositories.ICodeItemRepository, Repositories.CodeItemRepository>();
        }
    }
}
