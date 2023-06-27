using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using NLog;
using SimpleGrpcCrudService.Core.BLL.RecordContents;
using SimpleGrpcCrudService.Core.DAL.Data;
using SimpleGrpcCrudService.Core.DAL.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace SimpleGrpcCrudService.Core.DAL.GAP.Adapters
{
    public class StudentAdapter
    {
        private static readonly Logger _nlog = LogManager.GetCurrentClassLogger();
        private DbContextOptions<StudentContext> _contextOptions;
        private readonly IConfiguration _config;

        public StudentAdapter(IConfiguration config)
        {
            _config = config;
            var optionsBuilder = new DbContextOptionsBuilder<StudentContext>();
            optionsBuilder.UseSqlServer(config.GetConnectionString("connectionString"));
            _contextOptions = optionsBuilder.Options;
        }

        #region InsertStudent
        internal void InsertStudent(StudentRecordRequest recordRequest)
        {
            try
            {
                using var dbContext = new StudentContext(_contextOptions);
                if (recordRequest != null)
                {
                    var rule = new Student()
                    {
                        WebId = recordRequest.WebId,
                        FirstName = recordRequest.FirstName,
                        LastName = recordRequest.LastName,
                        StudentSecurityNumber = recordRequest.StudentSecurityNumber,
                    };

                    dbContext.Students.Add(rule);
                }

                _ = dbContext.SaveChanges();
            }
            catch (Exception ex)
            {
                _nlog.Fatal(ex);
                throw;
            }
        }
        #endregion

        #region Update
        internal void UpdateStudent(StudentRecordRequest recordRequest)
        {
            try
            {
                using var dbContext = new StudentContext(_contextOptions);

                var studentRecord = from student in dbContext.Students
                                    where student.WebId == recordRequest.WebId
                                    select student;

                if (studentRecord != null )
                {
                    var record = studentRecord.Single();

                    record.FirstName = recordRequest.FirstName.Trim();
                    record.LastName = recordRequest.LastName.Trim();
                    record.StudentSecurityNumber = recordRequest.StudentSecurityNumber.Trim();

                    dbContext.Students.Update(record);
                }

                _ = dbContext.SaveChanges();
            }
            catch (Exception ex)
            {
                _nlog.Fatal(ex);
            }
        }
        #endregion

        #region GetStudentByWebId
        internal int GetStudentByWebId(string webid)
        {
            try
            {
                using var dbContext = new StudentContext(_contextOptions);
                var getStudentById = from student in dbContext.Students
                                     where student.WebId == webid
                                     select student;

                if (getStudentById.Count() > 0)
                    return getStudentById.FirstOrDefault().StudentId;
                else
                    return 0;
            }
            catch (Exception ex)
            {
                _nlog.Fatal(ex);
                return 0;
            }
        }
        #endregion

        #region SelectStudentPart
        internal bool SelectStudentPart(StudentRecordRequest recordRequest)
        {
            try
            {
                using var dbContext = new StudentContext(_contextOptions);

                var studentRecord = (from student in dbContext.Students
                                     where student.WebId == recordRequest.WebId
                                     select student).ToList();

                if (studentRecord.Count == 0)
                {
                    _nlog.Error($"No data found");
                    return false;
                }
                else if (studentRecord.Count == 1)
                {
                    var studentList = studentRecord[0];
                    recordRequest.FirstName = studentList.FirstName?.Trim() ?? "";
                    recordRequest.LastName = studentList.LastName?.Trim() ?? "";
                    recordRequest.StudentSecurityNumber = studentList.StudentSecurityNumber?.Trim() ?? "";

                    return true;
                }
                else
                {
                    _nlog.Error($"Webid {recordRequest.WebId} duplicate rule");
                    return false;
                }
            }
            catch (Exception ex)
            {
                _nlog.Fatal(ex);
                return false;
            }
        }
        #endregion

        #region CreateNewBllRule
        private static SimpleGrpcCrudService.Core.BLL.RecordContents.StudentRecordComparable CreateBNewBllStudent(Student student)
        {
            return new BLL.RecordContents.StudentRecordComparable()
            {
                StudentId = student.StudentId,
                WebId = student.WebId,
                FirstName = student.FirstName,
                LastName = student.LastName,
                StudentSecurityNumber = student.StudentSecurityNumber,
            };
        }
        #endregion

        #region GetAllStudentRecord
        internal List<StudentRecordComparable> GetAllStudentRecord()
        {
            var studentRecord = new List<StudentRecordComparable>();
            try
            {
                using var dbContext = new StudentContext(_contextOptions);
                var students = from student in dbContext.Students
                               orderby student.FirstName
                               select CreateBNewBllStudent(student);

                if (students != null)
                {
                    return students.ToList();
                }

            }
            catch (Exception ex)
            {
                _nlog.Fatal($"{ex.Message}");
            }

            return studentRecord;
        }
        #endregion

        #region DeleteStudent
        internal void DeleteStudent(int studentId)
        {
            try
            {
                using var dbContext = new StudentContext(_contextOptions);

                var deleteStudent = from student in dbContext.Students
                                    where student.StudentId == studentId
                                    select student;

                if (deleteStudent != null)
                {
                    dbContext.Students.RemoveRange(deleteStudent);
                }
            }
            catch (Exception ex)
            {
                _nlog.Fatal(ex);
                throw;
            }
        }
        #endregion
    }
}
