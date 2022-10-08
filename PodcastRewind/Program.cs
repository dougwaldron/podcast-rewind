using PodcastRewind.Services;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.UseSentry();
builder.Services.AddRazorPages();
builder.Services.AddControllers();
builder.Services.AddTransient<IFeedRewindRepository, FeedRewindRepository>();

// Configure bundling and minification.
builder.Services.AddWebOptimizer();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseWebOptimizer();
app.UseStaticFiles();
app.UseRouting();
app.UseSentryTracing();
app.MapRazorPages();
app.MapControllers();

// Make it so.
app.Run();
