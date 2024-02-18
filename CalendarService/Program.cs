var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();


app.UseSwagger();
app.UseSwaggerUI();


app.UseHttpsRedirection();

HashSet<System.DayOfWeek> weekends = new[] { DayOfWeek.Saturday, DayOfWeek.Sunday }.ToHashSet();

app.MapGet("/SwedishWorkingDays", (int starYear, int endYear) =>
{
    var result = new List<MonthInfo>();
    if (endYear>=starYear)
        for (int y = starYear; y<=endYear; y++)
        {
            var holiDays = new PublicHoliday.SwedenPublicHoliday().PublicHolidays(y).Select(r => DateOnly.FromDateTime(r.Date)).ToHashSet();
            for (int m = 1; m<=12; m++)
            {

                var days = new DateOnly(y, m, 1).AddMonths(1).AddDays(-1).Day;
                var workingDays = 0;
                for (int i = 1; i <= days; i++)
                {
                    var d = new DateOnly(y, m, i);
                    if (!weekends.Contains(d.DayOfWeek) && !holiDays.Contains(d))
                        workingDays++;
                }
                //Special case when 6/6 occurs on a weekend
                if (m==6 && y>=2005)
                {
                    var d = new DateOnly(y, 6, 6);
                    if (weekends.Contains(d.DayOfWeek))
                        workingDays--;
                }
                result.Add(new MonthInfo(y, m, workingDays));
            }
        }
    return result;
})
.WithName("GetSwedishWorkingDays")
.WithOpenApi();

app.Run();


internal record MonthInfo(int Year, int Month, int WokingDays)
{    
}