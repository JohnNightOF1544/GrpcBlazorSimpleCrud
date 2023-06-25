using SimpleGrpcCrudService.Core.BLL.RecordContents;

namespace SimpleGrpcCrudService.Core.DAL.GAP.PersistenceInterfaces
{
    public interface IStudentRecordPersistence
    {
        bool Delete(string webId);
        List<StudentRecordComparable> GetAllStudent();
        bool Save(StudentFilter studentRecord);
        bool SelectById(string webid, out StudentFilter studentRecord);
    }
}