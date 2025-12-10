using System;
using System.Collections.Generic;
using System.Text;

namespace DBSQLClient.Models
{
    public class Table1
    {
        public int Param1 { get; set; }
        public int Param11 { get; set; }
    }

    public class Table2
    {
        public DateTime Fecha { get; set; }
    }

    public class NewDataSet
    {
        public Table1? Table1 { get; set; }
        public Table2? Table2 { get; set; }
    }

    public class UserModel
    {
        public int Id { get; set; }
        public string? Nombre { get; set; }
        public string? Correo { get; set; }
    }

}
