using SimpleGrpcCrudService.Core.BLL.RecordContents;

namespace SimpleGrpcCrudService.Core.BLL.RecordInterfaces
{
    public interface IStudentRecordRepository
    {
        void DeleteStudentRecord(string webId);
        List<StudentRecordComparable> GetAllRecord();
        bool GetRecord(string webid, out StudentFilter studentRecord);
        void SaveRuleRecord(StudentFilter studentRecord);
    }
}