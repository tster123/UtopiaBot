using System.Text;
using Microsoft.AspNetCore.Http.Extensions;
using NLog;

namespace UtopiaListener
{
    public class Program
    {
        private static Logger log = LogManager.GetCurrentClassLogger();

        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddRazorPages();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.MapRazorPages();

            app.Use(async (context, next) =>
            {
                var initialBody = context.Request.Body;

                using (var bodyReader = new StreamReader(context.Request.Body))
                {
                    string body = await bodyReader.ReadToEndAsync();
                    string url = context.Request.GetDisplayUrl();
                    string headers = string.Join("\n", context.Request.Headers.Select(h => h.Key + ":" + h.Value));
                    log.Info(url + "\nHEADERS:" + headers + "\nBODY:\n" + body);
                    Console.WriteLine(body);
                    context.Request.Body = new MemoryStream(Encoding.UTF8.GetBytes(body));
                    
                    await next.Invoke();
                    context.Request.Body = initialBody;
                }
            });

            app.Run();
        }
    }
}