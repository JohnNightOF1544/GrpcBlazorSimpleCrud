using NLog;
using SimpleGrpcCrudService.Core.BLL.RecordContents;
using SimpleGrpcCrudService.Core.BLL.RecordInterfaces;
using SimpleGrpcCrudService.Core.DAL.GAP.PersistenceInterfaces;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleGrpcCrudService.Core.BLL.RecordRepository
{
    public class StudentRecordRepository : IStudentRecordRepository
    {
        private static readonly Logger _nlog = LogManager.GetCurrentClassLogger();
        private readonly IStudentRecordPersistence _studentRecordPersistence;
        private ConcurrentDictionary<string, RecordContents.StudentFilter> _studentRecordCache = new ConcurrentDictionary<string, RecordContents.StudentFilter>();

        public StudentRecordRepository(IStudentRecordPersistence studentRecordPersistence)
        {
            _studentRecordPersistence = studentRecordPersistence;
        }

        #region SaveRuleRecord
        public void SaveRuleRecord(RecordContents.StudentFilter studentRecord)
        {
            if (_studentRecordCache.ContainsKey(studentRecord.Webid))
            {
                _studentRecordCache.TryRemove(studentRecord.Webid, out RecordContents.StudentFilter studentFilter);
                if (!_studentRecordCache.TryAdd(studentRecord.Webid, studentFilter))
                    throw new Exception($"Rule Record couldn't add to the rules: {studentRecord.Webid}");
                _nlog.Trace("Webid {0} Update is Cache", studentRecord.Webid);
            }
        }
        #endregion

        #region GetAllRecord
        public List<StudentRecordComparable> GetAllRecord()
        {
            var studentList = _studentRecordPersistence.GetAllStudent();

            _nlog.Trace("Rule are order by name");

            return studentList;
        }
        #endregion

        #region GetRecord
        public bool GetRecord(string webid, out RecordContents.StudentFilter studentRecord)
        {
            if (webid == null)
            {
                _nlog.Fatal("Attempting to find a Student record with a webid == null in the repo.");
                studentRecord = new RecordContents.StudentFilter(Guid.NewGuid().ToString());
            }

            if (_studentRecordCache.TryGetValue(webid, out RecordContents.StudentFilter studentFromCache))
            {
                studentRecord = studentFromCache;
                return true;
            }

            if (_studentRecordPersistence.SelectById(webid, out RecordContents.StudentFilter studentFromDB))
            {
                _studentRecordCache.TryAdd(webid, studentFromDB);
                studentRecord = studentFromDB;
                return true;
            }

            studentRecord = new RecordContents.StudentFilter(webid);
            return false;
        }
        #endregion

        #region DeleteStudentRecord
        public void DeleteStudentRecord(string webId)
        {
            _studentRecordCache.TryRemove(webId, out RecordContents.StudentFilter removedStudent);
            _studentRecordPersistence.Delete(webId);
        }
        #endregion

    }
}
