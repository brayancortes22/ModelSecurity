using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.Model
{
    public class RolForm
    {
        public int Id { get; set; }
        public string Permission { get; set; }
        public int RolId { get; set; }
        public Rol Rol { get; set; }
        public int FormId { get; set; }
        public Form Form { get; set; }

        // Propiedades de navegación
        public ICollection<RolForm> RolForms { get; set; }
        public ICollection<FormModule> FormModules { get; set; }

    }
}
