﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.DTOautogestion
{
    public class FormDTOAuto
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public string Question { get; set; }
        public string TypeQuestion { get; set; }
        public string Answer { get; set; }
        public bool Active { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime UpdateDate { get; set; }
        public DateTime DeleteDate { get; set; }
    }
}
