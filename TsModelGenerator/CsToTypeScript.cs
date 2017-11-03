namespace TsModelGenerator
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using ionix.Utils.Extensions;
    using ionix.Utils.Reflection;
    using Newtonsoft.Json;

    public interface ITypeScriptCodeWriter<in TSource>
    {
        void Write(StringBuilder sb, HashSet<Type> types, TSource source);
    }

    public sealed class PropertyWriter : ITypeScriptCodeWriter<PropertyInfo>
    {
        private static string GetTypeName(HashSet<Type> types, Type propertyType)
        {
            var jsType = propertyType.ToJsType();
            string jsTypeStr = jsType.ToString();
            if (jsType == JavascriptType.any)
            {
                if (types.Contains(propertyType) && !propertyType.IsEnum)
                {
                    jsTypeStr = propertyType.Name;
                }
                else
                {
                    if (ModuleWriter.IEnumerableType.IsAssignableFrom(propertyType))
                    {
                        if (propertyType.IsGenericType)
                        {
                            Type genericType = propertyType.GetGenericArguments().First();
                            return GetTypeName(types, genericType);
                        }
                    }
                }
            }

            return jsTypeStr;
        }
        public void Write(StringBuilder sb, HashSet<Type> types, PropertyInfo pi)
        {
            //var jsType = pi.PropertyType.ToJsType();
            //string jsTypeStr = jsType.ToString();
            //if (jsType == JavascriptType.any)
            //{
            //    if (types.Contains(pi.PropertyType) && !pi.PropertyType.IsEnum)
            //    {
            //        jsTypeStr = pi.PropertyType.Name;
            //    }
            //}

            var jpAttr = pi.GetCustomAttribute<JsonPropertyAttribute>();
            string propertyName = jpAttr != null ? jpAttr.PropertyName : pi.Name;

            string jsTypeStr = GetTypeName(types, pi.PropertyType);
            sb.Append("    ");
            sb.Append(propertyName)
                .Append("?: ") //Interface olduğu içim optional
                .Append(jsTypeStr);
            if (ReflectionExtensions.IsEnumerable(pi.PropertyType))
                sb.Append("[]");
            sb.Append(";");
        }
    }

    public sealed class InterfaceWtiter : ITypeScriptCodeWriter<Type>
    {
        private static readonly Type AttributeType = typeof(Attribute);
        public void Write(StringBuilder sb, HashSet<Type> types, Type source)
        {
            if (AttributeType.IsAssignableFrom(source))
                return;

            var className = source.IsGenericType ? source.Name.Replace("`1", "") : source.Name;

            PropertyInfo[] pis = source.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            if (pis.IsEmptyList()) return;

            sb.Append("export interface ")
                .Append(className)
                .Append(" { ");

            PropertyWriter pw = new PropertyWriter();
            foreach (PropertyInfo pi in pis)
            {
                sb.AppendLine();
                pw.Write(sb, types, pi);
            }

            sb.AppendLine().Append("}");
        }
    }

    public sealed class ModuleWriter
    {
        internal static readonly Type IEnumerableType = typeof(IEnumerable);

        public void Write(StringBuilder text, Assembly asm)
        {
            var types = new HashSet<Type>(asm.GetTypes().OrderBy(p => p.Name));

            InterfaceWtiter iw = new InterfaceWtiter();
            foreach (Type type in types)
            {
                if (IEnumerableType.IsAssignableFrom(type))
                    continue;//Collection tipi ise işleme.
                iw.Write(text, types, type);
                text.AppendLine().AppendLine();
            }
        }
    }
}