using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Entity.Contexts;
using Entity.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Data
{
    public class ModuleData
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger _logger;

        public ModuleData(ApplicationDbContext context, ILogger logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IEnumerable<Module>> GetAllAsync()
        {
            try
            {
                var modules = await _context.Set<Module>().Where(x => x.Active).ToListAsync();
                return modules;
            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener los módulos", ex);
            }
        }

        public async Task<Module> GetByidAsync(int id)
        {
            try
            {
                var module = await _context.Set<Module>().FirstOrDefaultAsync(x => x.Id == id);
                return module;
            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener el módulo", ex);
            }
        }

        public async Task<Module> CreateAsync(Module module)
        {
            try
            {
                module.CreateDate = DateTime.Now;
                module.Active = true;
                _context.Set<Module>().Add(module);
                await _context.SaveChangesAsync();
                return module;
            }
            catch (Exception ex)
            {
                throw new Exception("Error al crear el módulo", ex);
            }
        }

        public async Task<bool> UpdateAsync(Module module)
        {
            try
            {
                _context.Set<Module>().Update(module);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al actualizar el módulo {ex.Message}");
                return false;
            }
        }

        public async Task<bool> DeleteAsync(int id)
        {
            try
            {
                var module = await _context.Set<Module>().FindAsync(id);
                if (module == null)
                    return false;

                _context.Set<Module>().Remove(module);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al eliminar el módulo {ex.Message}");
                return false;
            }
        }
    }
}
