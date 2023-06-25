using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleGrpcCrudService.Core.DAL.Models
{
    public partial class Student
    {
        public int StudentId { get; set; }
        public string WebId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string StudentSecurityNumber { get; set; }

    }
}
