using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.DTOautogestion
{
    public class EnterpriseDTOAuto
    {
        public int Id { get; set; }
        public string Observation { get; set; }
        public string BossName { get; set; }
        public string EnterpriseName { get; set; }
        public string EnterprisePhone { get; set; }
        public string Location { get; set; }
        public string BossEmail { get; set; }
        public string EnterpriseNit { get; set; }
        public string EnterpriseEmail { get; set; }
        public bool Active { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime DeleteDate { get; set; }
        public DateTime UpdateDate { get; set; }
    }
}
