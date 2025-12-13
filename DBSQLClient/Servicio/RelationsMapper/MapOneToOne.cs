using System;
using System.Collections.Generic;
using System.Text;
using DBSQLClient.Servicio.Conexion;

namespace DBSQLClient.Servicio.RelationsMapper
{
    public static TParent MapOneToOne<TParent, TChild>(
    this SqlQueryResult result,
    string childPropertyName)
    where TParent : new()
    where TChild : new()
    {
        var parentTable = result.AsDataTable(0);
        var childTable = result.AsDataTable(1);

        if (parentTable.Rows.Count == 0)
            throw new InvalidOperationException("No se encontró el registro padre.");

        var parent = MapSingleRow<TParent>(parentTable.Rows[0]);

        if (childTable.Rows.Count > 0)
        {
            var child = MapSingleRow<TChild>(childTable.Rows[0]);
            var prop = typeof(TParent).GetProperty(childPropertyName)
                ?? throw new ArgumentException($"Propiedad '{childPropertyName}' no encontrada");

            prop.SetValue(parent, child);
        }

        return parent;
    }

}
