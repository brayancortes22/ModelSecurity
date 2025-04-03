using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.DTOautogestion
{
    public class FormModuleDTOAuto
    {
        public int Id { get; set; }
        public string StatusProcedure { get; set; }
        public int FormId { get; set; }
        public int ModuleId { get; set; }
        public bool Active { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime DeleteDate { get; set; }
        public DateTime UpdateDate { get; set; }
    }
} 