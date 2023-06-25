using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleGrpcCrudService.Core.BLL.RecordContents
{
    public class StudentFilter
    {
        private static readonly Logger _nlog = LogManager.GetCurrentClassLogger();

        public string Webid { get; set; }

        private StudentRecordRequest _studentRecordRequest;

        public StudentRecordRequest studentRecordRequest
        {
            get { return _studentRecordRequest; }
            set 
            {
                Webid = _studentRecordRequest.WebId;
                _studentRecordRequest = value;
            }
        }

        public StudentFilter(string webid)
        {
            if (string.IsNullOrEmpty(webid))
                _nlog.Error("Student record created with an empty webId");

            Webid = webid;
            _studentRecordRequest = new StudentRecordRequest();
            _studentRecordRequest.WebId = webid;
        }

        public StudentFilter(StudentRecordRequest studentRecordRequest)
        {
            _studentRecordRequest = studentRecordRequest;
            Webid = studentRecordRequest.WebId;
        }

    }
}
