using System;
using System.Collections.Generic;
using System.Text;
using DBSQLClient.Servicio.Conexion;

namespace DBSQLClient.Servicio.RelationsMapper
{
    /// <summary>
    /// Maps a one-to-many relationship from the SqlQueryResult to a list of child objects,
    /// </summary>
    /// <typeparam name="TChild"></typeparam>
    /// <typeparam name="TParent"></typeparam>
    /// <param name="result"></param>
    /// <param name="parentPropertyName"></param>
    /// <param name="parentKey"></param>
    /// <param name="childForeignKey"></param>
    /// <returns></returns>
    /// <example><code>var orders = result.MapManyToOne<Order, Customer>("Customer")</code></example>
    public static List<TChild> MapManyToOne<TChild, TParent>(
        this SqlQueryResult result,
        string parentPropertyName,
        string parentKey = "Id",
        string childForeignKey = "ParentId")
        where TChild : new()
        where TParent : new()
    {
        var childTable = result.AsDataTable(0);
        var parentTable = result.AsDataTable(1);

        var children = MapRows<TChild>(childTable);
        var parents = MapRows<TParent>(parentTable);

        var parentKeyProp = typeof(TParent).GetProperty(parentKey);
        var childFkProp = typeof(TChild).GetProperty(childForeignKey);
        var navProp = typeof(TChild).GetProperty(parentPropertyName);

        var parentDict = parents.ToDictionary(
            p => parentKeyProp!.GetValue(p)!
        );

        foreach (var child in children)
        {
            var fk = childFkProp!.GetValue(child);
            if (fk != null && parentDict.TryGetValue(fk, out var parent))
            {
                navProp!.SetValue(child, parent);
            }
        }

        return children;
    }

}
