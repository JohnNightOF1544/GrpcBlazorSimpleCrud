using Grpc.Core;
using NLog;
using SimpleGrpcCrudService.Core.BLL.RecordInterfaces;
using System.Reflection;

namespace SimpleGrpcCrudService.Services
{
    public class StudentService : StudentBackend.StudentBackendBase
    {
        private static readonly Logger _nlog = LogManager.GetCurrentClassLogger();
        private readonly IStudentRecordRepository _studentRecordRepository;

        public StudentService(IStudentRecordRepository studentRecordRepository)
        {
            _studentRecordRepository = studentRecordRepository;

            _nlog.Info("{0} started", Assembly.GetExecutingAssembly().GetName().Version);
        }

        #region SaveRule
        public override Task<Google.Rpc.Status> SaveStudent(StudentRecordRequest request, ServerCallContext context)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request.WebId))
                    return Task.FromResult(new Google.Rpc.Status { Code = 3, Message = "WebID is missed" });

                var studentRecord = new Core.BLL.RecordContents.StudentFilter(request);

                _studentRecordRepository.SaveRuleRecord(studentRecord);

                return Task.FromResult(new Google.Rpc.Status { Code = 0 });
            }
            catch (Exception ex)
            {
                _nlog.Fatal(ex);
                return Task.FromResult(new Google.Rpc.Status { Code = 2, Message = ex.Message });
            }
        }
        #endregion

        #region GetAll
        public override Task<StudentArray> GetAll(StudentEmpty request, ServerCallContext context)
        {
            try
            {
                var record = new StudentArray();
                record.Status = new Google.Rpc.Status { Code = 0, Message = "Rule is queryable." };

                var list = _studentRecordRepository.GetAllRecord();

                foreach ( var item in list)
                {
                    record.Items.Add(new StudentRecordRequest
                    {
                        StudentId = item.StudentId,
                        WebId = item.WebId,
                        FirstName = item.FirstName,
                        LastName = item.LastName,
                        StudentSecurityNumber = item.StudentSecurityNumber
                    });
                }
                return Task.FromResult(record);
            }
            catch (Exception ex)
            {
                _nlog.Fatal(ex);
                return Task.FromResult(new StudentArray { Status = new Google.Rpc.Status { Code = 2, Message = ex.Message } });
            }
        }
        #endregion

        #region GetStudentRecord
        public override Task<StudentRecordRequest> GetStudentRecord(StudentRecordFilter request, ServerCallContext context)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request.WebId))
                    return Task.FromResult(new StudentRecordRequest { Status = new Google.Rpc.Status { Code = 3, Message = "webid missed" } });

                if (_studentRecordRepository.GetRecord(request.WebId, out Core.BLL.RecordContents.StudentFilter studentRecord))
                {
                    var studentList = studentRecord.studentRecordRequest;
                    studentList.Status = new Google.Rpc.Status() { Code = 0, Message = "Student record Found" };
                    return Task.FromResult(studentList);
                }
                else
                {
                    var studentList = new StudentRecordRequest();
                    studentList.Status = new Google.Rpc.Status { Code = 5, Message = "Rule record not found" };
                    return Task.FromResult(studentList);
                }
            }
            catch (Exception ex)
            {
                _nlog.Fatal(ex);
                return Task.FromResult(new StudentRecordRequest { Status = new Google.Rpc.Status { Code = 2, Message = ex.Message } });
            }
        }
        #endregion

        #region DeleteStudentRecord
        public override Task<Google.Rpc.Status> DeleteStudentRecord(StudentRecordFilter request, ServerCallContext context)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request.WebId))
                    return Task.FromResult(new Google.Rpc.Status { Code = 3, Message = "webid missed" });

                _studentRecordRepository.DeleteStudentRecord(request.WebId);
                return Task.FromResult(new Google.Rpc.Status { Code = 0 });
            }
            catch (Exception ex)
            {
                _nlog.Fatal(ex);
                return Task.FromResult(new Google.Rpc.Status { Code = 2, Message = ex.Message });
            }
        }
        #endregion
    }
}
