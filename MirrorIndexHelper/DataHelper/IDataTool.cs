using System;
using System.Collections.Generic;
using System.Text;

namespace DataHelper
{
    public interface IDataTool
    {
        string ConnectionString
        {
            get;
            set;
        }
        System.Data.IDataReader DoReader(string sql);
        void Dispose();
        string Scaler(string sql);
        int Delete(string sql);
        int Update(string sql);
        int Insert(string sql);
        List<string> GetTableList();
        List<string> GetViewList();
        List<string> GetFieldNameList(string tablename);
        System.Data.DataTable Query(string sql);
    }
}
