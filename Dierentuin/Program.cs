using Dierentuin.Data;
using Dierentuin.Models;
using Dierentuin.Services; // Ensure you have this using directive
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using System.Reflection;
using System.Text.Json.Serialization; // Add this to use JsonStringEnumConverter

var builder = WebApplication.CreateBuilder(args);

// Register services with Dependency Injection container
builder.Services.AddScoped<ZooService>();
builder.Services.AddScoped<Zoo>(provider => new Zoo
{
    Animals = new List<Animal>(),
    Enclosures = new List<Enclosure>()
});
builder.Services.AddScoped<AnimalService>();
builder.Services.AddScoped<EnclosureService>();
builder.Services.AddScoped<CategoryService>();


// Add controllers and configure JSON options to handle enum values and circular references
builder.Services.AddControllersWithViews()
    .AddJsonOptions(options =>
    {
        // This will allow enums to be serialized/deserialized as both string or numeric values
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());

        // Handle circular references with ReferenceHandler.Preserve
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve;
    });

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Dierentuin API", Version = "v1" });
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
});

builder.Services.AddDbContext<DBContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add controllers if needed, no need to call AddControllers() separately
builder.Services.AddControllers();

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

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Dierentuin API V1");
});

// Map MVC controller routes (Views)
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Animal}/{action=Index}/{id?}");
// Default route starts with Animal controller

app.MapControllers(); // Still map API controllers if needed

app.Run();