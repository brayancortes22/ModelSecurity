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
        public Instructor Instructor { get; set; }
        public int ProgramId { get; set; }
        public Program Program { get; set; }
        public bool Active { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime DeleteDate { get; set; }
        public DateTime UpdateDate { get; set; }
    }
}
