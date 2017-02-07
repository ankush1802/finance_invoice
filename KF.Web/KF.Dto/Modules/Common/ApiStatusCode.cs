using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KF.Dto.Modules.Common
{
    public enum ApiStatusCode
    {
        Success = 1,
        Failure = 2,
        Unauthorised = 3,
        NotFound = 4,
        NoContent = 5,
        NullParameter = 6,
        IsExist = 7,
        NotExist = 8,
        AlreadyRegistered = 9
    }
}
