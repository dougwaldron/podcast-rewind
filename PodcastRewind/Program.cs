using PodcastRewind.Services;
using Polly;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.UseSentry();

builder.Services.AddHttpClient("Polly")
    .AddTransientHttpErrorPolicy(policy => policy.WaitAndRetryAsync(2, _ => TimeSpan.FromMilliseconds(600)));
builder.Services.AddHsts(options => options.MaxAge = TimeSpan.FromDays(365));
builder.Services.AddRazorPages();
builder.Services.AddControllers();
builder.Services.AddTransient<IFeedRewindRepository, FeedRewindRepository>();
builder.Services.AddTransient<ISyndicationFeedService, SyndicationFeedService>();
builder.Services.AddWebOptimizer();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
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
