using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using System.Text.Json.Serialization;

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

    /*
        CREATE OR ALTER   PROCEDURE [dbo].[sp_Get_User] 
								        @pId INT = NULL
        AS
        BEGIN
            -- Tabla 1: Usuario
            SELECT 1 as Id, 'erick' as Nombre, 'erahon@outlook.com' as Correo;
    
            -- Tabla 2: Roles del usuario
            SELECT 1 as RolId, 'dev' as Nombre
            UNION ALL
            SELECT 2 as RolId, 'admin' as Nombre;

        END     
     */
    public class UserRoles
    {
        public int RolId { get; set; }
        
        //[JsonPropertyName("Rol")]
        public string? Nombre { get; set; }
        public int UserId { get; set; }  // ⚠️ FK necesaria
    }

    public class UserModel
    {
        public int Id { get; set; }
        public string? Nombre { get; set; }
        public string? Correo { get; set; }
        public List<UserRoles>? Roles { get; set; }
    }


    #region Propiedades de Orden y Productos
    /*
     CREATE PROCEDURE sp_Get_Order
        @OrderId INT
    AS
    BEGIN
        -- Tabla 1: Orden
        SELECT Id, OrderDate, Total FROM Orders WHERE Id = @OrderId;
    
        -- Tabla 2: Productos de la orden
        SELECT 
            p.ProductId,
            p.ProductName,
            op.Price,
            op.Quantity,
            op.OrderId
        FROM OrderProducts op
        INNER JOIN Products p ON op.ProductId = p.Id
        WHERE op.OrderId = @OrderId;
    END
    -------------------------------------------------------------------------------

    var result = await service.ExecuteAsync("sp_Get_Order", SqlHelper.Params(("OrderId", 123)));
    var order = result.ToSingleWithChildren<Order, OrderProduct>("Products");

    Console.WriteLine($"Orden #{order.Id}");
    Console.WriteLine($"Fecha: {order.OrderDate}");
    Console.WriteLine($"Productos:");
    foreach (var product in order.Products)
    {
        Console.WriteLine($"  - {product.ProductName}: ${product.Price} x {product.Quantity}");
    }
    Console.WriteLine($"Total: ${order.Total}");
     */
    public class OrderProduct
    {
        public int ProductId { get; set; }
        public string? ProductName { get; set; }
        public decimal Price { get; set; } = decimal.Zero;
        public int Quantity { get; set; }
        public int OrderId { get; set; }  // FK
    }

    public class Order
    {
        public int Id { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal Total { get; set; }
        public List<OrderProduct>? Products { get; set; }
    }
    #endregion


}
