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
