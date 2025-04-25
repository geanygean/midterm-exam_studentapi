using Microsoft.EntityFrameworkCore;
using StudentAPI.Models;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(opts => opts.AddPolicy("NextApp", policy =>
{
	policy
		.WithOrigins("http://localhost:3000")
		.AllowAnyMethod()
		.AllowAnyHeader();
}));


builder.Services.AddControllers()
	.AddJsonOptions(options =>
	{
		options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
	});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


builder.Services.AddDbContext<ApplicationDbContext>(options =>
	options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();


if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.UseCors("NextApp");
app.UseAuthorization();
app.MapControllers();

app.Run();
