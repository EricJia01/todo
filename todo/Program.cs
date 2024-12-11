using todo;

var builder = WebApplication.CreateBuilder(args);
var section = builder.Configuration.GetSection("CosmosDb");
builder.Services.AddSingleton<ICosmosDbService>(InitializeCosmosClientInstanceAsync(section).GetAwaiter().GetResult());
// Add services to the container.
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
static async Task<CosmosDbService> InitializeCosmosClientInstanceAsync(IConfigurationSection configurationSection)
{
	var databaseName = "Tasks";
	var containerName = "Item";
	var account = configurationSection["AccountEndpoint"];
	var key = configurationSection["AccountKey"];
	var client = new Microsoft.Azure.Cosmos.CosmosClient(account, key);
	var database = await client.CreateDatabaseIfNotExistsAsync(databaseName);
	await database.Database.CreateContainerIfNotExistsAsync(containerName, "/id");
	var cosmosDbService = new CosmosDbService(client, databaseName, containerName);
	return cosmosDbService;
}
app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
	name: "default",
	pattern: "{controller=Item}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
