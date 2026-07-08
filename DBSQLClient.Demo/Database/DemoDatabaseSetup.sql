-- ============================================================================
-- DBSQLClient.Demo - Script de base de datos (dominio: tienda en línea)
-- ============================================================================
-- Crea (o reconstruye) la base de datos DBSQLClientDemo con las tablas,
-- procedimientos almacenados y datos de ejemplo que usa cada Example*.cs de
-- este proyecto.
--
-- Se ejecuta automáticamente al arrancar el demo (ver
-- Database/DemoDatabaseBootstrapper.cs), pero también se puede correr a mano
-- contra cualquier instancia de SQL Server / LocalDB (por ejemplo con sqlcmd
-- o SSMS) si prefieres controlarlo tú mismo.
--
-- ADVERTENCIA: este script hace DROP + CREATE de todas las tablas y
-- procedimientos en cada ejecución, dejando siempre el mismo estado de datos
-- limpio y predecible para practicar. Es intencional para un demo; NO uses
-- este patrón contra una base de datos real.
-- ============================================================================

IF DB_ID(N'DBSQLClientDemo') IS NULL
BEGIN
    CREATE DATABASE DBSQLClientDemo;
END
GO

USE DBSQLClientDemo;
GO

-- Procedimientos: se recrean siempre para reflejar el contenido de este script.
-- Los siguientes son de una versión anterior de este demo (dominio genérico User/Profile/Role)
-- y ya no los crea este script; se limpian igual por si existen de una corrida vieja.
IF OBJECT_ID('dbo.sp_GetUserById', 'P') IS NOT NULL DROP PROCEDURE dbo.sp_GetUserById;
IF OBJECT_ID('dbo.sp_User_With_Profile', 'P') IS NOT NULL DROP PROCEDURE dbo.sp_User_With_Profile;
IF OBJECT_ID('dbo.sp_User_With_Orders', 'P') IS NOT NULL DROP PROCEDURE dbo.sp_User_With_Orders;
IF OBJECT_ID('dbo.sp_Users_With_Roles', 'P') IS NOT NULL DROP PROCEDURE dbo.sp_Users_With_Roles;
IF OBJECT_ID('dbo.sp_AllUsers_With_Orders', 'P') IS NOT NULL DROP PROCEDURE dbo.sp_AllUsers_With_Orders;
IF OBJECT_ID('dbo.sp_GetUserOrderTotal', 'P') IS NOT NULL DROP PROCEDURE dbo.sp_GetUserOrderTotal;
IF OBJECT_ID('dbo.sp_CreateUser', 'P') IS NOT NULL DROP PROCEDURE dbo.sp_CreateUser;

IF OBJECT_ID('dbo.sp_GetProductById', 'P') IS NOT NULL DROP PROCEDURE dbo.sp_GetProductById;
IF OBJECT_ID('dbo.sp_GetProductByPublicId', 'P') IS NOT NULL DROP PROCEDURE dbo.sp_GetProductByPublicId;
IF OBJECT_ID('dbo.sp_GetCartTotal', 'P') IS NOT NULL DROP PROCEDURE dbo.sp_GetCartTotal;
IF OBJECT_ID('dbo.sp_Order_With_Invoice', 'P') IS NOT NULL DROP PROCEDURE dbo.sp_Order_With_Invoice;
IF OBJECT_ID('dbo.sp_Invoice_With_Items', 'P') IS NOT NULL DROP PROCEDURE dbo.sp_Invoice_With_Items;
IF OBJECT_ID('dbo.sp_Variant_With_Attributes', 'P') IS NOT NULL DROP PROCEDURE dbo.sp_Variant_With_Attributes;
IF OBJECT_ID('dbo.sp_Order_With_Items', 'P') IS NOT NULL DROP PROCEDURE dbo.sp_Order_With_Items;
IF OBJECT_ID('dbo.sp_Orders_With_User', 'P') IS NOT NULL DROP PROCEDURE dbo.sp_Orders_With_User;
IF OBJECT_ID('dbo.sp_Orders_With_Guest', 'P') IS NOT NULL DROP PROCEDURE dbo.sp_Orders_With_Guest;
IF OBJECT_ID('dbo.sp_Products_With_Categories', 'P') IS NOT NULL DROP PROCEDURE dbo.sp_Products_With_Categories;
IF OBJECT_ID('dbo.sp_AllCarts_With_Items', 'P') IS NOT NULL DROP PROCEDURE dbo.sp_AllCarts_With_Items;
IF OBJECT_ID('dbo.sp_AddCartItem', 'P') IS NOT NULL DROP PROCEDURE dbo.sp_AddCartItem;
IF OBJECT_ID('dbo.sp_GetVariantEffectivePrice', 'P') IS NOT NULL DROP PROCEDURE dbo.sp_GetVariantEffectivePrice;
IF OBJECT_ID('dbo.sp_Combo_With_Items', 'P') IS NOT NULL DROP PROCEDURE dbo.sp_Combo_With_Items;
IF OBJECT_ID('dbo.sp_Variant_With_Movements', 'P') IS NOT NULL DROP PROCEDURE dbo.sp_Variant_With_Movements;
IF OBJECT_ID('dbo.sp_GetVariantAvailableStock', 'P') IS NOT NULL DROP PROCEDURE dbo.sp_GetVariantAvailableStock;
GO

-- Tablas: se recrean siempre (orden por dependencias: hijas antes que padres).
-- Las tres siguientes son de una versión anterior de este demo (dominio genérico
-- User/Profile/Role) y ya no las crea este script; se limpian igual por si existen de una
-- corrida vieja, para no bloquear el DROP de Users con una FK huérfana.
IF OBJECT_ID('dbo.UserRole', 'U') IS NOT NULL DROP TABLE dbo.UserRole;
IF OBJECT_ID('dbo.UserProfile', 'U') IS NOT NULL DROP TABLE dbo.UserProfile;
IF OBJECT_ID('dbo.UserProfiles', 'U') IS NOT NULL DROP TABLE dbo.UserProfiles;
IF OBJECT_ID('dbo.Roles', 'U') IS NOT NULL DROP TABLE dbo.Roles;

IF OBJECT_ID('dbo.InventoryMovements', 'U') IS NOT NULL DROP TABLE dbo.InventoryMovements;
IF OBJECT_ID('dbo.OrderItems', 'U') IS NOT NULL DROP TABLE dbo.OrderItems;
IF OBJECT_ID('dbo.InvoiceItems', 'U') IS NOT NULL DROP TABLE dbo.InvoiceItems;
IF OBJECT_ID('dbo.Invoices', 'U') IS NOT NULL DROP TABLE dbo.Invoices;
IF OBJECT_ID('dbo.CartItems', 'U') IS NOT NULL DROP TABLE dbo.CartItems;
IF OBJECT_ID('dbo.ComboItems', 'U') IS NOT NULL DROP TABLE dbo.ComboItems;
IF OBJECT_ID('dbo.Combos', 'U') IS NOT NULL DROP TABLE dbo.Combos;
IF OBJECT_ID('dbo.DiscountVariants', 'U') IS NOT NULL DROP TABLE dbo.DiscountVariants;
IF OBJECT_ID('dbo.DiscountCategories', 'U') IS NOT NULL DROP TABLE dbo.DiscountCategories;
IF OBJECT_ID('dbo.Discounts', 'U') IS NOT NULL DROP TABLE dbo.Discounts;
IF OBJECT_ID('dbo.ProductVariantAttributes', 'U') IS NOT NULL DROP TABLE dbo.ProductVariantAttributes;
IF OBJECT_ID('dbo.ProductCategories', 'U') IS NOT NULL DROP TABLE dbo.ProductCategories;
IF OBJECT_ID('dbo.Carts', 'U') IS NOT NULL DROP TABLE dbo.Carts;
IF OBJECT_ID('dbo.Orders', 'U') IS NOT NULL DROP TABLE dbo.Orders;
IF OBJECT_ID('dbo.ProductVariants', 'U') IS NOT NULL DROP TABLE dbo.ProductVariants;
IF OBJECT_ID('dbo.Products', 'U') IS NOT NULL DROP TABLE dbo.Products;
IF OBJECT_ID('dbo.Categories', 'U') IS NOT NULL DROP TABLE dbo.Categories;
IF OBJECT_ID('dbo.GuestCheckouts', 'U') IS NOT NULL DROP TABLE dbo.GuestCheckouts;
IF OBJECT_ID('dbo.Users', 'U') IS NOT NULL DROP TABLE dbo.Users;
GO

-- Todas las tablas con clave propia siguen el mismo patrón: el INT IDENTITY es la PK/FK
-- interna (compacta, eficiente para joins e índices), y public_id (UNIQUEIDENTIFIER, con
-- DEFAULT NEWID() y UNIQUE) es lo único que expondría una API, para que un cliente no pueda
-- enumerar recursos incrementando un entero (ej: /products/1, /products/2...) ni adivinar ids
-- de otro recurso. La excepción es ProductCategories: es una tabla de unión pura, identificada
-- por el par (product_id, category_id), sin id propio al que darle un equivalente público.

CREATE TABLE dbo.Users (
    user_id         INT IDENTITY PRIMARY KEY,
    public_id       UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(),
    full_name       NVARCHAR(100) NOT NULL,
    email           NVARCHAR(150) NOT NULL,
    password_hash   NVARCHAR(200) NULL,
    CONSTRAINT UQ_Users_PublicId UNIQUE (public_id)
);

-- Checkout como invitado: sin password, se crea al vuelo al momento de ordenar.
CREATE TABLE dbo.GuestCheckouts (
    guest_id        INT IDENTITY PRIMARY KEY,
    public_id       UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(),
    full_name       NVARCHAR(100) NOT NULL,
    email           NVARCHAR(150) NOT NULL,
    CONSTRAINT UQ_GuestCheckouts_PublicId UNIQUE (public_id)
);

CREATE TABLE dbo.Categories (
    category_id     INT IDENTITY PRIMARY KEY,
    public_id       UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(),
    category_name   NVARCHAR(50) NOT NULL,
    CONSTRAINT UQ_Categories_PublicId UNIQUE (public_id)
);

CREATE TABLE dbo.Products (
    product_id      INT IDENTITY PRIMARY KEY,
    public_id       UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(),
    product_name    NVARCHAR(150) NOT NULL,
    description     NVARCHAR(500) NULL,
    CONSTRAINT UQ_Products_PublicId UNIQUE (public_id)
);

-- Unión muchos a muchos: un producto puede estar en varias categorías y viceversa.
CREATE TABLE dbo.ProductCategories (
    product_id      INT NOT NULL,
    category_id     INT NOT NULL,
    CONSTRAINT PK_ProductCategories PRIMARY KEY (product_id, category_id),
    CONSTRAINT FK_ProductCategories_Product FOREIGN KEY (product_id) REFERENCES dbo.Products(product_id),
    CONSTRAINT FK_ProductCategories_Category FOREIGN KEY (category_id) REFERENCES dbo.Categories(category_id)
);

-- Cada SKU vendible es una variante; el precio y el stock viven aquí, no en Products,
-- porque varían por variante (ej: distinto precio por talla).
CREATE TABLE dbo.ProductVariants (
    variant_id      INT IDENTITY PRIMARY KEY,
    public_id       UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(),
    product_id      INT NOT NULL,
    sku             NVARCHAR(50) NOT NULL,
    price           DECIMAL(10, 2) NOT NULL,
    stock_quantity  INT NOT NULL,
    CONSTRAINT FK_ProductVariants_Product FOREIGN KEY (product_id) REFERENCES dbo.Products(product_id),
    CONSTRAINT UQ_ProductVariants_PublicId UNIQUE (public_id)
);

-- Atributos de variante en formato EAV: cada variante tiene tantas filas como atributos
-- le apliquen (talla/color en ropa, talla/ancho en calzado, ninguna en electrónica).
CREATE TABLE dbo.ProductVariantAttributes (
    variant_attribute_id   INT IDENTITY PRIMARY KEY,
    public_id              UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(),
    variant_id             INT NOT NULL,
    attribute_name         NVARCHAR(50) NOT NULL,
    attribute_value        NVARCHAR(100) NOT NULL,
    CONSTRAINT FK_ProductVariantAttributes_Variant FOREIGN KEY (variant_id) REFERENCES dbo.ProductVariants(variant_id),
    CONSTRAINT UQ_ProductVariantAttributes_PublicId UNIQUE (public_id)
);

-- Precio y promociones separados del catálogo base: Discounts unifica promoción/descuento/oferta
-- (son la misma idea: una regla que reduce el precio por un tiempo), y puede apuntar a una
-- variante puntual (DiscountVariants) o a toda una categoría (DiscountCategories). Cuando una
-- variante cae bajo más de un descuento aplicable, gana el que dé mayor monto en moneda
-- (ver sp_GetVariantEffectivePrice) — no importa si es porcentaje o monto fijo.
CREATE TABLE dbo.Discounts (
    discount_id     INT IDENTITY PRIMARY KEY,
    public_id       UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(),
    name            NVARCHAR(100) NOT NULL,
    discount_type   NVARCHAR(20) NOT NULL, -- 'Percentage' | 'FixedAmount'
    value           DECIMAL(10, 2) NOT NULL,
    start_date      DATETIME NOT NULL,
    end_date        DATETIME NULL, -- NULL = sin vencimiento
    is_active       BIT NOT NULL DEFAULT 1,
    CONSTRAINT UQ_Discounts_PublicId UNIQUE (public_id),
    CONSTRAINT CK_Discounts_Type CHECK (discount_type IN ('Percentage', 'FixedAmount'))
);

-- Uniones puras (como ProductCategories): sin id ni public_id propios.
CREATE TABLE dbo.DiscountVariants (
    discount_id     INT NOT NULL,
    variant_id      INT NOT NULL,
    CONSTRAINT PK_DiscountVariants PRIMARY KEY (discount_id, variant_id),
    CONSTRAINT FK_DiscountVariants_Discount FOREIGN KEY (discount_id) REFERENCES dbo.Discounts(discount_id),
    CONSTRAINT FK_DiscountVariants_Variant FOREIGN KEY (variant_id) REFERENCES dbo.ProductVariants(variant_id)
);

CREATE TABLE dbo.DiscountCategories (
    discount_id     INT NOT NULL,
    category_id     INT NOT NULL,
    CONSTRAINT PK_DiscountCategories PRIMARY KEY (discount_id, category_id),
    CONSTRAINT FK_DiscountCategories_Discount FOREIGN KEY (discount_id) REFERENCES dbo.Discounts(discount_id),
    CONSTRAINT FK_DiscountCategories_Category FOREIGN KEY (category_id) REFERENCES dbo.Categories(category_id)
);

-- Bundle de variantes a un precio especial. Solo catálogo por ahora: no se puede agregar un
-- combo al carrito todavía (CartItems/OrderItems siguen referenciando variantes individuales).
CREATE TABLE dbo.Combos (
    combo_id        INT IDENTITY PRIMARY KEY,
    public_id       UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(),
    name            NVARCHAR(100) NOT NULL,
    combo_price     DECIMAL(10, 2) NOT NULL,
    start_date      DATETIME NOT NULL,
    end_date        DATETIME NULL,
    is_active       BIT NOT NULL DEFAULT 1,
    CONSTRAINT UQ_Combos_PublicId UNIQUE (public_id)
);

CREATE TABLE dbo.ComboItems (
    combo_item_id   INT IDENTITY PRIMARY KEY,
    public_id       UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(),
    combo_id        INT NOT NULL,
    variant_id      INT NOT NULL,
    quantity        INT NOT NULL,
    CONSTRAINT FK_ComboItems_Combo FOREIGN KEY (combo_id) REFERENCES dbo.Combos(combo_id),
    CONSTRAINT FK_ComboItems_Variant FOREIGN KEY (variant_id) REFERENCES dbo.ProductVariants(variant_id),
    CONSTRAINT UQ_ComboItems_PublicId UNIQUE (public_id)
);

CREATE TABLE dbo.Carts (
    cart_id         INT IDENTITY PRIMARY KEY,
    public_id       UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(),
    user_id         INT NOT NULL,
    CONSTRAINT FK_Carts_User FOREIGN KEY (user_id) REFERENCES dbo.Users(user_id),
    CONSTRAINT UQ_Carts_PublicId UNIQUE (public_id)
);

CREATE TABLE dbo.CartItems (
    cart_item_id    INT IDENTITY PRIMARY KEY,
    public_id       UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(),
    cart_id         INT NOT NULL,
    variant_id      INT NOT NULL,
    quantity        INT NOT NULL,
    CONSTRAINT FK_CartItems_Cart FOREIGN KEY (cart_id) REFERENCES dbo.Carts(cart_id),
    CONSTRAINT FK_CartItems_Variant FOREIGN KEY (variant_id) REFERENCES dbo.ProductVariants(variant_id),
    CONSTRAINT UQ_CartItems_PublicId UNIQUE (public_id)
);

-- Una orden pertenece a un usuario registrado O a un guest checkout, nunca ambos ni ninguno.
CREATE TABLE dbo.Orders (
    order_id        INT IDENTITY PRIMARY KEY,
    public_id       UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(),
    user_id         INT NULL,
    guest_id        INT NULL,
    order_date      DATETIME NOT NULL DEFAULT GETDATE(),
    status          NVARCHAR(20) NOT NULL,
    total           DECIMAL(10, 2) NOT NULL,
    CONSTRAINT FK_Orders_User FOREIGN KEY (user_id) REFERENCES dbo.Users(user_id),
    CONSTRAINT FK_Orders_Guest FOREIGN KEY (guest_id) REFERENCES dbo.GuestCheckouts(guest_id),
    CONSTRAINT CK_Orders_Owner CHECK (
        (user_id IS NOT NULL AND guest_id IS NULL) OR
        (user_id IS NULL AND guest_id IS NOT NULL)
    ),
    CONSTRAINT UQ_Orders_PublicId UNIQUE (public_id)
);

-- Historial de stock por variante: cada fila es un movimiento (con signo: positivo = entra,
-- negativo = sale). stock_quantity en ProductVariants es el saldo actual (más rápido de leer
-- que sumar todo el historial cada vez); esta tabla es la auditoría de cómo se llegó a ese saldo.
-- order_id es NULL salvo en movimientos 'Sale'/'Return', donde trazan a la orden que los originó.
CREATE TABLE dbo.InventoryMovements (
    movement_id     INT IDENTITY PRIMARY KEY,
    public_id       UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(),
    variant_id      INT NOT NULL,
    movement_type   NVARCHAR(20) NOT NULL, -- 'Purchase' | 'Sale' | 'Return' | 'Adjustment' | 'Damaged'
    quantity        INT NOT NULL,
    movement_date   DATETIME NOT NULL DEFAULT GETDATE(),
    order_id        INT NULL,
    notes           NVARCHAR(200) NULL,
    CONSTRAINT FK_InventoryMovements_Variant FOREIGN KEY (variant_id) REFERENCES dbo.ProductVariants(variant_id),
    CONSTRAINT FK_InventoryMovements_Order FOREIGN KEY (order_id) REFERENCES dbo.Orders(order_id),
    CONSTRAINT CK_InventoryMovements_Type CHECK (movement_type IN ('Purchase', 'Sale', 'Return', 'Adjustment', 'Damaged')),
    CONSTRAINT UQ_InventoryMovements_PublicId UNIQUE (public_id)
);

CREATE TABLE dbo.OrderItems (
    order_item_id   INT IDENTITY PRIMARY KEY,
    public_id       UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(),
    order_id        INT NOT NULL,
    variant_id      INT NOT NULL,
    quantity        INT NOT NULL,
    unit_price      DECIMAL(10, 2) NOT NULL,
    CONSTRAINT FK_OrderItems_Order FOREIGN KEY (order_id) REFERENCES dbo.Orders(order_id),
    CONSTRAINT FK_OrderItems_Variant FOREIGN KEY (variant_id) REFERENCES dbo.ProductVariants(variant_id),
    CONSTRAINT UQ_OrderItems_PublicId UNIQUE (public_id)
);

-- Relación 1 a 1 con Orders: no toda orden tiene factura todavía (ej: pendiente de pago).
CREATE TABLE dbo.Invoices (
    invoice_id      INT IDENTITY PRIMARY KEY,
    public_id       UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(),
    order_id        INT NOT NULL UNIQUE,
    invoice_number  NVARCHAR(30) NOT NULL,
    issued_date     DATETIME NOT NULL DEFAULT GETDATE(),
    total_amount    DECIMAL(10, 2) NOT NULL,
    CONSTRAINT FK_Invoices_Order FOREIGN KEY (order_id) REFERENCES dbo.Orders(order_id),
    CONSTRAINT UQ_Invoices_PublicId UNIQUE (public_id)
);

-- Foto fija de lo que se facturó: se llena copiando OrderItems al momento de EMITIR la factura,
-- no es una referencia viva a OrderItems. Si después cambiara el precio de la variante o la
-- cantidad de la orden, esta tabla no se entera -- una factura ya emitida no debería cambiar.
CREATE TABLE dbo.InvoiceItems (
    invoice_item_id INT IDENTITY PRIMARY KEY,
    public_id       UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(),
    invoice_id      INT NOT NULL,
    variant_id      INT NOT NULL,
    quantity        INT NOT NULL,
    unit_price      DECIMAL(10, 2) NOT NULL,
    CONSTRAINT FK_InvoiceItems_Invoice FOREIGN KEY (invoice_id) REFERENCES dbo.Invoices(invoice_id),
    CONSTRAINT FK_InvoiceItems_Variant FOREIGN KEY (variant_id) REFERENCES dbo.ProductVariants(variant_id),
    CONSTRAINT UQ_InvoiceItems_PublicId UNIQUE (public_id)
);
GO

-- ============================================================================
-- Datos de ejemplo
-- ============================================================================

INSERT INTO dbo.Users (full_name, email, password_hash) VALUES
    ('Erick', 'erick@test.com', 'hash-demo-1'),
    ('Ana', 'ana@test.com', 'hash-demo-2');

INSERT INTO dbo.GuestCheckouts (full_name, email) VALUES
    ('Invitado Demo', 'invitado@test.com');

INSERT INTO dbo.Categories (category_name) VALUES
    ('Ropa'), ('Calzado'), ('Electrónica'), ('Ofertas');

INSERT INTO dbo.Products (product_name, description) VALUES
    ('Camiseta Básica', 'Camiseta de algodón de corte regular'),
    ('Zapatilla Runner', 'Zapatilla deportiva para running'),
    ('Auriculares Bluetooth', 'Auriculares inalámbricos con cancelación de ruido');

-- Camiseta: Ropa + Ofertas. Zapatilla: solo Calzado. Auriculares: Electrónica + Ofertas.
INSERT INTO dbo.ProductCategories (product_id, category_id) VALUES
    (1, 1), (1, 4),
    (2, 2),
    (3, 3), (3, 4);

INSERT INTO dbo.ProductVariants (product_id, sku, price, stock_quantity) VALUES
    (1, 'CAM-M-ROJO', 19.99, 50),   -- variant_id 1
    (1, 'CAM-L-AZUL', 21.99, 30),   -- variant_id 2
    (2, 'ZAP-42-NORMAL', 59.99, 20), -- variant_id 3
    (3, 'AUD-BT-NEGRO', 39.99, 15);  -- variant_id 4, sin atributos

-- Ropa: Talla + Color. Calzado: Talla + Ancho. Auriculares (variant_id 4): ninguno a propósito.
INSERT INTO dbo.ProductVariantAttributes (variant_id, attribute_name, attribute_value) VALUES
    (1, 'Talla', 'M'), (1, 'Color', 'Rojo'),
    (2, 'Talla', 'L'), (2, 'Color', 'Azul'),
    (3, 'Talla', '42'), (3, 'Ancho', 'Normal');

-- Descuento por categoría: 15% a toda la Ropa (activo, sin vencimiento).
INSERT INTO dbo.Discounts (name, discount_type, value, start_date, end_date, is_active) VALUES
    ('Descuento Ropa 15%', 'Percentage', 15.00, DATEADD(DAY, -30, GETDATE()), NULL, 1),      -- discount_id 1
    ('Oferta Zapatilla $10', 'FixedAmount', 10.00, DATEADD(DAY, -10, GETDATE()), NULL, 1),   -- discount_id 2
    ('Promo Expirada 50%', 'Percentage', 50.00, DATEADD(DAY, -60, GETDATE()), DATEADD(DAY, -30, GETDATE()), 1); -- discount_id 3, vencida

INSERT INTO dbo.DiscountCategories (discount_id, category_id) VALUES
    (1, 1); -- 15% a toda la categoría Ropa

INSERT INTO dbo.DiscountVariants (discount_id, variant_id) VALUES
    (2, 3),  -- $10 fijos a la Zapatilla (variant_id 3)
    (3, 1);  -- 50% a la Camiseta M/Rojo, pero ya venció: no debe aplicar

-- Auriculares (variant_id 4) no tiene ningún descuento en ningún lado: precio efectivo = precio base.

-- Combo Verano: vigente. Camiseta M/Rojo + Zapatilla Runner por $65.00 (vs. $79.98 sumados).
-- Combo Invierno: ya venció (igual que la "Promo Expirada 50%") -- no debe listarse como vigente,
-- aunque is_active siga en 1, porque combos/promociones/descuentos son por tiempo.
INSERT INTO dbo.Combos (name, combo_price, start_date, end_date, is_active) VALUES
    ('Combo Verano', 65.00, DATEADD(DAY, -5, GETDATE()), NULL, 1),                                    -- combo_id 1
    ('Combo Invierno', 45.00, DATEADD(DAY, -60, GETDATE()), DATEADD(DAY, -30, GETDATE()), 1);         -- combo_id 2, vencido

INSERT INTO dbo.ComboItems (combo_id, variant_id, quantity) VALUES
    (1, 1, 1),
    (1, 3, 1),
    (2, 2, 1),
    (2, 4, 1);

-- Carrito de Erick con 2 ítems; carrito de Ana vacío a propósito.
INSERT INTO dbo.Carts (user_id) VALUES (1), (2);

INSERT INTO dbo.CartItems (cart_id, variant_id, quantity) VALUES
    (1, 1, 2),
    (1, 3, 1);

-- Orden de Erick y de Ana: pagadas, con factura. Orden del guest: pendiente, sin factura.
INSERT INTO dbo.Orders (user_id, guest_id, status, total) VALUES
    (1, NULL, 'Pagado', 39.98),
    (2, NULL, 'Pagado', 59.99),
    (NULL, 1, 'Pendiente', 39.99);

INSERT INTO dbo.OrderItems (order_id, variant_id, quantity, unit_price) VALUES
    (1, 1, 2, 19.99),
    (2, 3, 1, 59.99),
    (3, 4, 1, 39.99);

INSERT INTO dbo.Invoices (order_id, invoice_number, total_amount) VALUES
    (1, 'INV-0001', 39.98),   -- invoice_id 1
    (2, 'INV-0002', 59.99);   -- invoice_id 2

-- Copia congelada de los OrderItems correspondientes al momento de emitir cada factura (acá
-- coinciden con la orden porque en este demo no hay correcciones posteriores, pero son filas
-- independientes: si cambiara OrderItems después, esto no se entera).
INSERT INTO dbo.InvoiceItems (invoice_id, variant_id, quantity, unit_price) VALUES
    (1, 1, 2, 19.99),
    (2, 3, 1, 59.99);

-- Historial de movimientos: la suma de cada variante coincide con su stock_quantity actual.
-- Camiseta M/Rojo (variant_id 1): compra inicial + la venta de la orden 1 (2 unidades).
-- Zapatilla (variant_id 3): compra inicial + la venta de la orden 2 (1 unidad).
-- Auriculares (variant_id 4): compra inicial + una unidad dañada (no vendible).
-- Camiseta L/Azul (variant_id 2): sin ningún movimiento registrado todavía, a propósito.
INSERT INTO dbo.InventoryMovements (variant_id, movement_type, quantity, movement_date, order_id, notes) VALUES
    (1, 'Purchase', 52, DATEADD(DAY, -20, GETDATE()), NULL, 'Reposición inicial'),
    (1, 'Sale', -2, DATEADD(DAY, -1, GETDATE()), 1, NULL),
    (3, 'Purchase', 21, DATEADD(DAY, -15, GETDATE()), NULL, 'Reposición inicial'),
    (3, 'Sale', -1, DATEADD(DAY, -1, GETDATE()), 2, NULL),
    (4, 'Purchase', 20, DATEADD(DAY, -18, GETDATE()), NULL, 'Reposición inicial'),
    (4, 'Damaged', -5, DATEADD(DAY, -3, GETDATE()), NULL, 'Unidades dañadas en depósito');
GO

-- ============================================================================
-- Procedimientos almacenados
-- ============================================================================

CREATE PROCEDURE dbo.sp_GetProductById
    @ProductId INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT product_id, public_id, product_name, description
    FROM dbo.Products
    WHERE product_id = @ProductId;
END;
GO

-- Pensada para una API: recibe el GUID público en vez del id interno secuencial.
CREATE PROCEDURE dbo.sp_GetProductByPublicId
    @PublicId UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;
    SELECT product_id, public_id, product_name, description
    FROM dbo.Products
    WHERE public_id = @PublicId;
END;
GO

CREATE PROCEDURE dbo.sp_GetCartTotal
    @CartId INT,
    @Total DECIMAL(10, 2) OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT @Total = ISNULL(SUM(ci.quantity * pv.price), 0)
    FROM dbo.CartItems ci
    INNER JOIN dbo.ProductVariants pv ON pv.variant_id = ci.variant_id
    WHERE ci.cart_id = @CartId;
END;
GO

CREATE PROCEDURE dbo.sp_Order_With_Invoice
    @OrderId INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT order_id, public_id, order_date, status, total
    FROM dbo.Orders
    WHERE order_id = @OrderId;

    SELECT invoice_id, public_id, order_id, invoice_number, issued_date, total_amount
    FROM dbo.Invoices
    WHERE order_id = @OrderId;
END;
GO

-- Detalle de lo que se facturó: InvoiceItems es una foto fija propia (no un JOIN contra
-- OrderItems), traída por separado porque MapOneToOne solo resuelve 2 tablas por llamada.
CREATE PROCEDURE dbo.sp_Invoice_With_Items
    @InvoiceId INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT invoice_id, public_id, order_id, invoice_number, issued_date, total_amount
    FROM dbo.Invoices
    WHERE invoice_id = @InvoiceId;

    SELECT invoice_item_id, public_id, invoice_id, variant_id, quantity, unit_price
    FROM dbo.InvoiceItems
    WHERE invoice_id = @InvoiceId;
END;
GO

CREATE PROCEDURE dbo.sp_Variant_With_Attributes
    @VariantId INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT variant_id, public_id, product_id, sku, price, stock_quantity
    FROM dbo.ProductVariants
    WHERE variant_id = @VariantId;

    SELECT variant_attribute_id, public_id, variant_id, attribute_name, attribute_value
    FROM dbo.ProductVariantAttributes
    WHERE variant_id = @VariantId;
END;
GO

CREATE PROCEDURE dbo.sp_Order_With_Items
    @OrderId INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT order_id, public_id, order_date, status, total
    FROM dbo.Orders
    WHERE order_id = @OrderId;

    SELECT order_item_id, public_id, order_id, variant_id, quantity, unit_price
    FROM dbo.OrderItems
    WHERE order_id = @OrderId;
END;
GO

CREATE PROCEDURE dbo.sp_Orders_With_User
    @UserId INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT order_id, public_id, order_date, status, total
    FROM dbo.Orders
    WHERE user_id = @UserId;

    SELECT user_id, public_id, full_name, email, password_hash
    FROM dbo.Users
    WHERE user_id = @UserId;
END;
GO

CREATE PROCEDURE dbo.sp_Orders_With_Guest
    @GuestId INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT order_id, public_id, order_date, status, total
    FROM dbo.Orders
    WHERE guest_id = @GuestId;

    SELECT guest_id, public_id, full_name, email
    FROM dbo.GuestCheckouts
    WHERE guest_id = @GuestId;
END;
GO

CREATE PROCEDURE dbo.sp_Products_With_Categories
AS
BEGIN
    SET NOCOUNT ON;
    SELECT product_id, public_id, product_name, description
    FROM dbo.Products;

    SELECT category_id, public_id, category_name
    FROM dbo.Categories;

    SELECT product_id, category_id
    FROM dbo.ProductCategories;
END;
GO

CREATE PROCEDURE dbo.sp_AllCarts_With_Items
AS
BEGIN
    SET NOCOUNT ON;
    SELECT cart_id, public_id, user_id
    FROM dbo.Carts;

    SELECT cart_item_id, public_id, cart_id, variant_id, quantity
    FROM dbo.CartItems;
END;
GO

CREATE PROCEDURE dbo.sp_AddCartItem
    @CartId INT,
    @VariantId INT,
    @Quantity INT,
    @NewCartItemId INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO dbo.CartItems (cart_id, variant_id, quantity)
    VALUES (@CartId, @VariantId, @Quantity);

    SET @NewCartItemId = SCOPE_IDENTITY();
END;
GO

-- Precio efectivo de una variante: precio base menos el mejor descuento vigente que le aplique
-- (directo por DiscountVariants, o heredado por categoría vía DiscountCategories). "Vigente"
-- exige is_active = 1 Y estar dentro de start_date/end_date -- un descuento puede seguir activo
-- pero ya vencido, o desactivado a mano antes de vencer; ambos casos deben quedar afuera. Si hay
-- más de un descuento aplicable, gana el que dé mayor monto en moneda (no importa si es
-- porcentaje o monto fijo, se comparan ya convertidos). @AppliedDiscountName queda NULL si
-- ninguno aplica.
CREATE PROCEDURE dbo.sp_GetVariantEffectivePrice
    @VariantId INT,
    @BasePrice DECIMAL(10, 2) OUTPUT,
    @EffectivePrice DECIMAL(10, 2) OUTPUT,
    @AppliedDiscountName NVARCHAR(100) OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT @BasePrice = price
    FROM dbo.ProductVariants
    WHERE variant_id = @VariantId;

    SET @EffectivePrice = @BasePrice;
    SET @AppliedDiscountName = NULL;

    ;WITH ApplicableDiscounts AS (
        SELECT d.name, d.discount_type, d.value
        FROM dbo.Discounts d
        INNER JOIN dbo.DiscountVariants dv ON dv.discount_id = d.discount_id
        WHERE dv.variant_id = @VariantId
          AND d.is_active = 1
          AND GETDATE() >= d.start_date
          AND (d.end_date IS NULL OR GETDATE() <= d.end_date)

        UNION

        SELECT d.name, d.discount_type, d.value
        FROM dbo.Discounts d
        INNER JOIN dbo.DiscountCategories dc ON dc.discount_id = d.discount_id
        INNER JOIN dbo.ProductCategories pc ON pc.category_id = dc.category_id
        INNER JOIN dbo.ProductVariants pv ON pv.product_id = pc.product_id
        WHERE pv.variant_id = @VariantId
          AND d.is_active = 1
          AND GETDATE() >= d.start_date
          AND (d.end_date IS NULL OR GETDATE() <= d.end_date)
    ),
    ComputedDiscounts AS (
        SELECT
            name,
            CASE discount_type
                WHEN 'Percentage' THEN @BasePrice * value / 100.0
                ELSE value
            END AS discount_amount
        FROM ApplicableDiscounts
    )
    SELECT TOP 1
        @AppliedDiscountName = name,
        @EffectivePrice = @BasePrice - CASE WHEN discount_amount > @BasePrice THEN @BasePrice ELSE discount_amount END
    FROM ComputedDiscounts
    ORDER BY discount_amount DESC;
END;
GO

-- Detalle de un combo. No filtra por vigencia a propósito (a diferencia del listado de combos
-- vigentes que arma Example14 con un SELECT directo): sirve tanto para mostrar un combo activo
-- como uno vencido si ya sabés su id.
CREATE PROCEDURE dbo.sp_Combo_With_Items
    @ComboId INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT combo_id, public_id, name, combo_price, start_date, end_date, is_active
    FROM dbo.Combos
    WHERE combo_id = @ComboId;

    SELECT combo_item_id, public_id, combo_id, variant_id, quantity
    FROM dbo.ComboItems
    WHERE combo_id = @ComboId;
END;
GO

CREATE PROCEDURE dbo.sp_Variant_With_Movements
    @VariantId INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT variant_id, public_id, product_id, sku, price, stock_quantity
    FROM dbo.ProductVariants
    WHERE variant_id = @VariantId;

    SELECT movement_id, public_id, variant_id, movement_type, quantity, movement_date, order_id, notes
    FROM dbo.InventoryMovements
    WHERE variant_id = @VariantId
    ORDER BY movement_date;
END;
GO

-- Stock disponible real para vender: lo físico en depósito menos lo ya reservado en carritos
-- (todavía sin convertirse en orden). Sin esto, dos clientes podrían "comprar" las mismas
-- últimas unidades desde sus carritos sin que ninguno se entere hasta el checkout.
CREATE PROCEDURE dbo.sp_GetVariantAvailableStock
    @VariantId INT,
    @PhysicalStock INT OUTPUT,
    @ReservedInCarts INT OUTPUT,
    @AvailableStock INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT @PhysicalStock = stock_quantity
    FROM dbo.ProductVariants
    WHERE variant_id = @VariantId;

    SELECT @ReservedInCarts = ISNULL(SUM(quantity), 0)
    FROM dbo.CartItems
    WHERE variant_id = @VariantId;

    SET @AvailableStock = @PhysicalStock - @ReservedInCarts;
END;
GO
