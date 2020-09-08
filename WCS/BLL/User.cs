using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bll
{
    public class User
    {
        public static User CurrUser;

        string _id;
        string _groupId;
        string _name;
        string _lifeTime;
        string _description;
        string _createTime;

        List<string> _privilegeList;

        public User(DataSet ds)
        {
            _name = ds.Tables[0].Rows[0]["name"].ToString();
            _privilegeList = new List<string>();
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                _privilegeList.Add(dr["privilege_id"].ToString());
            }
        }
    }
}
