using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.Model
{
    public class ChangeLog
    {

        public int Id { get; set; }
        public string Name { get; set; }
        public string DeleteAT {  get; set; }
        public string CreateAT { get; set; }
        public int IdTable { get; set; }
        public int IdUser { get; set; }
        public int IdPermission {get; set; }
        public string Action { get ; set; }

    }
}
