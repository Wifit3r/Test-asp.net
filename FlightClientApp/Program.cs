using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Threading.Tasks; // <-- додано

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();
builder.Services.AddHttpClient(); 

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}
app.UseRouting();

app.UseEndpoints(endpoints =>
{
    endpoints.MapRazorPages();
    endpoints.MapGet("/", context =>
    {
        context.Response.Redirect("/Flights");
        return Task.CompletedTask; // тепер Task доступний
    });
});

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAuthorization();

app.MapRazorPages();

app.Run();