using Company.Application.Interfaces;
using Company.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Company.Application.Services
{
    public class BaseInfo : IBaseInfo
    {
        private readonly IGenericRepository<Category> _cat;
        //private readonly IUnitOfWork _UnitOfWork;
        public BaseInfo(
            IGenericRepository<Category> genericRepository,
            IUnitOfWork unitOfWork)
        {
            _cat = genericRepository;
            //_UnitOfWork = unitOfWork;
        }
        public async Task<List<Category>> GetAllAsync()
        {
            return await _cat.GetAllAsync();
        }

        public Task CreateAsync(Category cat)
        {
            throw new NotImplementedException();
        }

        public Task DeleteAsync(int id)
        {
            throw new NotImplementedException();
        }
        
        public Task<Category?> GetByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task UpdateAsync(Category cat)
        {
            throw new NotImplementedException();
        }
    }
}
