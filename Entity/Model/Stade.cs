﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.Model
{
    public class Stade
    {
        public int Id { get; set; }
        public bool Active { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime DeleteDate { get; set; }
        public DateTime UpdateDate { get; set; }
        public string Description { get; set; }
        public string TypeStade { get; set; }
    }
}
