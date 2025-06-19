namespace Project.Middlewares;

public static class ExampleMiddlewaresExtension
{
    public static void UseExampleMiddlewares(this IApplicationBuilder app)
    {
        app.Use(async (context, next) =>
        {
            
            Console.WriteLine("Middleware1 called");
            await next.Invoke();
            
            Console.WriteLine("Middleware1 after next");
        });

        app.Use(async (context, next) =>
        {
            Console.WriteLine("Middleware2 called");
            await next.Invoke();
            Console.WriteLine("Middleware2 after next");
        });
    }
}