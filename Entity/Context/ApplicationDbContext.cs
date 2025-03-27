using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


using Dapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Configuration;
using System.Data;
using System.Reflection;
using Entity.Model;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Diagnostics.Contracts;
using System.Runtime.InteropServices;

namespace Entity.Context
{
    internal class ApplicationDbContext
    {
        /// <summary>

        /// Actualiza un rol existente en la base de datos.

        /// </summary>

        /// <param name="rol">Objeto con la información actualizada. </param>

        /// <returns>True si la operación fue exitosa, False en caso contrario.</returns>
        public async Task<bool> UpdateAsync(Rol rol)
        {
            try
            {
                _context.Set<Rol>().Update(rol);

                await _context.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al actualizar el rol: {ex.Message}");
                return false;
            }
        }
    }
}
