using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.EntityFrameworkCore;
using NLBAudit.AspNetCore.Controllers;
using NLBAudit.AspNetCore.Extensions;
using NLBAudit.AspNetCore.Mvc.Extensions;
using NLBAudit.Sample;
using NLBAudit.Store.EfCore.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddUserSecrets<Program>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpContextAccessor();
builder.Services.ConfigureAspNetCoreAuditing<int>(config =>
{
    config.IsEnabled = true;
    config.IsEnabledForAnonymousUsers = true;
    config.SaveReturnValues = true;
});
builder.Services.AddDbContext<MyDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("MyConnectionString"));
});

builder.Services.ConfigureAuditingEfCoreStore<MyDbContext>();
builder.Services.AddControllers(options =>
{
    options.AddAuditingFilter();
}).PartManager.ApplicationParts.Add(new AssemblyPart(typeof(AuditLogController).Assembly));

var app = builder.Build();

// Configure the HTTP request pipeline.
if(app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseRouting();

app.MapControllers();

MinimalApis.Configure(app);