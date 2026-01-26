using SeatReservation.Application.DataBase;
using SeatReservation.Application.Venues;
using SeatReservation.Infrastructure.Postgres;
using SeatReservation.Infrastructure.Postgres.Database;
using SeatReservation.Infrastructure.Postgres.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<ReservationServiceDbContext>(_ =>
    new ReservationServiceDbContext(builder.Configuration.GetConnectionString("ReservationServiceDb")!));

builder.Services.AddScoped<IVenuesRepository, NpgSqlVenuesRepository>();

//builder.Services.AddScoped<IVenuesRepository, EfCoreVenuesRepository>();
builder.Services.AddScoped<CreateVenueHandler>();
builder.Services.AddScoped<UpdateVenueNameHandler>();
builder.Services.AddScoped<UpdateVenueNameByPrefixHandler>();

builder.Services.AddSingleton<IDbConnectionFactory, NpgSqlConnectionFactory>();

builder.Services.AddOpenApi();
builder.Services.AddControllers();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(op => op.SwaggerEndpoint("/openapi/v1.json", "AuthService"));
}

// app.UseHttpsRedirection();
// app.UseAuthorization();
app.MapControllers();
app.Run();