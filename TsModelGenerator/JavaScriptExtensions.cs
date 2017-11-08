namespace TsModelGenerator
{
    using System;
    using System.Collections.Generic;
    using ionix.Utils;

    internal enum JavascriptType
    {
        @string = 0,
        number = 1,
        Date = 2,
        boolean = 3,
        Uint8Array = 4,
        any = 5,
    }


    internal static class JavaScriptExtensions
    {
        private static readonly object TypeMapSync = new object();
        private static Dictionary<Type, JavascriptType> typeMap;
        private static Dictionary<Type, JavascriptType> TypeMap
        {
            get
            {
                if (null == typeMap)
                {
                    lock (TypeMapSync)
                    {
                        if (null == typeMap)
                        {
                            typeMap = new Dictionary<Type, JavascriptType>();

                            typeMap.Add(CachedTypes.Byte, JavascriptType.number);
                            typeMap.Add(CachedTypes.SByte, JavascriptType.number);
                            typeMap.Add(CachedTypes.Int16, JavascriptType.number);
                            typeMap.Add(CachedTypes.UInt16, JavascriptType.number);
                            typeMap.Add(CachedTypes.Int32, JavascriptType.number);
                            typeMap.Add(CachedTypes.UInt32, JavascriptType.number);
                            typeMap.Add(CachedTypes.Int64, JavascriptType.number);
                            typeMap.Add(CachedTypes.UInt64, JavascriptType.number);
                            typeMap.Add(CachedTypes.Single, JavascriptType.number);
                            typeMap.Add(CachedTypes.Double, JavascriptType.number);
                            typeMap.Add(CachedTypes.Decimal, JavascriptType.number);
                            typeMap.Add(CachedTypes.Boolean, JavascriptType.boolean);
                            typeMap.Add(CachedTypes.String, JavascriptType.@string);
                            typeMap.Add(CachedTypes.Char, JavascriptType.@string);
                            typeMap.Add(CachedTypes.Guid, JavascriptType.@string);
                            typeMap.Add(CachedTypes.DateTime, JavascriptType.Date);
                            typeMap.Add(typeof(DateTimeOffset), JavascriptType.Date);
                           // typeMap.Add(CachedTypes.ByteArray, JavascriptType.Uint8Array); //Byte Array json da base64 string olması lazım.
                            typeMap.Add(CachedTypes.Nullable_Byte, JavascriptType.number);
                            typeMap.Add(CachedTypes.Nullable_SByte, JavascriptType.number);
                            typeMap.Add(CachedTypes.Nullable_Int16, JavascriptType.number);
                            typeMap.Add(CachedTypes.Nullable_UInt16, JavascriptType.number);
                            typeMap.Add(CachedTypes.Nullable_Int32, JavascriptType.number);
                            typeMap.Add(CachedTypes.Nullable_UInt32, JavascriptType.number);
                            typeMap.Add(CachedTypes.Nullable_Int64, JavascriptType.number);
                            typeMap.Add(CachedTypes.Nullable_UInt64, JavascriptType.number);
                            typeMap.Add(CachedTypes.Nullable_Single, JavascriptType.number);
                            typeMap.Add(CachedTypes.Nullable_Double, JavascriptType.number);
                            typeMap.Add(CachedTypes.Nullable_Decimal, JavascriptType.number);
                            typeMap.Add(CachedTypes.Nullable_Boolean, JavascriptType.boolean);
                            typeMap.Add(CachedTypes.Nullable_Char, JavascriptType.@string);
                            typeMap.Add(CachedTypes.Nullable_Guid, JavascriptType.@string);
                            typeMap.Add(CachedTypes.Nullable_DateTime, JavascriptType.Date);
                            typeMap.Add(typeof(DateTimeOffset?), JavascriptType.Date);
                        }
                    }
                }

                return typeMap;
            }
        }

        internal static JavascriptType ToJsType(this Type type)
        {
           // if (pi.Name == "JsonData")
             //   return JavascriptType.any;
            JavascriptType ret;
            if (TypeMap.TryGetValue(type, out ret))
            {
                return ret;
            }
            return JavascriptType.any;
        }

    }
}
