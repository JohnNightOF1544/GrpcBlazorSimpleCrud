using Grpc.Net.Client;
using NLog;
using System.Linq.Expressions;

namespace SimpleCrudFrontend.Services
{
    public class BackendService
    {
        GrpcChannel _channel;
        SimpleGrpcCrudService.StudentBackend.StudentBackendClient _client;

        private static readonly Logger _nlog = LogManager.GetCurrentClassLogger();
        private string _url;

        public BackendService(ILogger<BackendService> logger, IConfiguration configuration)
        {
            _url = configuration.GetValue<string>("ServiceData:BackendService:URL");
        }

        public void Connect()
        {
            try
            {
                var httpHandler = new HttpClientHandler();

                var httpClient = new HttpClient(httpHandler);

                _channel = GrpcChannel.ForAddress(_url, new GrpcChannelOptions
                {
                    HttpClient = httpClient,
                });

                _client = new SimpleGrpcCrudService.StudentBackend.StudentBackendClient(_channel);
            }
            catch (Exception ex)
            {
                _nlog.Fatal(ex);
                throw;
            }
        }

        #region SaveStudent
        public async Task<Google.Rpc.Status> SaveStudent(SimpleCrudFrontend.Models.Student studentRecord)
        {
            if (_client == null)
                Connect();

            SimpleGrpcCrudService.StudentRecordRequest grpcStudentRecord = ToGrpcFromat(studentRecord);

            Google.Rpc.Status result;
            try
            {
                result = await _client.SaveStudentAsync(grpcStudentRecord);
                return result;
            }
            catch (Exception ex)
            {
                _nlog.Error($"Save rule threw up: {ex.Message}, {ex}");
                return new Google.Rpc.Status { Code = 2, Message = ex.Message };
            }
        }
        #endregion

        #region GetStudentRecord
        public async Task<SimpleCrudFrontend.Models.Student> GetStudentRecord(Guid id)
        {
            if (_client == null)
                Connect();

            try
            {
                SimpleGrpcCrudService.StudentRecordRequest result;
                try
                {
                    result = await _client.GetStudentRecordAsync(GetFilterFor(id));
                }
                catch (Exception ex)
                {
                    _nlog.Error($"Get student record threw up: " + ex.Message);
                    return null;
                }
                if (result.Status.Code == 0)
                {
                    SimpleCrudFrontend.Models.Student student = ToFrontendFormat(result);
                    if (result.WebId != "")
                        return student;
                }
                _nlog.Error(result.Status.Message);
                return null;
            }
            catch (Exception ex)
            {
                _nlog.Fatal(ex);
                throw;
            }
        }
        #endregion

        #region GetAll
        public async Task<List<SimpleCrudFrontend.Models.Student>> GetAllStudent()
        {
            if (_client == null)
                Connect();

            var studentList = await _client.GetAllAsync(new SimpleGrpcCrudService.StudentEmpty());
            List<SimpleCrudFrontend.Models.Student> students = new List<Models.Student>();

            foreach (var studentRecord in studentList.Items)
            {
                students.Add(ToFrontendFormat(studentRecord));
            }
            return students;
        }
        #endregion

        #region DeleteRecord
        public async Task<Google.Rpc.Status> DeleteRecord(string id)
        {
            if (_client == null)
                Connect();
            Google.Rpc.Status result;
            try
            {
                result = await _client.DeleteStudentRecordAsync(GetFilterFor(id));
                return result;
            }
            catch (Exception ex)
            {
                _nlog.Error($"Delete Record threw up: {ex.Message}, {ex} ");
                return new Google.Rpc.Status { Code = 2, Message = ex.Message };
            }
        }
        #endregion

        #region GetFilterFor
        public SimpleGrpcCrudService.StudentRecordFilter GetFilterFor(SimpleCrudFrontend.Models.Student student)
        {
            return new SimpleGrpcCrudService.StudentRecordFilter()
            {
                WebId = student.WebId.ToString(),
            };
        }

        public SimpleGrpcCrudService.StudentRecordFilter GetFilterFor(Guid id)
        {
            return new SimpleGrpcCrudService.StudentRecordFilter()
            {
                WebId = id.ToString(),
            };
        }

        public SimpleGrpcCrudService.StudentRecordFilter GetFilterFor(string id)
        {
            return new SimpleGrpcCrudService.StudentRecordFilter()
            {
                WebId = id.ToString()
            };
        }
        #endregion

        #region ToGrpcFormat
        public SimpleGrpcCrudService.StudentRecordRequest ToGrpcFromat(SimpleCrudFrontend.Models.Student studentRecord)
        {
            var GrpcStudentRecord = new SimpleGrpcCrudService.StudentRecordRequest()
            {
                StudentId = studentRecord.StudentId,
                WebId = studentRecord.id.ToString(),
                FirstName = studentRecord.FirstName,
                LastName = studentRecord.LastName,
                StudentSecurityNumber = studentRecord.StudentSecurityNumber
            };
            return GrpcStudentRecord;
        }
        #endregion

        #region ToFrontendFormat
        public SimpleCrudFrontend.Models.Student ToFrontendFormat(SimpleGrpcCrudService.StudentRecordRequest studentRequest)
        {
            SimpleCrudFrontend.Models.Student frontendStudentRecord = new SimpleCrudFrontend.Models.Student()
            {
                StudentId = studentRequest.StudentId,
                WebId = studentRequest.WebId,
                FirstName = studentRequest.FirstName,
                LastName = studentRequest.LastName,
                StudentSecurityNumber = studentRequest.StudentSecurityNumber
            };
            try
            {
                frontendStudentRecord.id = Guid.Parse(studentRequest.WebId);
            }
            catch (Exception ex)
            {
                _nlog.Error(ex, "Received student record has an invalid ID");
                throw;
            }

            return frontendStudentRecord;
        }
        #endregion

    }
}
