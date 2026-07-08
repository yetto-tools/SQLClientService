# DBSQLClient.Demo

Demo ejecutable de [`DBSQLClient`](../DBSQLClient/README.md), orientado a una **tienda en
línea**: usuarios registrados y checkout como invitado, carrito, productos con variantes
(tallas, colores, etc.), órdenes y facturas. Cada archivo en `Examples/` muestra **una sola
capacidad** de la librería, aislada y pensada para copiar tal cual a otro proyecto (nuevo o
existente).

## Cómo correrlo

```bash
dotnet run --project DBSQLClient.Demo
```

No hace falta ningún paso manual: al arrancar, `Program.cs` llama a
`DemoDatabaseBootstrapper.EnsureCreatedAsync()`, que crea (si no existe) la base
`DBSQLClientDemo` en LocalDB (`(localdb)\MSSQLLocalDB`) y **siempre** reconstruye sus tablas,
procedimientos y datos de ejemplo desde cero (`Database/DemoDatabaseSetup.sql`). Cada corrida
del demo arranca de un estado limpio y predecible.

> Si preferís aplicar el script vos mismo (por ejemplo con `sqlcmd` o SSMS contra otra instancia
> de SQL Server), `Database/DemoDatabaseSetup.sql` es un archivo `.sql` normal y ejecutable —
> ajustá `Database/DemoDatabase.cs` con la connection string que corresponda.

**Advertencia:** el bootstrapper hace `DROP` + `CREATE` de todo en `DBSQLClientDemo` en cada
ejecución. Es el comportamiento correcto para un demo (reproducible, sin acumular datos de
corridas anteriores), pero **no repliques este patrón en un proyecto real**.

## Qué muestra cada ejemplo

| Archivo | Capacidad de `DBSQLClient` | Procedimiento / origen |
|---|---|---|
| `Examples/Example01_RawQuery.cs` | `QueryAsync` con SQL de texto, `ToList<T>` | `SELECT` directo sobre `Products` |
| `Examples/Example02_StoredProcedure.cs` | `ExecuteAsync` básico | `sp_GetProductById` |
| `Examples/Example03_OutputParameters.cs` | `SqlParams.OutParam` + `SqlQueryResult.GetOutputValue<T>` | `sp_GetCartTotal` (subtotal del carrito) |
| `Examples/Example04_OneToOne.cs` | Relación 1 a 1: `MapOneToOne` + `[OneToOne]` | `sp_Order_With_Invoice` |
| `Examples/Example05_ProductVariantAttributes.cs` | Atributos de variante en EAV: `MapOneToMany` + `[OneToMany]` | `sp_Variant_With_Attributes` |
| `Examples/Example06_OneToMany.cs` | Relación 1 a muchos: `MapOneToMany` + `[OneToMany]` | `sp_Order_With_Items` |
| `Examples/Example07_ManyToOne.cs` | Relación muchos a 1 con **dos padres posibles**: `MapManyToOne` + dos `[ManyToOne]` | `sp_Orders_With_User` / `sp_Orders_With_Guest` |
| `Examples/Example08_ManyToMany.cs` | Relación muchos a muchos vía tabla de unión: `MapManyToMany` + `[ManyToMany]` | `sp_Products_With_Categories` |
| `Examples/Example09_MultipleOneToMany.cs` | 1 a muchos con varios padres a la vez: `OneToManyMultiple` | `sp_AllCarts_With_Items` |
| `Examples/Example10_JsonSerialization.cs` | `SqlQueryResult.ToJson<T>` (columnas planas, sin relaciones) + `SaveToJsonFileAsync`/`FromJsonFileAsync`, contrastado con `ToJsonString` sobre un objeto ya mapeado con `MapManyToMany` (relaciones sí incluidas) | sobre `Products` |
| `Examples/Example11_ParameterBuilder.cs` | `SqlParameterBuilder` fluido + único INSERT del demo (escritura con output) | `sp_AddCartItem` |
| `Examples/Example12_GuidPublicId.cs` | Exponer un GUID público en vez del `Id` interno (evita enumeración/IDOR en una API) | `sp_GetProductByPublicId` |
| `Examples/Example13_VariantPricing.cs` | Precio efectivo con descuentos/promociones: 3 parámetros de salida en la misma llamada | `sp_GetVariantEffectivePrice` |
| `Examples/Example14_Combo.cs` | Combos con vigencia por fecha: `MapOneToMany` + `[OneToMany]` | `sp_Combo_With_Items` |
| `Examples/Example15_Inventory.cs` | Historial de stock (`MapOneToMany`) + stock disponible real: 3 parámetros de salida en la misma llamada | `sp_Variant_With_Movements` / `sp_GetVariantAvailableStock` |

`Program.cs` los corre todos en orden, con un encabezado por ejemplo y capturando la excepción
de cada uno por separado — si rompés uno mientras practicás, el resto del demo sigue corriendo.

## El caso "usuario o invitado" (`Example07_ManyToOne`)

Una `Order` pertenece a un `User` registrado **o** a un `GuestCheckout` invitado, nunca a ambos
(columnas `user_id`/`guest_id` nulas + `CHECK` en la tabla `Orders`). En el modelo `Order` esto
se declara con **dos atributos `[ManyToOne]` independientes**, uno por cada tipo de padre
posible — el sistema de relaciones de la librería ya soporta esto sin cambios, porque resuelve
la propiedad de navegación según el tipo pedido en `MapManyToOne<Order, User>()` o
`MapManyToOne<Order, GuestCheckout>()`.

## Por qué `InvoiceItems` existe por separado de `OrderItems` (`Example04_OneToOne`)

Una factura no es una vista en vivo de la orden: es un documento que, una vez emitido, no debería
cambiar aunque la orden sí lo haga después (corrección de precio, devolución parcial, etc.). Por
eso `InvoiceItems` es su propia tabla, con sus propias filas — se llena copiando `OrderItems` al
momento de emitir la factura (ver el seed en `DemoDatabaseSetup.sql`), no con un `JOIN` contra
`OrderItems` en tiempo de consulta. En este demo ambas tablas siempre coinciden porque no hay
correcciones posteriores, pero conceptualmente son independientes: `Order.Items` refleja la orden
tal como está *hoy*, `Invoice.Items` refleja lo que se facturó *en su momento*.

## GUID público vs. Id interno (`Example12_GuidPublicId`)

**Todas las tablas con clave propia** (`Users`, `GuestCheckouts`, `Categories`, `Products`,
`ProductVariants`, `ProductVariantAttributes`, `Carts`, `CartItems`, `Orders`, `OrderItems`,
`Invoices`, `InvoiceItems`, `Discounts`, `Combos`, `ComboItems`, `InventoryMovements`) siguen el
mismo patrón: el `INT IDENTITY` sigue siendo la clave primaria/foránea
real — la más compacta y eficiente para joins e índices — y una columna `public_id`
(`UNIQUEIDENTIFIER`, con `DEFAULT NEWID()` y `UNIQUE`) es lo que expondría una API en su lugar.
Si un cliente externo recibe `product_id=1`, puede probar `2`, `3`, etc. y enumerar todo el
catálogo o adivinar ids de otro recurso (IDOR); con un GUID aleatorio eso deja de ser viable. La
única excepción es `ProductCategories`: es una tabla de unión pura, identificada por el par
`(product_id, category_id)`, sin id propio al que darle un equivalente público.

La librería no necesita ningún cambio para este patrón — `[PrimaryKey]`/`[ForeignKey]`/`[Column]`
no exigen ningún tipo en particular, y `SqlParams.Param`/`AddParams` infieren
`SqlDbType.UniqueIdentifier` automáticamente de un `Guid`. Es la alternativa recomendada frente a
usar el GUID como PK real: esta última es más simple (un solo id en todos lados) pero el índice
clustered pesa 4 veces más que un `int` y sufre más fragmentación en inserts.

`Example12_GuidPublicId` solo ejercita el caso de `Product` (vía `sp_GetProductByPublicId`), pero
el mismo patrón está disponible en cualquier otro modelo a través de su columna `PublicId`.

## Precio separado del producto: descuentos, promociones, ofertas y combos

`ProductVariants.price` es solo el **precio base**. Todo lo demás vive separado, en tablas
propias, para no mezclar "cuánto cuesta el producto" con "qué reglas cambian ese precio":

- **`Discounts`** unifica promoción/descuento/oferta — son la misma idea: una regla temporal que
  reduce el precio (`discount_type` = `Percentage` o `FixedAmount`, con `start_date`/`end_date`/
  `is_active`). Puede aplicar a **una variante puntual** (`DiscountVariants`) o a **toda una
  categoría** (`DiscountCategories`, heredado por todos los productos de esa categoría).
- **`Combos`**/`ComboItems` agrupan variantes a un precio especial (`combo_price`), con la misma
  vigencia por fecha que un descuento. Solo catálogo por ahora: **no se pueden agregar al
  carrito todavía** — `CartItems`/`OrderItems` siguen referenciando variantes individuales; sería
  el siguiente paso natural (agregar una FK opcional a combo, con el mismo patrón de FK doble +
  `CHECK` que ya usa `Orders` para usuario/invitado).

Un descuento **vencido pero con `is_active = 1`** no debe aplicar — por eso `sp_GetVariantEffectivePrice`
y el listado de descuentos/combos vigentes de `Example13`/`Example14` siempre exigen **las dos
condiciones a la vez**: `is_active = 1` **y** estar dentro de `start_date`/`end_date`. El seed
incluye a propósito una "Promo Expirada 50%" (con `is_active = 1` pero `end_date` en el pasado) y
un "Combo Invierno" en la misma situación, para probar que ninguno de los dos se cuela.

Cuando una variante cae bajo más de un descuento vigente (uno directo + uno heredado de
categoría, por ejemplo), **gana el que dé mayor monto en moneda** — no importa si es porcentaje o
monto fijo, `sp_GetVariantEffectivePrice` convierte ambos a monto antes de comparar.

**Bug real encontrado mientras se armaba este ejemplo:** un output `SqlDbType.Decimal` sin
precisión/escala explícitas asume `Scale = 0` y ADO.NET redondea el valor de salida a un entero
sin avisar (`$99.97` volvía `$100.00` en `Example03`, antes de este fix). Por eso ahora existe
`SqlParams.OutParam(name, type, precision, scale)` — usalo siempre para output decimales.

## Inventario separado del catálogo: historial de movimientos y stock disponible

Mismo patrón que el precio: `ProductVariants.stock_quantity` es solo el **saldo actual**, no la
única fuente de verdad. `InventoryMovements` es la auditoría de cómo se llegó a ese saldo — cada
fila es un movimiento con cantidad **con signo** (positivo = entra, negativo = sale) y un
`movement_type` (`Purchase`, `Sale`, `Return`, `Adjustment`, `Damaged`). Los movimientos `Sale`/
`Return` traen `order_id` para trazar a la orden que los originó; el resto lo deja en `NULL`.

`sp_GetVariantAvailableStock` (3 parámetros de salida) responde la pregunta que el saldo por sí
solo no contesta: **cuánto queda realmente disponible para vender**, descontando lo que ya está
reservado en carritos (`CartItems`) pero todavía no se convirtió en orden. Sin esto, dos clientes
podrían ver "stock disponible" y agregar al carrito las mismas últimas unidades, sin que ninguno
se entere hasta el checkout — el carrito reserva, no debería vender dos veces lo mismo.

## Datos de ejemplo (y sus casos "sin hijos" a propósito)

- **Erick** (usuario registrado): carrito con 2 ítems, una orden pagada con factura.
- **Ana** (usuario registrado): carrito **vacío** — ver `Example09_MultipleOneToMany`, donde su
  carrito aparece con `Items: []` en vez de fallar. También tiene una orden pagada con factura.
- **Invitado Demo** (`GuestCheckout`): una orden **pendiente, sin factura todavía** — ver
  `Example04_OneToOne`, donde `Invoice` es genuinamente nullable para reflejar este caso.
- **Productos**: Camiseta Básica (Ropa + Ofertas, variantes con atributos Talla/Color),
  Zapatilla Runner (Calzado, atributos Talla/Ancho), Auriculares Bluetooth (Electrónica +
  Ofertas, su única variante **no tiene ningún atributo** — ver `Example05_ProductVariantAttributes`).
- **Descuentos**: 15% a toda la categoría Ropa, $10 fijos a la Zapatilla, y una promo del 50%
  **ya vencida** sobre la Camiseta que no debe aplicar. Los Auriculares no tienen ningún
  descuento en ningún lado.
- **Combos**: "Combo Verano" vigente (Camiseta + Zapatilla), "Combo Invierno" **ya vencido**.
- **Inventario**: Camiseta M/Rojo y Zapatilla tienen compra inicial + una venta (ligada a su
  orden); Auriculares tiene compra inicial + una unidad dañada; Camiseta L/Azul **no tiene ningún
  movimiento registrado todavía** — ver `Example15_Inventory`.
