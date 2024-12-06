using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace movieRental
{
    public class ReportThreeContainer
    {
        public int id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int orders { get; set; }

        public string fullName => $"{FirstName} {LastName}";
    }
}
