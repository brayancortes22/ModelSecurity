using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.DTOautogestion
{
    public class AprendizProcessInstructorDTOAuto
    {
        public int Id { get; set; }
        public int TypeModalityId { get; set; }
        public int RegistrySofiaId { get; set; }
        public int ConceptId { get; set; }
        public int EnterpriseId { get; set; }
        public int ProcessId { get; set; }
        public int AprendizId { get; set; }
        public int InstructorId { get; set; }
        public int StadeId { get; set; }
        public int VerificationId { get; set; }
        public bool Active { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime DeleteDate { get; set; }
        public DateTime UpdateDate { get; set; }
    }
} 