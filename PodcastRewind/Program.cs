using PodcastRewind.Services;
using PodcastRewind.TestData;
using Polly;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.UseSentry();

builder.Services.AddHttpClient("Polly")
    .AddTransientHttpErrorPolicy(policy => policy.WaitAndRetryAsync(2, _ => TimeSpan.FromMilliseconds(600)));
builder.Services.AddHsts(options => options.MaxAge = TimeSpan.FromDays(365));
builder.Services.AddRazorPages();
builder.Services.AddControllers();
builder.Services.AddWebOptimizer();

// Configure feed services.
if (builder.Configuration.GetValue<bool>("UseTestData"))
{
    builder.Services.AddTransient<IFeedRewindInfoRepository, TestFeedRewindInfoRepository>();
    builder.Services.AddTransient<ISyndicationFeedService, TestSyndicationFeedService>();
}
else
{
    builder.Services.AddTransient<IFeedRewindInfoRepository, FeedRewindInfoRepository>();
    builder.Services.AddTransient<ISyndicationFeedService, SyndicationFeedService>();
}

builder.Services.AddTransient<IFeedRewindDataService, FeedRewindDataService>();

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
