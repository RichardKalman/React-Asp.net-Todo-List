using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TodosApplication.Model;

namespace TodosApplication.DAL
{
    public interface IDbContext
    {
        DbSet<Todo> Todo { get; set; }
        DbSet<TodoType> TodoTypes { get; set; }
        int SaveChanges();
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
