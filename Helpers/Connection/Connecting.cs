using MySklad.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MySklad.Helpers.Connecting
{
    static class Connecting
    {
        public static MySkladEntities conn { get; set; } = new MySkladEntities();
    }
}
