using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace DBSQLClient.Data.Servicio
{
    public interface ISQLClientService
    {
        
        public Task<DataTable> QueryAsyncAsDataTable(
            string sqlCommand,
            SqlParameter[]? parameters = default,
            CancellationToken ct = default,
            int TimeOut = 5000);


        public Task<DataSet> ExecuteAsyncAsDataSet(
                string sqlCommand, 
                SqlParameter[]? parameters = default, 
                CancellationToken ct = default,  
                int TimeOut = 5000);


        public Task<DataSet> ExecuteStoredProcedureAsync(
         string nameStoredProcedure,
         SqlParameter[]? parameters = default,
         CancellationToken ct = default,
         int TimeOut = 5000);

        public Task<T?> ExecuteStoredProcedureAsync<T>(
            string nameStoredProcedure,
            SqlParameter[]? parameters = default,
            CancellationToken ct = default,
            int TimeOut = 5000);
    }
}
