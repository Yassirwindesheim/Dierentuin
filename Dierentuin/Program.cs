using Dierentuin.Services; // Ensure you have this using directive
using System.Text.Json.Serialization; // Add this to use JsonStringEnumConverter

var builder = WebApplication.CreateBuilder(args);

// Register services with Dependency Injection container
builder.Services.AddScoped<ZooService>();
builder.Services.AddScoped<AnimalService>();
builder.Services.AddScoped<EnclosureService>();
builder.Services.AddScoped<CategoryService>();

// Add controllers and views, and configure JSON options to handle enum values correctly
builder.Services.AddControllersWithViews().AddJsonOptions(options =>
{
    // This will allow enums to be serialized/deserialized as both string or numeric values
    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

// Map MVC controller routes (Views)
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Animal}/{action=Test}/{id?}");
// Default route starts with Animal controller

app.MapControllers(); // Still map API controllers if needed

app.Run();
