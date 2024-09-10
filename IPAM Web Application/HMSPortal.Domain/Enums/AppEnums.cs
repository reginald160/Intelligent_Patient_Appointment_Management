using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMSPortal.Domain.Enums
{
    public enum Gender
    {
        Male, Female, Others
    }

    public enum AuditType
    {
        None,
        Create,
        Update,
        Delete
    }
}
