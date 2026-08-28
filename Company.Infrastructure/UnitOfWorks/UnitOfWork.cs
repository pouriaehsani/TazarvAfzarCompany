using Company.Application.Interfaces;
using Company.Infrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Company.Infrastructure.UnitOfWorks
{
    public class UnitOfWork: IUnitOfWork
    {
        private readonly CompanyDbContext _context;

        public UnitOfWork(CompanyDbContext context)
        {
            _context = context;
        }
        public async Task SaveChangedAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
