using HotelAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace HotelAPITests
{
    public static class Utility
    {
        private static readonly string connectionString = "Server=MyServerNam;Database=HotelsTest;Trusted_Connection=True;";

        public static HotelsContext GetDbContext()
        {
            var optionsBuilder = new DbContextOptionsBuilder<HotelsContext>().UseSqlServer(connectionString);
            return new HotelsContext(optionsBuilder.Options);
        }
    }
}
