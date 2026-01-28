using Dwarf.Digger.AppConfig;
using DD = Dwarf.Digger;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddSignalR();
builder.Services.AddCors(options =>
{
	options.AddDefaultPolicy(policy =>
	{
		policy.AllowAnyOrigin();
	});
});
// Local part
builder.AddConfigs();
builder.Services.AddBatch<DD.Interaction.Services>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
	app.UseExceptionHandler("/Home/Error");
}
app.UseStaticFiles();
app.UseRouting();
app.UseCors();

app.UseAuthorization();

app.MapControllerRoute(
	name: "default",
	pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapHub<DD.Interaction.ArduinoHub>("/arduino");

app.Run(app.Configuration.GetValue("HostUrl", "http://*:5000"));
