﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.Model
{
    public class UserSede
    {
        public int Id { get; set; }
        public string StatusProcedure { get; set; }
        public int UserId { get; set; }
        public int SedeId { get; set; }

        // Propiedades de navegación
        public Sede Sede { get; set; }
        public User User { get; set; }
        public ICollection<UserSede> UserSedes { get; set; }
        
    }
}
