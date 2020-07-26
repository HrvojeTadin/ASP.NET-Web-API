using HoteliAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace HoteliAPITests
{
    public static class Utility
    {
        private static readonly string connectionString = "Server=DESKTOP-RGHHS6S;Database=HoteliTest;Trusted_Connection=True;";

        public static HoteliContext GetDbContext()
        {
            var optionsBuilder = new DbContextOptionsBuilder<HoteliContext>().UseSqlServer(connectionString);
            return new HoteliContext(optionsBuilder.Options);
        }
    }
}
