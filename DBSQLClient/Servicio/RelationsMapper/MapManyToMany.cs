using System;
using System.Collections.Generic;
using System.Text;
using DBSQLClient.Models;
using DBSQLClient.Servicio.Conexion;

namespace DBSQLClient.Servicio.RelationsMapper
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TLeft"></typeparam>
    /// <typeparam name="TRight"></typeparam>
    /// <typeparam name="TJoin"></typeparam>
    /// <param name="result"></param>
    /// <param name="leftCollectionProperty"></param>
    /// <param name="leftKey"></param>
    /// <param name="rightKey"></param>
    /// <param name="joinLeftKey"></param>
    /// <param name="joinRightKey"></param>
    /// <returns></returns>
    /// <example><c>var users = result.MapManyToMany<User, Role, UserRole>(leftCollectionProperty: "Roles",joinLeftKey: "UserId",joinRightKey: "RoleId")</c></example>
    public static List<TLeft> MapManyToMany<TLeft, TRight, TJoin>(
        this SqlQueryResult result,
        string leftCollectionProperty,
        string leftKey = "Id",
        string rightKey = "Id",
        string joinLeftKey = "LeftId",
        string joinRightKey = "RightId")
        where TLeft : new()
        where TRight : new()
        where TJoin : new()
    {
        var lefts = MapRows<TLeft>(result.AsDataTable(0));
        var rights = MapRows<TRight>(result.AsDataTable(1));
        var joins = MapRows<TJoin>(result.AsDataTable(2));

        var leftKeyProp = typeof(TLeft).GetProperty(leftKey);
        var rightKeyProp = typeof(TRight).GetProperty(rightKey);
        var joinLeftProp = typeof(TJoin).GetProperty(joinLeftKey);
        var joinRightProp = typeof(TJoin).GetProperty(joinRightKey);

        var rightDict = rights.ToDictionary(
            r => rightKeyProp!.GetValue(r)!
        );

        var leftNavProp = typeof(TLeft).GetProperty(leftCollectionProperty);

        foreach (var left in lefts)
        {
            var leftId = leftKeyProp!.GetValue(left);
            var related = joins
                .Where(j => Equals(joinLeftProp!.GetValue(j), leftId))
                .Select(j => rightDict[joinRightProp!.GetValue(j)!])
                .ToList();

            leftNavProp!.SetValue(left, related);
        }

        return lefts;
    }

}
