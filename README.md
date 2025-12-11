# ⚡ Referencia Rápida - SqlHelper

## 🎯 Cheat Sheet

```csharp
using DBSQLClient.Servicio.Helpers;
```

---

## 📝 **Sintaxis Rápida**

### Crear Parámetros

| Código | Descripción |
|--------|-------------|
| `SqlHelper.Param("Name", value)` | Parámetro simple |
| `SqlHelper.Params(("N1", v1), ("N2", v2))` | Múltiples parámetros |
| `SqlHelper.Int("Id", 123)` | Entero |
| `SqlHelper.String("Name", "Juan", 100)` | String con tamaño |
| `SqlHelper.Decimal("Price", 99.99m, 18, 2)` | Decimal con precisión |
| `SqlHelper.DateTime("Date", DateTime.Now)` | Fecha/hora |
| `SqlHelper.Bool("IsActive", true)` | Booleano |
| `SqlHelper.Guid("UserId", guid)` | GUID |
| `SqlHelper.OutParam("Result", SqlDbType.Int)` | Parámetro de salida |

---

## 🔥 **Ejemplos de 1 Línea**

```csharp
// Procedimiento simple
await service.ExecuteAsync("sp_GetUser", SqlHelper.Params(("Id", 123)));

// Consulta con filtro
await service.QueryAsync("SELECT * FROM Users WHERE Id = @Id", 
    SqlHelper.Params(("Id", userId)));

// Con múltiples parámetros
await service.ExecuteAsync("sp_UpdateUser", SqlHelper.Params(
    ("Id", 123), ("Name", "Juan"), ("Email", "juan@email.com")
));
```

---

## 📊 **Métodos Disponibles**

### Creación Básica
```csharp
SqlHelper.Param(name, value)
SqlHelper.Param(name, value, type)
SqlHelper.Param(name, value, type, size)
```

### Por Tipo
```csharp
SqlHelper.Int(name, value)
SqlHelper.String(name, value, size = -1)
SqlHelper.Decimal(name, value, precision = 18, scale = 2)
SqlHelper.DateTime(name, value)
SqlHelper.DateTime2(name, value)
SqlHelper.Date(name, value)
SqlHelper.Bool(name, value)
SqlHelper.Guid(name, value)
SqlHelper.Binary(name, value)
```

### Salida
```csharp
SqlHelper.OutParam(name, type)
SqlHelper.InOutParam(name, value, type)
SqlHelper.ReturnParam()
```

### Múltiples
```csharp
SqlHelper.Params((name, value), ...)
SqlHelper.FromDictionary(dict)
SqlHelper.FromObject(obj)
```

### Obtener Valores
```csharp
SqlHelper.GetOutputValue<T>(parameter)
```

---

## 🏗️ **Builder Pattern**

```csharp
var parametros = new SqlParameterBuilder()
    .AddInt("Id", 123)
    .AddString("Name", "Juan")
    .AddDecimal("Price", 99.99m)
    .AddOutput("Result", SqlDbType.Int)
    .Build();
```

---

## 🔧 **Extension Methods**

```csharp
param.AsOutput()
param.AsInputOutput()
param.WithSize(100)
param.WithPrecision(18, 2)
param.WithValue(value)
```

---

## 💡 **Patrones Comunes**

### Parámetro Opcional
```csharp
SqlHelper.Param("Optional", value ?? null)
```

### String Largo
```csharp
SqlHelper.String("Text", longText, -1)  // NVARCHAR(MAX)
```

### Fecha sin Hora
```csharp
SqlHelper.Date("BirthDate", DateTime.Today)
```

### Valor de Salida
```csharp
var output = SqlHelper.OutParam("Result", SqlDbType.Int);
await service.ExecuteAsync("sp_Proc", new[] { output });
int result = SqlHelper.GetOutputValue<int>(output) ?? 0;
```

---

## ⚠️ **Errores Comunes**

| ❌ Incorrecto | ✅ Correcto |
|--------------|-------------|
| `Param("Name", null)` | `Param("Name", value ?? null)` ✓ |
| `String("Text", text)` sin tamaño | `String("Text", text, -1)` ✓ |
| No verificar null en output | `GetOutputValue<int>(p) ?? 0` ✓ |

---

## 🚀 **Inicio Rápido - 3 Pasos**

### 1. Copia SqlHelper.cs a tu proyecto

### 2. Agrega using
```csharp
using DBSQLClient.Servicio.Helpers;
```

### 3. Usa en tu código
```csharp
var parametros = SqlHelper.Params(
    ("UserId", 123),
    ("Status", "Active")
);

var result = await service.ExecuteAsync("sp_GetUsers", parametros);
```

---

## 📚 **Más Información**

Ver ejemplos completos en: `Ejemplos de Uso - SqlHelper`


--- 

# 📘 Guía Completa - Uso de SqlParameter

## ✅ **Código Actualizado**

El servicio ahora usa `SqlParameter` de Microsoft directamente. Más simple y estándar.

---

## 🎯 **Formas de Crear Parámetros (de mejor a menos recomendada)**

### **Opción 1: Constructor Simple (RECOMENDADA)** ⭐⭐⭐⭐⭐

```csharp
var parametros = new[]
{
    new SqlParameter("@pN1", 1),
    new SqlParameter("@pN2", 0),
    new SqlParameter("@pN3", 0)
};

var result = await service.ExecuteAsync("sp_MiProcedimiento", parametros);
```

**✅ Ventajas:**
- Más simple y directo
- Menos líneas de código
- Tipo se infiere automáticamente

---

### **Opción 2: Constructor con Tipo Explícito** ⭐⭐⭐⭐

```csharp
var parametros = new[]
{
    new SqlParameter("@pN1", SqlDbType.Int) { Value = 1 },
    new SqlParameter("@pN2", SqlDbType.Int) { Value = 0 },
    new SqlParameter("@pN3", SqlDbType.Int) { Value = 0 }
};
```

**Cuándo usar:**
- Tipos complejos (DateTime, Decimal, etc.)
- Para evitar ambigüedades
- Para mayor control sobre el tipo SQL

---

### **Opción 3: Object Initializer** ⭐⭐⭐

```csharp
var parametros = new[]
{
    new SqlParameter { ParameterName = "@pN1", Value = 1, SqlDbType = SqlDbType.Int },
    new SqlParameter { ParameterName = "@pN2", Value = 0, SqlDbType = SqlDbType.Int },
    new SqlParameter { ParameterName = "@pN3", Value = 0, SqlDbType = SqlDbType.Int }
};
```

---

### **Opción 4: Inline (Para casos simples)** ⭐⭐⭐⭐

```csharp
var result = await service.ExecuteAsync("sp_MiProcedimiento", new[]
{
    new SqlParameter("@pN1", 1),
    new SqlParameter("@pN2", 0),
    new SqlParameter("@pN3", 0)
});
```

---

## 📊 **Ejemplos por Tipo de Dato**

### **Enteros**
```csharp
new SqlParameter("@Id", 123)
new SqlParameter("@Count", SqlDbType.Int) { Value = 100 }
new SqlParameter("@BigNumber", SqlDbType.BigInt) { Value = 999999999L }
```

### **Cadenas**
```csharp
new SqlParameter("@Name", "Juan Pérez")
new SqlParameter("@Email", SqlDbType.NVarChar, 100) { Value = "juan@email.com" }
new SqlParameter("@Description", SqlDbType.NVarChar, -1) { Value = longText } // -1 = MAX
```

### **Fechas**
```csharp
new SqlParameter("@CreatedDate", SqlDbType.DateTime) { Value = DateTime.Now }
new SqlParameter("@UpdatedDate", SqlDbType.DateTime2) { Value = DateTime.UtcNow }
new SqlParameter("@OnlyDate", SqlDbType.Date) { Value = DateTime.Today }
new SqlParameter("@OnlyTime", SqlDbType.Time) { Value = DateTime.Now.TimeOfDay }
```

### **Decimales/Dinero**
```csharp
new SqlParameter("@Price", SqlDbType.Decimal) { Value = 99.99m, Precision = 18, Scale = 2 }
new SqlParameter("@Amount", SqlDbType.Money) { Value = 1500.50m }
```

### **Booleanos**
```csharp
new SqlParameter("@IsActive", SqlDbType.Bit) { Value = true }
new SqlParameter("@HasPermission", true) // Se convierte a bit automáticamente
```

### **Valores NULL**
```csharp
new SqlParameter("@OptionalField", SqlDbType.Int) { Value = DBNull.Value }
new SqlParameter("@NullableDate", SqlDbType.DateTime) { Value = (object?)null ?? DBNull.Value }
```

### **Binarios**
```csharp
byte[] fileData = File.ReadAllBytes("image.jpg");
new SqlParameter("@FileData", SqlDbType.VarBinary, -1) { Value = fileData }
```

### **Uniqueidentifier (GUID)**
```csharp
new SqlParameter("@UserId", SqlDbType.UniqueIdentifier) { Value = Guid.NewGuid() }
```

### **XML**
```csharp
string xmlData = "<root><item>value</item></root>";
new SqlParameter("@XmlData", SqlDbType.Xml) { Value = xmlData }
```

---

## 🔧 **Parámetros de Salida (Output)**

```csharp
var parametros = new[]
{
    new SqlParameter("@InputId", 123),
    new SqlParameter("@OutputValue", SqlDbType.Int) { Direction = ParameterDirection.Output },
    new SqlParameter("@ReturnValue", SqlDbType.Int) { Direction = ParameterDirection.ReturnValue }
};

var result = await service.ExecuteAsync("sp_GetData", parametros);

// Leer valores de salida
int outputValue = (int)parametros[1].Value;
int returnValue = (int)parametros[2].Value;
```

---

## 🔄 **Parámetros Input/Output**

```csharp
var param = new SqlParameter("@Counter", SqlDbType.Int)
{
    Direction = ParameterDirection.InputOutput,
    Value = 10
};

var result = await service.ExecuteAsync("sp_IncrementCounter", new[] { param });

int newValue = (int)param.Value; // Valor actualizado por el SP
```

---

## 💡 **Métodos Helper Recomendados**

### **Helper Básico**

```csharp
public static class SqlHelper
{
    public static SqlParameter Param(string name, object value)
    {
        return new SqlParameter(name, value ?? DBNull.Value);
    }

    public static SqlParameter Param(string name, object value, SqlDbType type)
    {
        return new SqlParameter(name, type) { Value = value ?? DBNull.Value };
    }

    public static SqlParameter OutParam(string name, SqlDbType type)
    {
        return new SqlParameter(name, type) { Direction = ParameterDirection.Output };
    }
}

// USO
var parametros = new[]
{
    SqlHelper.Param("@Id", 123),
    SqlHelper.Param("@Name", "Juan"),
    SqlHelper.OutParam("@Result", SqlDbType.Int)
};
```

### **Helper con Fluent API**

```csharp
public static class SqlParameterExtensions
{
    public static SqlParameter AsOutput(this SqlParameter param)
    {
        param.Direction = ParameterDirection.Output;
        return param;
    }

    public static SqlParameter WithSize(this SqlParameter param, int size)
    {
        param.Size = size;
        return param;
    }

    public static SqlParameter WithPrecision(this SqlParameter param, byte precision, byte scale)
    {
        param.Precision = precision;
        param.Scale = scale;
        return param;
    }
}

// USO
var parametros = new[]
{
    new SqlParameter("@Name", SqlDbType.NVarChar).WithSize(100).Value = "Juan",
    new SqlParameter("@Price", SqlDbType.Decimal).WithPrecision(18, 2).Value = 99.99m,
    new SqlParameter("@Result", SqlDbType.Int).AsOutput()
};
```

---

## 📝 **Ejemplos Completos de Uso**

### **Ejemplo 1: Consulta Simple**

```csharp
var service = new SqlClientService(connectionString);

var parametros = new[]
{
    new SqlParameter("@UserId", 123)
};

var result = await service.QueryAsync(
    "SELECT * FROM Users WHERE Id = @UserId",
    parametros
);

var users = result.ToList<User>();
```

### **Ejemplo 2: Procedimiento con Múltiples Parámetros**

```csharp
var parametros = new[]
{
    new SqlParameter("@StartDate", SqlDbType.DateTime) { Value = DateTime.Now.AddDays(-30) },
    new SqlParameter("@EndDate", SqlDbType.DateTime) { Value = DateTime.Now },
    new SqlParameter("@Status", SqlDbType.NVarChar, 50) { Value = "Active" },
    new SqlParameter("@MinAmount", SqlDbType.Decimal) { Value = 100m }
};

var result = await service.ExecuteAsync("sp_GetSalesReport", parametros);
```

### **Ejemplo 3: Con Parámetros de Salida**

```csharp
var parametros = new[]
{
    new SqlParameter("@UserId", 123),
    new SqlParameter("@TotalOrders", SqlDbType.Int) { Direction = ParameterDirection.Output },
    new SqlParameter("@TotalAmount", SqlDbType.Decimal) { Direction = ParameterDirection.Output, Precision = 18, Scale = 2 }
};

var result = await service.ExecuteAsync("sp_GetUserStatistics", parametros);

// Leer valores de salida
int totalOrders = (int)parametros[1].Value;
decimal totalAmount = (decimal)parametros[2].Value;

Console.WriteLine($"Órdenes: {totalOrders}, Total: ${totalAmount}");
```

### **Ejemplo 4: Con Valores Nullables**

```csharp
int? optionalId = null;
string? optionalName = null;

var parametros = new[]
{
    new SqlParameter("@Id", (object?)optionalId ?? DBNull.Value),
    new SqlParameter("@Name", (object?)optionalName ?? DBNull.Value)
};

var result = await service.QueryAsync("SELECT * FROM Users WHERE (@Id IS NULL OR Id = @Id)", parametros);
```

### **Ejemplo 5: Inline para Casos Simples**

```csharp
// Sin variable intermedia
var result = await service.ExecuteAsync("sp_DeleteUser", new[]
{
    new SqlParameter("@UserId", 123)
});

if (result.HasRows)
{
    Console.WriteLine("Usuario eliminado");
}
```

---

## ⚠️ **Errores Comunes y Soluciones**

### **Error 1: Tipo incorrecto**
```csharp
// ❌ INCORRECTO
new SqlParameter("@Price", DbType.Decimal) // DbType en lugar de SqlDbType

// ✅ CORRECTO
new SqlParameter("@Price", SqlDbType.Decimal)
```

### **Error 2: Tamaño de string**
```csharp
// ❌ PUEDE TRUNCAR
new SqlParameter("@LongText", longString) // Sin especificar tamaño

// ✅ CORRECTO
new SqlParameter("@LongText", SqlDbType.NVarChar, -1) { Value = longString } // -1 = MAX
```

### **Error 3: Valores NULL**
```csharp
// ❌ INCORRECTO
new SqlParameter("@OptionalField", null) // Puede causar error

// ✅ CORRECTO
new SqlParameter("@OptionalField", (object?)value ?? DBNull.Value)
```

### **Error 4: Olvidar @ en el nombre**
```csharp
// ✅ AMBOS SON VÁLIDOS
new SqlParameter("@UserId", 123)  // Con @
new SqlParameter("UserId", 123)   // Sin @ (se agrega automáticamente)
```

---

## 🎯 **Mejores Prácticas**

1. **Usa parámetros siempre** - Previene SQL Injection
2. **Especifica tipos para decimales y fechas** - Evita problemas de conversión
3. **Usa -1 para NVARCHAR(MAX)** - Para textos largos
4. **Maneja valores NULL correctamente** - Usa DBNull.Value
5. **Nombra parámetros con @** - Es la convención estándar
6. **Reutiliza helpers** - Crea métodos helper para casos comunes
7. **Valida tamaños** - Especifica Size para strings

---

## 📚 **Referencia Rápida de Tipos**

| Tipo C# | SqlDbType | Ejemplo |
|---------|-----------|---------|
| `int` | `Int` | `new SqlParameter("@Id", 123)` |
| `long` | `BigInt` | `new SqlParameter("@BigId", 999999L)` |
| `string` | `NVarChar` | `new SqlParameter("@Name", "Juan")` |
| `decimal` | `Decimal` | `new SqlParameter("@Price", 99.99m)` |
| `DateTime` | `DateTime` / `DateTime2` | `new SqlParameter("@Date", DateTime.Now)` |
| `bool` | `Bit` | `new SqlParameter("@IsActive", true)` |
| `Guid` | `UniqueIdentifier` | `new SqlParameter("@Guid", Guid.NewGuid())` |
| `byte[]` | `VarBinary` | `new SqlParameter("@Data", byteArray)` |

---

## ✅ **Resumen**

**Para el 90% de los casos:**
```csharp
var parametros = new[]
{
    new SqlParameter("@Param1", valor1),
    new SqlParameter("@Param2", valor2)
};

var result = await service.ExecuteAsync("sp_MiProcedimiento", parametros);
```

**Para casos complejos, crea helpers personalizados** según las necesidades de tu proyecto.





