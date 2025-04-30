using Syncfusion.Licensing;

var builder = WebApplication.CreateBuilder(args);

SyncfusionLicenseProvider.RegisterLicense("Mgo+DSMBMAY9C3t2XFhhQlJHfV5AQmBIYVp/TGpJfl96cVxMZVVBJAtUQF1hTH5Xd0NiWn9cdHxTRWFVWkZ/");

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

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

app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();
app.MapRazorPages()
   .WithStaticAssets();

app.MapControllers();

app.Run();
