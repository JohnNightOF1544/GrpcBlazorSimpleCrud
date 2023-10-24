using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleGrpcCrudService.Core.BLL.RecordContents
{
    public class StudentRecordComparable : IComparable<StudentRecordComparable>
    {
        public int StudentId { get; set; }
        public string WebId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string StudentSecurityNumber { get; set; }
        public DateTime TimeIn { get; set; }

        public int CompareTo([AllowNull]StudentRecordComparable other)
        {
            if (other == null)
                return 1;

            return FirstName.CompareTo(other.FirstName);
        }

        public override bool Equals(object obj)
        {
            var other = obj as StudentRecordComparable;

            if (other == null) return false;

            return this.WebId == other.WebId;
        }
    }
}
