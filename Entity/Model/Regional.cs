using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.Model
{
    public class Regional
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string CodeRegional { get; set; }
        public string Description { get; set; }
        public string Address { get; set; }
        public bool Active { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime? DeleteDate { get; set; }
        public DateTime? UpdateDate { get; set; }

        // Propiedades de navegación
        public ICollection<Center> Centers { get; set; }
    }
}
