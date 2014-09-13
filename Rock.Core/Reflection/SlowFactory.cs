using System;
using System.Linq;
using System.Reflection;

namespace Rock.Reflection
{
    /// <summary>
    /// Provides unoptimized mechanisms for creating instances of types.
    /// </summary>
    public static class SlowFactory
    {
        /// <summary>
        /// Creates an instance of the type represented by <paramref name="assemblyQualifiedType"/>,
        /// cast to <typeparamref name="T"/>. If a type cannot be located, if the type is not
        /// assignable to <typeparamref name="T"/>, or if the type does not have a suitable
        /// constructor, an exception is thrown. Suitable constructors include public
        /// parameterless constructors, and constructors whose parameters all have
        /// a default value.
        /// </summary>
        /// <typeparam name="T">The type of the object to return.</typeparam>
        /// <param name="assemblyQualifiedType">The assembly qualified name of the type to create.</param>
        /// <returns>An instance of the type represented by <paramref name="assemblyQualifiedType"/>.</returns>
        public static T CreateInstance<T>(string assemblyQualifiedType)
        {
            var type = Type.GetType(assemblyQualifiedType);

            if (type == null)
            {
                throw GetTypeNotFoundException(assemblyQualifiedType);
            }

            if (type.IsAbstract)
            {
                throw GetTypeIsAbstractException(type);
            }

            if (!typeof(T).IsAssignableFrom(type))
            {
                throw GetNotAssignableException<T>(type);
            }

            var constructors = type.GetConstructors();

            // Prefer default constructor...
            if (constructors.Any(IsDefaultConstructor))
            {
                return (T)Activator.CreateInstance(type);
            }

            // ...but a constructor whose parameters all have a default value is also acceptable.
            if (constructors.Any(AllParametersHaveDefaultValue))
            {
                // If there is more than one constructor whose parameters all have a default
                // value, we'll deterministically pick one by picking the one with the most
                // parameters. If there is more than one with the same number of parameters,
                // we'll order them by the string produced by joining all of the parameter's
                // type's full name, separating each with a ':' (colon) character.
                // 
                // (I don't know *why* someone would design their class with multiple
                // constructors whose parameters all have default values (it kinda defeats
                // the purpose of having default values when you do this), but we'll support
                // it nonetheless.)
                var ctor =
                    constructors
                        .Where(AllParametersHaveDefaultValue)
                        .OrderByDescending(c => c.GetParameters().Length)
                        .ThenBy(c => string.Join(":", c.GetParameters().Select(p => p.ParameterType.FullName)))
                        .First();

                var args = GetDefaultValueArgs(ctor.GetParameters());

                return (T)Activator.CreateInstance(type, args);
            }

            throw GetNoSuitableConstructorException(type);
        }

        private static bool IsDefaultConstructor(ConstructorInfo ctor)
        {
            return ctor.GetParameters().Length == 0;
        }

        public static bool AllParametersHaveDefaultValue(ConstructorInfo ctor)
        {
            return ctor.GetParameters().All(p => p.HasDefaultValue);
        }

        private static object[] GetDefaultValueArgs(ParameterInfo[] parameters)
        {
            var args = new object[parameters.Length];

            for (int i = 0; i < parameters.Length; i++)
            {
                args[i] = parameters[i].DefaultValue;
            }

            return args;
        }

        private static Exception GetTypeNotFoundException(string assemblyQualifiedType)
        {
            return new InvalidOperationException(
                string.Format(
                    "Unable to locate a type with the name of '{0}'. Use the assembly " +
                    "qualified name of the type in order to ensure that the type can be " +
                    "located.", assemblyQualifiedType));
        }

        private static Exception GetTypeIsAbstractException(Type type)
        {
            return new InvalidOperationException(
                string.Format("Unable to create instance of abstract type '{0}'.", type));
        }

        private static Exception GetNotAssignableException<T>(Type type)
        {
            return new InvalidOperationException(
                string.Format(
                    "The '{0}' type is not assignable to the '{1}' type.", type, typeof(T)));
        }

        private static Exception GetNoSuitableConstructorException(Type type)
        {
            return new InvalidOperationException(
                string.Format(
                    "Unable to find suitable constructor for '{0}'. There must be either " +
                    "a public parameterless constructor or a public constructor whose " +
                    "parameters all have default values.", type));
        }
    }
}
