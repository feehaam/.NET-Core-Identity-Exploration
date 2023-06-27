using IdentityExploration.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace IdentityExploration
{
    // 9. The difference point is instead of Datacontext : DbContext use used the following, it works because
    // IdentityDbContext<T> extends DbContext after all. So it's just DbContext but with additional features
    // 10. The generic Employee is used because of extending IdentityUser by Employee for making custom user 
    // with additional fileds rather than IdentityUser fixed fields.
    public class DataContext : IdentityDbContext<Employee>
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options) 
        {

        }

        DbSet<Employee> Employees { get; set; }
    }
}
