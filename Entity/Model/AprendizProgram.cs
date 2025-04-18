﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.Model
{
    public class AprendizProgram
    {
        public int Id { get; set; }
        public int AprendizId { get; set; }
        public int ProgramId { get; set; }

        public bool Active { get; set; }

        // Propiedades de navegación
        public Program Program { get; set; }
        public Aprendiz Aprendiz { get; set; }
        
    }
}
