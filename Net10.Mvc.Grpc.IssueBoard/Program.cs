using Api.Grpc.IssueBoard;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

var grpcBaseUrl = builder.Configuration["GrpcServer:BaseUrl"]
    ?? throw new InvalidOperationException("GrpcServer:BaseUrl is not configured.");

builder.Services.AddGrpcClient<IssueService.IssueServiceClient>(o =>
{
    o.Address = new Uri(grpcBaseUrl);
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Issues/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Issues}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
