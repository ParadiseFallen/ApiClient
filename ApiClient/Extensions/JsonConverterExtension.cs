using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.Json.Serialization;

namespace ApiClient.Extensions
{
    public static class JsonConverterExtension
    {
        /// <summary>
        /// Scan assembly for JsonConverter<>
        /// </summary>
        /// <param name="assembly">Target scan assembly. If set to null get executing assembly</param>
        /// <param name="filter">Filter converters</param>
        /// <returns>List of instantiated converters</returns>
        public static IEnumerable<JsonConverter> GetAllConverters(
            Assembly assembly = null, 
            Func<Type, bool> filter = null,
            Func<Type,JsonConverter> instanceBuilder = null)
        {
            var list = new List<JsonConverter>();
            (assembly ?? Assembly.GetExecutingAssembly())
                    .GetTypes()
                    .Where(
                        t => t.BaseType != null &&
                        t.BaseType.IsGenericType &&
                        t.BaseType.GetGenericTypeDefinition() == typeof(JsonConverter<>))
                    .Where((x) => filter?.Invoke(x) ?? true)
                    .ToList()
                    // forach found types that matches filer and derived from <c>JsonConverter<></c>
                    .ForEach(type =>
                    {
                        list.Add(
                            // use instance builder if set
                            instanceBuilder?.Invoke(type) ?? 
                            // or instantiate as default
                            (JsonConverter)type.GetConstructor(Type.EmptyTypes).Invoke(null));
                    });

            return list;
        }

    }
}
