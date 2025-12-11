# 🚀 Ejemplos de Uso - SqlHelper

## 📦 Instalación

1. Copia la clase `SqlHelper.cs` en tu proyecto
2. Agrega el using:
```csharp
using DBSQLClient.Servicio.Helpers;
```

---

## 🎯 **Método 1: SqlHelper.Param() - Simple y Directo**

### Uso Básico
```csharp
// ❌ ANTES (Verbose)
var parametros = new[]
{
    new SqlParameter("@pN1", 1),
    new SqlParameter("@pN2", 0),
    new SqlParameter("@pN3", 0)
};

// ✅ AHORA (Con Helper - Igual de simple, más consistente)
var parametros = new[]
{
    SqlHelper.Param("pN1", 1),      // Sin @ necesario
    SqlHelper.Param("pN2", 0),
    SqlHelper.Param("pN3", 0)
};

var result = await service.ExecuteAsync("sp_MiProcedimiento", parametros);
```

### Con Tipos Específicos
```csharp
var parametros = new[]
{
    SqlHelper.Param("Id", 123, SqlDbType.Int),
    SqlHelper.Param("Name", "Juan", SqlDbType.NVarChar, 100),
    SqlHelper.Param("Price", 99.99m, SqlDbType.Decimal)
};
```

---

## 🎯 **Método 2: SqlHelper.Params() - Ultra Compacto**

```csharp
// ✅ UNA SOLA LÍNEA para múltiples parámetros
var parametros = SqlHelper.Params(
    ("pN1", 1),
    ("pN2", 0),
    ("pN3", 0)
);

var result = await service.ExecuteAsync("sp_MiProcedimiento", parametros);
```

### Ejemplo Real
```csharp
// Insertar usuario en una línea
var parametros = SqlHelper.Params(
    ("Name", "Juan Pérez"),
    ("Email", "juan@email.com"),
    ("Age", 30),
    ("IsActive", true)
);

await service.ExecuteAsync("sp_InsertUser", parametros);
```

---

## 🎯 **Método 3: Métodos por Tipo - Más Control**

```csharp
var parametros = new[]
{
    SqlHelper.Int("UserId", 123),
    SqlHelper.String("Name", "Juan", 100),
    SqlHelper.Decimal("Price", 99.99m, 18, 2),
    SqlHelper.DateTime("CreatedDate", DateTime.Now),
    SqlHelper.Bool("IsActive", true)
};
```

### Todos los Métodos por Tipo
```csharp
SqlHelper.Int("Id", 123)                                    // Int
SqlHelper.String("Name", "Juan", 100)                       // NVARCHAR(100)
SqlHelper.Decimal("Price", 99.99m, 18, 2)                   // DECIMAL(18,2)
SqlHelper.DateTime("Date", DateTime.Now)                    // DateTime
SqlHelper.DateTime2("PreciseDate", DateTime.Now)            // DateTime2
SqlHelper.Date("OnlyDate", DateTime.Today)                  // Date
SqlHelper.Bool("IsActive", true)                            // Bit
SqlHelper.Guid("UserId", Guid.NewGuid())                    // UniqueIdentifier
SqlHelper.Binary("FileData", byteArray)                     // VarBinary(MAX)
```

---

## 🎯 **Método 4: Parámetros de Salida**

### Simple
```csharp
var parametros = new[]
{
    SqlHelper.Param("InputId", 123),
    SqlHelper.OutParam("Result", SqlDbType.Int)
};

var result = await service.ExecuteAsync("sp_Calculate", parametros);

// Leer valor de salida
int resultValue = SqlHelper.GetOutputValue<int>(parametros[1]);
Console.WriteLine($"Resultado: {resultValue}");
```

### Múltiples Salidas
```csharp
var outputTotal = SqlHelper.OutParam("TotalOrders", SqlDbType.Int);
var outputAmount = SqlHelper.OutParam("TotalAmount", SqlDbType.Decimal);

var parametros = new[]
{
    SqlHelper.Int("UserId", 123),
    outputTotal,
    outputAmount
};

await service.ExecuteAsync("sp_GetUserStatistics", parametros);

int totalOrders = SqlHelper.GetOutputValue<int>(outputTotal) ?? 0;
decimal totalAmount = SqlHelper.GetOutputValue<decimal>(outputAmount) ?? 0m;

Console.WriteLine($"Órdenes: {totalOrders}, Monto: ${totalAmount}");
```

---

## 🎯 **Método 5: SqlParameterBuilder - Fluent API**

### Uso Básico
```csharp
var parametros = new SqlParameterBuilder()
    .Add("UserId", 123)
    .Add("Name", "Juan")
    .Add("Email", "juan@email.com")
    .Build();

await service.ExecuteAsync("sp_UpdateUser", parametros);
```

### Con Tipos Específicos
```csharp
var parametros = new SqlParameterBuilder()
    .AddInt("UserId", 123)
    .AddString("Name", "Juan Pérez", 100)
    .AddDecimal("Salary", 5000.50m, 18, 2)
    .AddDateTime("HireDate", DateTime.Now)
    .AddBool("IsActive", true)
    .Build();
```

### Con Parámetros de Salida
```csharp
var builder = new SqlParameterBuilder()
    .AddInt("InputValue", 100)
    .AddOutput("Result", SqlDbType.Int)
    .AddOutput("Message", SqlDbType.NVarChar);

var parametros = builder.Build();

await service.ExecuteAsync("sp_ProcessData", parametros);

int result = SqlHelper.GetOutputValue<int>(parametros[1]) ?? 0;
string message = SqlHelper.GetOutputValue<string>(parametros[2]) ?? "";
```

---

## 🎯 **Método 6: Desde Diccionario**

```csharp
var data = new Dictionary<string, object?>
{
    ["UserId"] = 123,
    ["Name"] = "Juan",
    ["Email"] = "juan@email.com",
    ["Age"] = 30
};

var parametros = SqlHelper.FromDictionary(data);
await service.ExecuteAsync("sp_UpdateUser", parametros);
```

---

## 🎯 **Método 7: Desde Objeto Anónimo**

```csharp
var user = new { UserId = 123, Name = "Juan", Email = "juan@email.com" };

var parametros = SqlHelper.FromObject(user);
await service.ExecuteAsync("sp_UpdateUser", parametros);
```

---

## 🎯 **Método 8: Extension Methods - Fluent**

```csharp
var parametros = new[]
{
    new SqlParameter("Name", "Juan").WithSize(100),
    new SqlParameter("Price", 99.99m).WithPrecision(18, 2),
    new SqlParameter("Result", SqlDbType.Int).AsOutput()
};
```

---

## 📊 **Comparación de Métodos**

| Método | Líneas | Legibilidad | Flexibilidad | Uso Recomendado |
|--------|--------|-------------|--------------|-----------------|
| **Param()** | 3-5 | ⭐⭐⭐⭐ | ⭐⭐⭐⭐ | Uso general |
| **Params()** | 1 | ⭐⭐⭐⭐⭐ | ⭐⭐⭐ | Parámetros simples |
| **Métodos por Tipo** | 3-5 | ⭐⭐⭐⭐⭐ | ⭐⭐⭐⭐⭐ | Tipos complejos |
| **Builder** | 5-8 | ⭐⭐⭐⭐ | ⭐⭐⭐⭐⭐ | Lógica compleja |
| **FromDictionary** | 2-3 | ⭐⭐⭐ | ⭐⭐⭐⭐ | Datos dinámicos |
| **FromObject** | 2 | ⭐⭐⭐⭐ | ⭐⭐⭐ | Objetos existentes |

---

## 🔥 **Ejemplos Completos del Mundo Real**

### Ejemplo 1: Login de Usuario
```csharp
var parametros = SqlHelper.Params(
    ("Email", email),
    ("Password", passwordHash)
);

var result = await service.ExecuteAsync("sp_ValidateUser", parametros);
var user = result.FirstOrDefault<User>();
```

### Ejemplo 2: Registro con Validación
```csharp
var outputUserId = SqlHelper.OutParam("NewUserId", SqlDbType.Int);
var outputMessage = SqlHelper.OutParam("Message", SqlDbType.NVarChar);

var parametros = new[]
{
    SqlHelper.String("Email", email, 100),
    SqlHelper.String("Name", name, 100),
    SqlHelper.String("Password", passwordHash, 255),
    outputUserId,
    outputMessage
};

await service.ExecuteAsync("sp_RegisterUser", parametros);

int? newUserId = SqlHelper.GetOutputValue<int>(outputUserId);
string message = SqlHelper.GetOutputValue<string>(outputMessage) ?? "";

if (newUserId.HasValue)
{
    Console.WriteLine($"Usuario creado con ID: {newUserId}");
}
else
{
    Console.WriteLine($"Error: {message}");
}
```

### Ejemplo 3: Búsqueda con Filtros Opcionales
```csharp
var parametros = new SqlParameterBuilder()
    .AddString("SearchTerm", searchTerm)
    .AddInt("CategoryId", categoryId)
    .AddDecimal("MinPrice", minPrice)
    .AddDecimal("MaxPrice", maxPrice)
    .AddDateTime("StartDate", startDate)
    .AddDateTime("EndDate", endDate)
    .AddInt("PageNumber", pageNumber)
    .AddInt("PageSize", pageSize)
    .Build();

var result = await service.ExecuteAsync("sp_SearchProducts", parametros);
var products = result.ToList<Product>();
```

### Ejemplo 4: Transacción Compleja
```csharp
var outputOrderId = SqlHelper.OutParam("OrderId", SqlDbType.Int);
var outputTotal = SqlHelper.OutParam("Total", SqlDbType.Decimal);

var parametros = new SqlParameterBuilder()
    .AddInt("UserId", userId)
    .AddDateTime("OrderDate", DateTime.Now)
    .AddString("ShippingAddress", address, 500)
    .AddDecimal("ShippingCost", shippingCost)
    .AddOutput("OrderId", SqlDbType.Int)
    .AddOutput("Total", SqlDbType.Decimal)
    .Build();

await service.ExecuteAsync("sp_CreateOrder", parametros);

int orderId = SqlHelper.GetOutputValue<int>(parametros[4]) ?? 0;
decimal total = SqlHelper.GetOutputValue<decimal>(parametros[5]) ?? 0m;
```

### Ejemplo 5: Reporte con Múltiples Tablas
```csharp
var parametros = new[]
{
    SqlHelper.DateTime("StartDate", startDate),
    SqlHelper.DateTime("EndDate", endDate),
    SqlHelper.Int("DepartmentId", departmentId)
};

var result = await service.ExecuteAsync("sp_GetSalesReport", parametros);

// Primera tabla: Resumen
var summary = result.AsDataTable(0).ToList<SalesSummary>();

// Segunda tabla: Detalle
var details = result.AsDataTable(1).ToList<SalesDetail>();

// Tercera tabla: Totales
var totals = result.AsDataTable(2).ToList<SalesTotals>();
```

---

## 💡 **Tips y Mejores Prácticas**

### 1. **Manejo de Valores NULL**
```csharp
// ✅ El helper maneja NULL automáticamente
SqlHelper.Param("OptionalField", nullableValue)  // Convierte a DBNull.Value

// ✅ Para valores condicionales
SqlHelper.Param("Field", condition ? value : null)
```

### 2. **Strings Largos**
```csharp
// ✅ Usa -1 para NVARCHAR(MAX)
SqlHelper.String("LongText", longContent, -1)
```

### 3. **Decimales con Precisión**
```csharp
// ✅ Especifica precisión y escala
SqlHelper.Decimal("Price", 99.999m, 18, 3)  // DECIMAL(18,3)
```

### 4. **Fechas sin Hora**
```csharp
// ✅ Usa Date para solo fecha
SqlHelper.Date("BirthDate", DateTime.Today)
```

### 5. **Reutilizar Builders**
```csharp
// ✅ Puedes reutilizar el builder
var builder = new SqlParameterBuilder()
    .AddInt("Type", 1)
    .AddBool("IsActive", true);

// Agregar más según condiciones
if (includeDeleted)
    builder.AddBool("IncludeDeleted", true);

var parametros = builder.Build();
```

---

## ✅ **Resumen - ¿Cuál Usar?**

### **Para el 80% de los casos:**
```csharp
var parametros = SqlHelper.Params(
    ("Param1", value1),
    ("Param2", value2)
);
```

### **Para tipos complejos:**
```csharp
var parametros = new[]
{
    SqlHelper.Int("Id", id),
    SqlHelper.Decimal("Price", price, 18, 2),
    SqlHelper.DateTime("Date", date)
};
```

### **Para lógica compleja:**
```csharp
var parametros = new SqlParameterBuilder()
    .AddInt("Id", id)
    .AddString("Name", name)
    .AddOutput("Result", SqlDbType.Int)
    .Build();
```

¡Elige el que mejor se adapte a tu caso de uso! 🚀
