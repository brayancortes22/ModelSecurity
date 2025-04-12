using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.Model
{
    public class Aprendiz
    {
        public int Id { get; set; }
        public string PreviousProgram { get; set; }
        public bool Active { get; set; }
        public int UserId { get; set; } 

        // Propiedades de navegación
        public User User { get; set; }
        public ICollection<AprendizProgram> AprendizPrograms { get; set; }
        public ICollection<AprendizProcessInstructor> AprendizProcessInstructors { get; set; }
        
    }
}
