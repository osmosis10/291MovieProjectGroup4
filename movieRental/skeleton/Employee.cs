﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace movieRental
{
    public class Employee
    {
        public int id { get; set; }
        public string socialSecurityNum { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public DateTime startDate { get; set; }

        // Computed property
        public string fullName => $"{firstName} {lastName}";

        public override string ToString()
        {
            return fullName;
        }
    }
}
