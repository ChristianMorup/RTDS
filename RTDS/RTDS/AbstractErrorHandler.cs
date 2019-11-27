using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RTDS
{
    public abstract class AbstractErrorHandler
    {
        public abstract void OnFatalError(string errorMessage);

        public abstract void OnWarning(string warningMessage);

    }
}
