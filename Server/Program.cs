using Server.Middlewares;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers().AddProtoBufNet();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();
app.UseWhen(
    context => context.Request.Path.StartsWithSegments("/sys/servlet/PowerOn", StringComparison.InvariantCulture),
    applicationBuilder => applicationBuilder.UseAllNetRequestMiddleware());
app.UseStatusCodePages(async context =>
{
    var code = context.HttpContext.Response.StatusCode;
    if (code == 404)
    {
        app.Logger.LogWarning("Request to {Path} returned 404, type is {Type}", 
            context.HttpContext.Request.Path,
            context.HttpContext.Request.Method);
    }
});
app.Run();