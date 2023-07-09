using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoBed.Base.Entities
{
    public class Role: AuditableEntity
    {
        public string RoleId { get; set; }
        public string UserId { get; set; }
        public string Name { get; set; }
    }
}
