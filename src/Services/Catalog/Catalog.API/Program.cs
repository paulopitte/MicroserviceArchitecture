var builder = WebApplication.CreateBuilder(args);


// add services to the container.
builder.Services.AddCarter();
builder.Services.AddMediatR(config =>
{
    config.RegisterServicesFromAssembly(typeof(Program).Assembly);
});

var app = builder.Build();



// Configure the http request Pipeline
app.MapCarter();

app.MapGet("/", () => "Hello World!");




app.Run();
