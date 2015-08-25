#region License

// Copyright (c) 2014 The Sentry Team and individual contributors.
// All rights reserved.
// 
// Redistribution and use in source and binary forms, with or without modification, are permitted
// provided that the following conditions are met:
// 
//     1. Redistributions of source code must retain the above copyright notice, this list of
//        conditions and the following disclaimer.
// 
//     2. Redistributions in binary form must reproduce the above copyright notice, this list of
//        conditions and the following disclaimer in the documentation and/or other materials
//        provided with the distribution.
// 
//     3. Neither the name of the Sentry nor the names of its contributors may be used to
//        endorse or promote products derived from this software without specific prior written
//        permission.
// 
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY EXPRESS OR
// IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND
// FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL
// DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE,
// DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY,
// WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN
// ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.

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
        /// <param name="source">The object that contains the member.</param>
        public static T GetPrivateInstanceFieldValue<T>(string fieldName, object source)
        {
            FieldInfo field = source.GetType().GetField(fieldName,
                                                        BindingFlags.GetField | BindingFlags.NonPublic |
                                                        BindingFlags.Instance);
            if (field != null)
                return (T)field.GetValue(source);
            return default(T);
        }


        /// <summary>
        /// Returns the value of the private member specified.
        /// </summary>
        /// <param name="fieldName">Name of the member.</param>
        /// /// <param name="type">Type of the member.</param>
        public static T GetStaticFieldValue<T>(string fieldName, Type type)
        {
            FieldInfo field = type.GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Static);
            if (field != null)
                return (T)field.GetValue(type);
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
                return (T)field.GetValue(type);
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
                return (T)property.GetValue(type, null);
            return default(T);
        }


        public static object Instantiate(string typeName)
        {
            return Instantiate(typeName, null, null);
        }


        public static object Instantiate(string typeName,
                                         Type[] constructorArgumentTypes,
                                         params object[] constructorParameterValues)
        {
            return Instantiate(Type.GetType(typeName, true), constructorArgumentTypes, constructorParameterValues);
        }


        public static object Instantiate(Type type,
                                         Type[] constructorArgumentTypes,
                                         params object[] constructorParameterValues)
        {
            ConstructorInfo constructor = type.GetConstructor(BindingFlags.NonPublic | BindingFlags.Instance,
                                                              null,
                                                              constructorArgumentTypes,
                                                              null);
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

            MethodInfo method = type.GetMethod(methodName,
                                               BindingFlags.NonPublic | BindingFlags.Static,
                                               null,
                                               paramTypes,
                                               null);
            if (method == null)
                throw new ArgumentException("Could not find method " + methodName, "methodName");

            return (TReturn)method.Invoke(null, parameters);
        }


        public static void InvokeNonPublicMethod(object source, string methodName, params object[] parameters)
        {
            Type[] paramTypes = Array.ConvertAll(parameters, o => o.GetType());

            MethodInfo method = source.GetType().GetMethod(methodName,
                                                           BindingFlags.NonPublic | BindingFlags.Instance,
                                                           null,
                                                           paramTypes,
                                                           null);
            if (method == null)
                throw new ArgumentException("Could not find method " + methodName, "methodName");

            method.Invoke(source, parameters);
        }


        public static TReturn InvokeNonPublicMethod<TReturn>(object source,
                                                             string methodName,
                                                             params object[] parameters)
        {
            Type[] paramTypes = Array.ConvertAll(parameters, o => o.GetType());

            MethodInfo method = source.GetType().GetMethod(methodName,
                                                           BindingFlags.NonPublic | BindingFlags.Instance,
                                                           null,
                                                           paramTypes,
                                                           null);
            if (method == null)
                throw new ArgumentException("Could not find method " + methodName, "methodName");

            return (TReturn)method.Invoke(source, parameters);
        }


        public static TReturn InvokeNonPublicProperty<TReturn>(object source, string propertyName)
        {
            PropertyInfo propertyInfo = source.GetType().GetProperty(propertyName,
                                                                     BindingFlags.NonPublic | BindingFlags.Instance,
                                                                     null,
                                                                     typeof(TReturn),
                                                                     new Type[0],
                                                                     null);
            if (propertyInfo == null)
                throw new ArgumentException("Could not find property " + propertyName, "propertyName");

            return (TReturn)propertyInfo.GetValue(source, null);
        }


        public static object InvokeNonPublicProperty(object source, string propertyName)
        {
            PropertyInfo propertyInfo = source.GetType().GetProperty(propertyName,
                                                                     BindingFlags.NonPublic | BindingFlags.Instance);
            if (propertyInfo == null)
                throw new ArgumentException("Could not find property " + propertyName, "propertyName");

            return propertyInfo.GetValue(source, null);
        }


        public static TReturn InvokeProperty<TReturn>(object source, string propertyName)
        {
            PropertyInfo propertyInfo = source.GetType().GetProperty(propertyName);
            if (propertyInfo == null)
                throw new ArgumentException("Could not find property " + propertyName, "propertyName");

            return (TReturn)propertyInfo.GetValue(source, null);
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
                throw new ArgumentException("Could not find instance field " + memberName);

            field.SetValue(source, value);
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
                throw new ArgumentException("Could not find static field " + fieldName);

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
                throw new ArgumentException("Could not find static field " + fieldName);

            field.SetValue(null, value);
        }
    }
}