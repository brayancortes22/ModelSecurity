using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.Model
{
    public class InstructorProgram
    {
        public int Id { get; set; }
        public int InstructorId { get; set; }
        public int ProgramId { get; set; }
        
        // Propiedades de navegación    
        public Instructor Instructor { get; set; }
        public Program Program { get; set; }
        public ICollection<InstructorProgram> InstructorPrograms { get; set; }
        public ICollection<AprendizProcessInstructor> AprendizProcessInstructors { get; set; }
    }
}
