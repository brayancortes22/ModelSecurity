﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.Model
{
    public class AprendizProcessInstructor
    {
        public int Id { get; set; }
        public int TypeModalityId { get; set; }
        public TypeModality TypeModality { get; set; }
        public int RegisterySofiaId { get; set; }
        public RegisterySofia RegisterySofia { get; set; }
        public int ConceptId { get; set; }
        public Concept Concept { get; set; }
        public int EnterpriseId { get; set; }
        public Enterprise Enterprise { get; set; }
        public int ProcessId { get; set; }
        public Process Process { get; set; }
        public int AprendizId { get; set; }
        public Aprendiz Aprendiz { get; set; }
        public int InstructorId { get; set; }
        public Instructor Instructor { get; set; }
        public int StateId { get; set; }
        public State State { get; set; }
        public int VerificationId { get; set; }
        public Verification Verification { get; set; }  

        // Propiedades de navegación - Eliminando colecciones incorrectas
        // public ICollection<AprendizProcessInstructor> AprendizProcessInstructors { get; set; } // Auto-referencia eliminada
        // public ICollection<AprendizProgram> AprendizPrograms { get; set; } // Relación incorrecta eliminada
    }
}
