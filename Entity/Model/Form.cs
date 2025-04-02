using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.Model
{
    public class Form
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public string Cuestion { get; set; }
        public string Type_Cuestion { get; set; }
        public string Answer { get; set; }
        public bool Active { get; set; }
        public DateTime create_date { get; set; }
        public DateTime delete_date { get; set; }
        public DateTime update_date { get; set; }
    }
}