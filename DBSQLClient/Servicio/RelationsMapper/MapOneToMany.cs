using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace DBSQLClient.Servicio.RelationsMapper
{
    internal class MapOneToMany
    {
    }

    private static void SetEmptyCollection(object parent, PropertyInfo prop)
        {
            var list = Activator.CreateInstance(prop.PropertyType);
            prop.SetValue(parent, list);
        }

    }

}
