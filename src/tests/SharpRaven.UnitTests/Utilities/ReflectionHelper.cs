#region Disclaimer/Info

///////////////////////////////////////////////////////////////////////////////////////////////////
// Subtext WebLog
// 
// Subtext is an open source weblog system that is a fork of the .TEXT
// weblog system.
//
// For updated news and information please visit http://subtextproject.com/
// Subtext is hosted at Google Code at http://code.google.com/p/subtext/
// The development mailing list is at subtext@googlegroups.com 
//
// This project is licensed under the BSD license.  See the License.txt file for more information.
///////////////////////////////////////////////////////////////////////////////////////////////////

#endregion

using System;
using System.Reflection;

namespace SharpRaven.UnitTests.Utilities
{
    /// <summary>
    /// Helper class to simplify common reflection tasks.
    /// </summary>
    public static class ReflectionHelper
    {
        /// <summary>
        /// Returns the value of the private member specified.
        /// </summary>
        /// <param name="fieldName">Name of the member.</param>
        /// /// <param name="type">Type of the member.</param>
        public static T GetStaticFieldValue<T>(string fieldName, Type type)
        {
            FieldInfo field = type.GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Static);
            if (field != null)
            {
                return (T)field.GetValue(type);
            }
            return default(T);
        }
        /// <summary>
        /// Returns the value of the private member specified.
        /// </summary>
        /// <param name="propertyName">Name of the member.</param>
        /// /// <param name="type">Type of the member.</param>
        public static T GetStaticPropertyValue<T>(string propertyName, Type type)
        {
            PropertyInfo property = type.GetProperty(propertyName, BindingFlags.NonPublic | BindingFlags.Static);
            if (property != null)
            {
                return (T)property.GetValue(type, null);
            }
            return default(T);
        }

        /// <summary>
        /// Returns the value of the private member specified.
        /// </summary>
        /// <param name="fieldName">Name of the member.</param>
        /// <param name="typeName"></param>
        public static T GetStaticFieldValue<T>(string fieldName, string typeName)
        {
            Type type = Type.GetType(typeName, true);
            FieldInfo field = type.GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Static);
            if (field != null)
            {
                return (T)field.GetValue(type);
            }
            return default(T);
        }

        /// <summary>
        /// Sets the value of the private static member.
        /// </summary>
        /// <param name="fieldName"></param>
        /// <param name="type"></param>
        /// <param name="value"></param>
        public static void SetStaticFieldValue<T>(string fieldName, Type type, T value)
        {
            FieldInfo field = type.GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Static);
            if (field == null)
            {
                throw new ArgumentException("Could not find static field " + fieldName);
            }

            field.SetValue(null, value);
        }

        /// <summary>
        /// Sets the value of the private static member.
        /// </summary>
        /// <param name="fieldName"></param>
        /// <param name="typeName"></param>
        /// <param name="value"></param>
        public static void SetStaticFieldValue<T>(string fieldName, string typeName, T value)
        {
            Type type = Type.GetType(typeName, true);
            FieldInfo field = type.GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Static);
            if (field == null)
            {
                throw new ArgumentException("Could not find static field " + fieldName);
            }

            field.SetValue(null, value);
        }

        /// <summary>
        /// Returns the value of the private member specified.
        /// </summary>
        /// <param name="fieldName">Name of the member.</param>
        /// <param name="source">The object that contains the member.</param>
        public static T GetPrivateInstanceFieldValue<T>(string fieldName, object source)
        {
            FieldInfo field = source.GetType().GetField(fieldName,
                                                        BindingFlags.GetField | BindingFlags.NonPublic |
                                                        BindingFlags.Instance);
            if (field != null)
            {
                return (T)field.GetValue(source);
            }
            return default(T);
        }

        /// <summary>
        /// Returns the value of the private member specified.
        /// </summary>
        /// <param name="memberName">Name of the member.</param>
        /// <param name="source">The object that contains the member.</param>
        /// <param name="value">The value to set the member to.</param>
        public static void SetPrivateInstanceFieldValue(string memberName, object source, object value)
        {
            FieldInfo field = source.GetType().GetField(memberName,
                                                        BindingFlags.GetField | BindingFlags.NonPublic |
                                                        BindingFlags.Instance);
            if (field == null)
            {
                throw new ArgumentException("Could not find instance field " + memberName);
            }

            field.SetValue(source, value);
        }

        public static object Instantiate(string typeName)
        {
            return Instantiate(typeName, null, null);
        }

        public static object Instantiate(string typeName, Type[] constructorArgumentTypes,
                                         params object[] constructorParameterValues)
        {
            return Instantiate(Type.GetType(typeName, true), constructorArgumentTypes, constructorParameterValues);
        }

        public static object Instantiate(Type type, Type[] constructorArgumentTypes,
                                         params object[] constructorParameterValues)
        {
            ConstructorInfo constructor = type.GetConstructor(BindingFlags.NonPublic | BindingFlags.Instance, null,
                                                              constructorArgumentTypes, null);
            return constructor.Invoke(constructorParameterValues);
        }

        /// <summary>
        /// Invokes a non-public static method.
        /// </summary>
        /// <typeparam name="TReturn"></typeparam>
        /// <param name="type"></param>
        /// <param name="methodName"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public static TReturn InvokeNonPublicMethod<TReturn>(Type type, string methodName, params object[] parameters)
        {
            Type[] paramTypes = Array.ConvertAll(parameters, o => o.GetType());

            MethodInfo method = type.GetMethod(methodName, BindingFlags.NonPublic | BindingFlags.Static, null,
                                               paramTypes, null);
            if (method == null)
            {
                throw new ArgumentException("Could not find method " + methodName, "methodName");
            }

            return (TReturn)method.Invoke(null, parameters);
        }

        public static void InvokeNonPublicMethod(object source, string methodName, params object[] parameters)
        {
            Type[] paramTypes = Array.ConvertAll(parameters, o => o.GetType());

            MethodInfo method = source.GetType().GetMethod(methodName, BindingFlags.NonPublic | BindingFlags.Instance,
                                                           null, paramTypes, null);
            if (method == null)
            {
                throw new ArgumentException("Could not find method " + methodName, "methodName");
            }

            method.Invoke(source, parameters);
        }

        public static TReturn InvokeNonPublicMethod<TReturn>(object source, string methodName,
                                                             params object[] parameters)
        {
            Type[] paramTypes = Array.ConvertAll(parameters, o => o.GetType());

            MethodInfo method = source.GetType().GetMethod(methodName, BindingFlags.NonPublic | BindingFlags.Instance,
                                                           null, paramTypes, null);
            if (method == null)
            {
                throw new ArgumentException("Could not find method " + methodName, "methodName");
            }

            return (TReturn)method.Invoke(source, parameters);
        }

        public static TReturn InvokeProperty<TReturn>(object source, string propertyName)
        {
            PropertyInfo propertyInfo = source.GetType().GetProperty(propertyName);
            if (propertyInfo == null)
            {
                throw new ArgumentException("Could not find property " + propertyName, "propertyName");
            }

            return (TReturn)propertyInfo.GetValue(source, null);
        }

        public static TReturn InvokeNonPublicProperty<TReturn>(object source, string propertyName)
        {
            PropertyInfo propertyInfo = source.GetType().GetProperty(propertyName,
                                                                     BindingFlags.NonPublic | BindingFlags.Instance,
                                                                     null, typeof(TReturn), new Type[0], null);
            if (propertyInfo == null)
            {
                throw new ArgumentException("Could not find property " + propertyName, "propertyName");
            }

            return (TReturn)propertyInfo.GetValue(source, null);
        }

        public static object InvokeNonPublicProperty(object source, string propertyName)
        {
            PropertyInfo propertyInfo = source.GetType().GetProperty(propertyName,
                                                                     BindingFlags.NonPublic | BindingFlags.Instance);
            if (propertyInfo == null)
            {
                throw new ArgumentException("Could not find property " + propertyName, "propertyName");
            }

            return propertyInfo.GetValue(source, null);
        }
    }
}