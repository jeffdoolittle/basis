using System;
using System.IO;
using System.Reflection;

namespace Basis
{
    public static class ReflectionHelper
    {
        public static object GetStaticProperty(string typeName, string property)
        {
            return ExceptionHandler.Do(() =>
            {
                var type = GetTypeFromName(typeName);
                return GetStaticProperty(type, property);
            });
        }

        public static object GetStaticProperty(Type type, string property)
        {
            return ExceptionHandler.Do(() =>
                type.InvokeMember(property, BindingFlags.Static | BindingFlags.Public | BindingFlags.GetField | BindingFlags.GetProperty, null, type, null));
        }

        public static Type GetTypeFromName(string typeName, string assemblyName, bool loadAssembly = false)
        {
            return ExceptionHandler.Do(() =>
            {
                if (loadAssembly)
                {
                    goto loadAssembly;
                }

                var type = Type.GetType(typeName);

                if (type != null) { return type; }

                var assemblies = AppDomain.CurrentDomain.GetAssemblies();

                foreach (var asm in assemblies)
                {
                    type = asm.GetType(typeName);

                    if (type != null) { return type; }
                }

                if (string.IsNullOrEmpty(assemblyName))
                {
                    throw new BasisException($"Unable to load type '{typeName}'");
                }

                loadAssembly:
                var a = LoadAssembly(assemblyName);

                if (a != null)
                {
                    type = a.GetType(typeName, false);

                    if (type != null) { return type; }
                }

                throw new BasisException($"Unable to load type '{typeName}'");
            });
        }
        
        public static Type GetTypeFromName(string typeName)
        {
            return GetTypeFromName(typeName, null);
        }

        public static Assembly LoadAssembly(string assemblyName)
        {
            Assembly assembly = null;
            try
            {
                assembly = Assembly.Load(assemblyName);
            }
            catch { }

            if (assembly != null) { return assembly; }

            return File.Exists(assemblyName) ? Assembly.LoadFrom(assemblyName) : null;
        }
    }
}