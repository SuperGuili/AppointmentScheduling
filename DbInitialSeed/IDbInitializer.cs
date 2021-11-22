using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AppointmentScheduling.DbInitialSeed
{
    public interface IDbInitializer
    {
        void InitializeDb();
    }
}
