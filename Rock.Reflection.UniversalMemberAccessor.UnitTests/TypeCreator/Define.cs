using System;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace Rock.Reflection.UnitTests.TypeCreator
{
    public static class Define
    {
        public static ConstructorDefinition Constructor(params Parameter[] parameters)
        {
            return Constructor(Visibility.Public, parameters);
        }

        public static ConstructorDefinition Constructor(Visibility visibility = Visibility.Public, params Parameter[] parameters)
        {
            parameters = parameters ?? new Parameter[0];

            var methodAttributes = GetMethodAttributes(visibility, true);

            return new ConstructorDefinition((tb, fields) =>
            {
                var constructorBuilder = tb.DefineConstructor(methodAttributes, CallingConventions.Standard, parameters.Select(p => p.Type).ToArray());

                for (int i = 0; i < parameters.Length; i++)
                {
                    if (parameters[i].HasDefaultValue)
                    {
                        var parameter = constructorBuilder.DefineParameter(i + 1, ParameterAttributes.HasDefault | ParameterAttributes.Optional, null);
                        parameter.SetConstant(parameters[i].DefaultValue);
                    }
                }

                var il = constructorBuilder.GetILGenerator();

                for (int i = 0; i < parameters.Length; i++)
                {
                    var parameter = parameters[i];

                    if (parameter.FieldToSet != null)
                    {
                        var fieldBuilder = fields.SingleOrDefault(f => f.Name == parameter.FieldToSet);

                        if (fieldBuilder != null)
                        {
                            if (fieldBuilder.IsStatic)
                            {
                                il.Emit(OpCodes.Ldarg, i);
                                il.Emit(OpCodes.Stsfld, fieldBuilder);
                            }
                            else
                            {
                                il.Emit(OpCodes.Ldarg_0);
                                il.Emit(OpCodes.Ldarg, i + 1);
                                il.Emit(OpCodes.Stfld, fieldBuilder);
                            }
                        }
                    }
                }

                il.Emit(OpCodes.Ldstr, tb.Name + ".ctor(" + string.Join(", ", parameters.Select(p => p.Type.Name)) + ")");
                il.Emit(OpCodes.Call, typeof(Console).GetMethod("WriteLine", new[] { typeof(string) }));

                il.Emit(OpCodes.Ret);
            });
        }

        public static MemberDefinition Method(string name, params Parameter[] parameters)
        {
            return Method(name, false, parameters:parameters);
        }

        public static MemberDefinition Method(string name, bool isStatic = false, Visibility visibility = Visibility.Public, params Parameter[] parameters)
        {
            parameters = parameters ?? new Parameter[0];

            var methodAttributes = GetMethodAttributes(visibility, false, isStatic);

            return new ConstructorDefinition((tb, fields) =>
            {
                var methodBuilder = tb.DefineMethod(name, methodAttributes, typeof(void), parameters.Select(p => p.Type).ToArray());

                for (int i = 0; i < parameters.Length; i++)
                {
                    if (parameters[i].HasDefaultValue)
                    {
                        var parameter = methodBuilder.DefineParameter(i + 1, ParameterAttributes.HasDefault | ParameterAttributes.Optional, null);
                        parameter.SetConstant(parameters[i].DefaultValue);
                    }
                }

                var il = methodBuilder.GetILGenerator();

                for (int i = 0; i < parameters.Length; i++)
                {
                    var parameter = parameters[i];

                    if (parameter.FieldToSet != null)
                    {
                        var fieldBuilder = fields.SingleOrDefault(f => f.Name == parameter.FieldToSet);

                        if (fieldBuilder != null)
                        {
                            if (fieldBuilder.IsStatic)
                            {
                                il.Emit(OpCodes.Ldarg, i);
                                il.Emit(OpCodes.Stsfld, fieldBuilder);
                            }
                            else
                            {
                                il.Emit(OpCodes.Ldarg_0);
                                il.Emit(OpCodes.Ldarg, i + 1);
                                il.Emit(OpCodes.Stfld, fieldBuilder);
                            }
                        }
                    }
                }

                il.Emit(OpCodes.Ldstr, "void " + tb.Name + "." + name + "(" + string.Join(", ", parameters.Select(p => p.Type.Name)) + ")");
                il.Emit(OpCodes.Call, typeof(Console).GetMethod("WriteLine", new[] { typeof(string) }));

                il.Emit(OpCodes.Ret);
            });
        }

        public static MemberDefinition EchoMethod(string name, Type type, bool isStatic = false, Visibility visibility = Visibility.Public)
        {
            if (name == null) throw new ArgumentNullException("name");
            if (type == null) throw new ArgumentNullException("type");

            var methodAttributes = GetMethodAttributes(visibility, false, isStatic);

            return new MemberDefinition((tb, fields) =>
            {
                var methodBuilder = tb.DefineMethod(name, methodAttributes, type, new[] { type });
                var il = methodBuilder.GetILGenerator();

                il.Emit(OpCodes.Ldstr, type.Name + " " + tb.Name + "." + name + "(" + type.Name + ")");
                il.Emit(OpCodes.Call, typeof(Console).GetMethod("WriteLine", new[] { typeof(string) }));

                il.Emit(isStatic ? OpCodes.Ldarg_0 : OpCodes.Ldarg_1);

                il.Emit(OpCodes.Ret);
            });
        }

        public static MemberDefinition EchoRefMethod(string name, Type type, bool returnsVoid = false, bool isStatic = false, Visibility visibility = Visibility.Public)
        {
            return EchoByRefMethod(name, type, false, returnsVoid, isStatic, visibility);
        }

        public static MemberDefinition EchoOutMethod(string name, Type type, bool returnsVoid = false, bool isStatic = false, Visibility visibility = Visibility.Public)
        {
            return EchoByRefMethod(name, type, true, returnsVoid, isStatic, visibility);
        }

        private static MemberDefinition EchoByRefMethod(string name, Type type, bool hasOutParameter, bool returnsVoid, bool isStatic, Visibility visibility)
        {
            if (name == null) throw new ArgumentNullException("name");
            if (type == null) throw new ArgumentNullException("type");

            var methodAttributes = GetMethodAttributes(visibility, false, isStatic);
            
            return new MemberDefinition((tb, fields) =>
            {
                var methodBuilder = tb.DefineMethod(name, methodAttributes, returnsVoid ? typeof(void) : type, new[] { type, type.MakeByRefType() });

                if (hasOutParameter)
                {
                    methodBuilder.DefineParameter(2, ParameterAttributes.Out, null);
                }

                var il = methodBuilder.GetILGenerator();

                if (isStatic)
                {
                    il.Emit(OpCodes.Ldarg_1);
                    il.Emit(OpCodes.Ldarg_0);
                }
                else
                {
                    il.Emit(OpCodes.Ldarg_2);
                    il.Emit(OpCodes.Ldarg_1);
                }

                if (type.IsValueType)
                {
                    il.Emit(OpCodes.Stobj, type);
                }
                else
                {
                    il.Emit(OpCodes.Stind_Ref);
                }

                if (!returnsVoid)
                {
                    il.Emit(isStatic ? OpCodes.Ldarg_0 : OpCodes.Ldarg_1);
                }

                il.Emit(OpCodes.Ret);
            });
        }

        public static MemberDefinition AutoProperty(string name, Type type, bool isStatic = false, Visibility visibility = Visibility.Public, string backingFieldName = null)
        {
            if (name == null) throw new ArgumentNullException("name");
            if (type == null) throw new ArgumentNullException("type");

            backingFieldName = backingFieldName ?? "<" + name + ">k__BackingField";

            var methodAttributes = GetMethodAttributes(Visibility.Public, false, isStatic)
                | MethodAttributes.SpecialName;
            var fieldAttributes = GetFieldAttributes(visibility, isStatic, false);

            return new MemberDefinition((tb, fields) =>
            {
                var fieldBuilder = tb.DefineField(backingFieldName, type, fieldAttributes);
                fields.Add(fieldBuilder);
                var propertyBuilder = tb.DefineProperty(name, PropertyAttributes.HasDefault, type, null);

                var getMethodBuilder = GetGetMethodBuilder(name, type, isStatic, tb, methodAttributes, fieldBuilder);
                var setMethodBuilder = GetSetMethodBuilder(name, type, isStatic, tb, methodAttributes, fieldBuilder);

                propertyBuilder.SetGetMethod(getMethodBuilder);
                propertyBuilder.SetSetMethod(setMethodBuilder);
            });
        }

        public static MemberDefinition ReadOnlyProperty(string name, Type type, bool isStatic = false, Visibility visibility = Visibility.Public, string backingFieldName = null)
        {
            if (name == null) throw new ArgumentNullException("name");
            if (type == null) throw new ArgumentNullException("type");

            backingFieldName = backingFieldName ?? "<" + name + ">k__BackingField";

            var fieldAttributes = GetFieldAttributes(Visibility.Private, isStatic, true);
            var methodAttributes = GetMethodAttributes(Visibility.Public, false, isStatic);

            return new MemberDefinition((tb, fields) =>
            {
                var fieldBuilder = tb.DefineField(backingFieldName, type, fieldAttributes);
                fields.Add(fieldBuilder);
                var propertyBuilder = tb.DefineProperty(name, PropertyAttributes.None, type, null);

                var getMethodBuilder = GetGetMethodBuilder(name, type, isStatic, tb, methodAttributes, fieldBuilder);

                propertyBuilder.SetGetMethod(getMethodBuilder);
            });
        }

        public static MemberDefinition WriteOnlyProperty(string name, Type type, bool isStatic = false, Visibility visibility = Visibility.Public, string backingFieldName = null)
        {
            if (name == null) throw new ArgumentNullException("name");
            if (type == null) throw new ArgumentNullException("type");

            backingFieldName = backingFieldName ?? "<" + name + ">k__BackingField";

            var fieldAttributes = GetFieldAttributes(Visibility.Private, isStatic, false);
            var methodAttributes = GetMethodAttributes(Visibility.Public, false, isStatic);

            return new MemberDefinition((tb, fields) =>
            {
                var fieldBuilder = tb.DefineField(backingFieldName, type, fieldAttributes);
                fields.Add(fieldBuilder);
                var propertyBuilder = tb.DefineProperty(name, PropertyAttributes.HasDefault, type, null);

                var setMethodBuilder = GetSetMethodBuilder(name, type, isStatic, tb, methodAttributes, fieldBuilder);

                propertyBuilder.SetSetMethod(setMethodBuilder);
            });
        }

        private static MethodBuilder GetGetMethodBuilder(string name, Type type, bool isStatic,
            TypeBuilder tb, MethodAttributes methodAttributes, FieldBuilder fieldBuilder)
        {
            var getMethodBuilder = tb.DefineMethod("get_" + name,
                methodAttributes, type, Type.EmptyTypes);
            var il = getMethodBuilder.GetILGenerator();

            il.Emit(OpCodes.Ldstr, type.Name + " " + tb.Name + ".get_" + name + "()");
            il.Emit(OpCodes.Call, typeof(Console).GetMethod("WriteLine", new[] { typeof(string) }));

            if (isStatic)
            {
                il.Emit(OpCodes.Ldsfld, fieldBuilder);
            }
            else
            {
                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Ldfld, fieldBuilder);
            }

            il.Emit(OpCodes.Ret);
            return getMethodBuilder;
        }

        private static MethodBuilder GetSetMethodBuilder(string name, Type type, bool isStatic,
            TypeBuilder tb, MethodAttributes methodAttributes, FieldBuilder fieldBuilder)
        {
            var setMethodBuilder = tb.DefineMethod("set_" + name,
                methodAttributes, null, new[] { type });
            var il = setMethodBuilder.GetILGenerator();

            il.Emit(OpCodes.Ldstr, "void " + tb.Name + ".set_" + name + "(" + type.Name + ")");
            il.Emit(OpCodes.Call, typeof(Console).GetMethod("WriteLine", new[] { typeof(string) }));

            if (isStatic)
            {
                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Stsfld, fieldBuilder);
            }
            else
            {
                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Ldarg_1);
                il.Emit(OpCodes.Stfld, fieldBuilder);
            }

            il.Emit(OpCodes.Ret);
            return setMethodBuilder;
        }

        public static MemberDefinition Field(string name, Type type, bool isStatic = false, bool isReadOnly = false, Visibility visibility = Visibility.Private)
        {
            if (name == null) throw new ArgumentNullException("name");
            if (type == null) throw new ArgumentNullException("type");

            var fieldAttributes = GetFieldAttributes(visibility, isStatic, isReadOnly);

            return new MemberDefinition((tb, fields) => fields.Add(tb.DefineField(name, type, fieldAttributes)));
        }

        private static MethodAttributes GetMethodAttributes(Visibility visibility, bool isConstructor, bool isStatic = false)
        {
            MethodAttributes methodAttributes;

            switch (visibility)
            {
                case Visibility.Private:
                    methodAttributes = MethodAttributes.Private;
                    break;
                case Visibility.Protected:
                    methodAttributes = MethodAttributes.Family;
                    break;
                case Visibility.Internal:
                    methodAttributes = MethodAttributes.Assembly;
                    break;
                case Visibility.Public:
                    methodAttributes = MethodAttributes.Public;
                    break;
                default:
                    throw new ArgumentOutOfRangeException("visibility");
            }

            if (!isConstructor)
            {
                methodAttributes |= MethodAttributes.HideBySig | (isStatic ? MethodAttributes.Static : MethodAttributes.Virtual);
            }

            return methodAttributes;
        }

        private static FieldAttributes GetFieldAttributes(Visibility visibility, bool isStatic, bool isReadOnly)
        {
            FieldAttributes fieldAttributes;

            switch (visibility)
            {
                case Visibility.Private:
                    fieldAttributes = FieldAttributes.Private;
                    break;
                case Visibility.Protected:
                    fieldAttributes = FieldAttributes.Family;
                    break;
                case Visibility.Internal:
                    fieldAttributes = FieldAttributes.Assembly;
                    break;
                case Visibility.Public:
                    fieldAttributes = FieldAttributes.Public;
                    break;
                default:
                    throw new ArgumentOutOfRangeException("visibility");
            }

            if (isStatic)
            {
                fieldAttributes |= FieldAttributes.Static;
            }

            if (isReadOnly)
            {
                fieldAttributes |= FieldAttributes.InitOnly;
            }

            return fieldAttributes;
        }
    }
}