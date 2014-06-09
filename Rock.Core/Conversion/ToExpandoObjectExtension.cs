using System.Dynamic;
using Rock.Defaults;
using Rock.Defaults.Implementation;

namespace Rock.Conversion
{
    public static class ToExpandoObjectExtension
    {
        [UsesDefaultValue(typeof(Default), "ExpandoObjectConverter")]
        public static ExpandoObject ToExpandoObject(this object obj)
        {
            return Default.ExpandoObjectConverter.Convert(obj);
        }
    }
}
