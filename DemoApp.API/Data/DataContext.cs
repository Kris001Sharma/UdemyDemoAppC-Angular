using DemoApp.API.Models;
using Microsoft.EntityFrameworkCore;

namespace DemoApp.API.Data
{
    public class DataContext : DbContext
    { 
        public DataContext(DbContextOptions<DataContext> options) : base(options){}
        // provide some options inside DataContext and specify the type (DataContext class) used for DataContext

        // Tell DC class about our entities
        public DbSet<Value> Values { get; set; }
        // Values : represents the table name that gets created when we scaffold DB
        
        // We need to let the application know about DataContext class.
        // => Need to make DataContext available as service to be consumed in other parts
        // => Add to ConfigureServices
    }
}