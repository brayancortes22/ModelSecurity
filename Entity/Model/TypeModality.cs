using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.Model
{
    public class TypeModality
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool Active { get; set; }

        // Propiedades de navegación
        public ICollection<AprendizProcessInstructor> AprendizProcessInstructors { get; set; }
        public ICollection<InstructorProgram> InstructorPrograms { get; set; }
    }
}
