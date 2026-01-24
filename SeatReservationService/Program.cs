using SeatReservation.Application;
using SeatReservation.Application.DataBase;
using SeatReservation.Domain;
using SeatReservation.Infrastructure.Postgres;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<ReservationServiceDbContext>(_ =>
    new ReservationServiceDbContext(builder.Configuration.GetConnectionString("ReservationServiceDb")!));

builder.Services.AddScoped<IReservationServiceDbContext, ReservationServiceDbContext>(_ =>
    new ReservationServiceDbContext(builder.Configuration.GetConnectionString("ReservationServiceDb")!));

builder.Services.AddScoped<CreateVenueHandler>();

builder.Services.AddOpenApi();
builder.Services.AddControllers();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(op => op.SwaggerEndpoint("/openapi/v1.json", "AuthService"));
}

// app.MapPost(
//    "/users",
//    async (ReservationServiceDbContext dbContext) =>
//    {
//        var socials = new SocialNetwork()
//        {
//            Link = "Test", Name = "Test",
//        };
//        await dbContext.AddAsync(new User()
//        {
//            Details = new Details()
//            {
//                Description = "Test", FIO = "Test", Socials = [socials],
//            },
//        });
//        await dbContext.SaveChangesAsync();
//    });

// app.MapGet(
//   "/users",
//   async (ReservationServiceDbContext dbContext) =>
//   {
//       await dbContext.Set<User>().ToListAsync();
//   });
// app.UseHttpsRedirection();
// app.UseAuthorization();
app.MapControllers();
app.Run();