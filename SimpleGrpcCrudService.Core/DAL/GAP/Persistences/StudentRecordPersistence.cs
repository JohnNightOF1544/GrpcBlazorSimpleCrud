using Microsoft.Extensions.Configuration;
using NLog;
using SimpleGrpcCrudService.Core.DAL.GAP.Adapters;
using SimpleGrpcCrudService.Core.DAL.GAP.PersistenceInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleGrpcCrudService.Core.DAL.GAP.Persistences
{
    public class StudentRecordPersistence : IStudentRecordPersistence
    {
        private readonly IConfiguration _config;
        private static readonly Logger _nlog = LogManager.GetCurrentClassLogger();

        public StudentRecordPersistence(IConfiguration config)
        {
            _config = config;
        }

        #region Save
        public bool Save(BLL.RecordContents.StudentFilter studentRecord)
        {
            var createRecord = new StudentAdapter(_config);
            int studentId = createRecord.GetStudentByWebId(studentRecord.studentRecordRequest.WebId);

            if (studentId == 0)
            {
                createRecord.InsertStudent(studentRecord.studentRecordRequest);
            }
            else
            {
                createRecord.UpdateStudent(studentRecord.studentRecordRequest);
            }
            return true;
        }
        #endregion

        #region GetAllStudent
        public List<BLL.RecordContents.StudentRecordComparable> GetAllStudent()
        {
            var selectStudent = new StudentAdapter(_config).GetAllStudentRecord();

            if (selectStudent.Count() > 1)
            {
                return selectStudent;
            }
            throw new Exception("There is no value");
        }
        #endregion

        #region SelectById
        public bool SelectById(string webid, out BLL.RecordContents.StudentFilter studentRecord)
        {
            if (CreateStudentRecordByWebId(webid, out BLL.RecordContents.StudentFilter studentRecordLocal))
            {
                studentRecord = studentRecordLocal;
                return true;
            }
            else
            {
                studentRecord = new BLL.RecordContents.StudentFilter(webid);
                return false;
            }
        }
        #endregion

        #region CreateStudentRecordByWebId
        private bool CreateStudentRecordByWebId(string webid, out BLL.RecordContents.StudentFilter studentRecord)
        {
            studentRecord = new BLL.RecordContents.StudentFilter(webid);

            var studentAdapter = new StudentAdapter(_config);

            if (!studentAdapter.SelectStudentPart(studentRecord.studentRecordRequest))
                return false;

            return true;
        }
        #endregion

        #region Delete
        public bool Delete(string webId)
        {
            try
            {
                var studentAdapt = new StudentAdapter(_config);
                int studentId = studentAdapt.GetStudentByWebId(webId);

                if (studentId == 0)
                {
                    _nlog.Fatal($"{webId} Student not found for delete");
                    return false;
                }

                if (studentId != 0)
                {
                    studentAdapt.DeleteStudent(studentId);
                    return true;
                }

                return true;
            }
            catch (Exception ex)
            {
                _nlog.Fatal($"{webId} {ex.Message}");
                return false;
            }
        }
        #endregion
    }
}
