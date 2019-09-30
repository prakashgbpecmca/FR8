using Frayte.Services.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;

namespace Frayte.Services.Business
{
    public class MoveRecordFromLiveToTestRepository
    {
        FrayteEntities dbcontext = new FrayteEntities();

        public bool MoveRecord()
        {
            try
            {
                List<string> _name = GetTableNames().Where(p => p.StartsWith("LogisticService")).ToList();
                foreach (string name in _name)
                {
                    dbcontext.MoveLiveDBTableIntoTestDBTable(name);
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public List<string> GetTableNames()
        {
            List<string> name = new List<string>();
            SqlConnection con = new SqlConnection("Data Source = 119.9.110.197; initial catalog = FrayteLogisticDirect; user id = sa; password = FrayteDB#15;");
            con.Open();
            DataTable dt = con.GetSchema("Tables");
            foreach (DataRow row in dt.Rows)
            {
                string tablename = (string)row[2];
                name.Add(tablename);
            }
            con.Close();
            return name;
        }
    }
}
