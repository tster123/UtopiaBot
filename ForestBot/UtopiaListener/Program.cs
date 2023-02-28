using System.Text;
using Microsoft.AspNetCore.Http.Extensions;
using NLog;
using UtopiaListener.Listener;

namespace UtopiaListener
{
    public class Program
    {
        private static Logger log = LogManager.GetCurrentClassLogger();
        private static IntelAccepter intel = new IntelAccepter();
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
                    context.Response.Headers["Access-Control-Allow-Origin"] = "*";
                    context.Response.Headers["Access-Control-Allow-Methods"] = "POST";
                    context.Response.Headers["Access-Control-Max-Age"] = "1000";

                    if (url.ToLower().EndsWith("/api/intel") &&
                        context.Request.Method.ToLower() == "post")
                    {
                        try
                        {
                            string ret = "{\"success\": true}";
                            var requestHeaders = context.Request.Headers.ToDictionary(h => h.Key, h => h.Value.First() ?? "");
                            intel.HandleRequest(requestHeaders, body);
                            context.Response.ContentType = "text/plain";
                            //context.Response.Headers["Content-Type"] = 
                            context.Response.ContentLength = Encoding.UTF8.GetBytes(ret).Length;
                            await context.Response.Body.WriteAsync(Encoding.UTF8.GetBytes(ret));
                            //context.Response.Body.Flush();
                            //context.Response.Body.Close();
                            //StreamWriter writer = new StreamWriter(context.Response.Body);
                            //writer.Write(ret);
                            return;
                        }
                        catch (Exception)
                        {

                        }
                    }

                    await next.Invoke();
                    context.Request.Body = initialBody;
                }
            });

            app.Run();
        }
    }
}