using System.Dynamic;
using Rock.Defaults;
using Rock.Defaults.Implementation;

namespace Rock.Conversion
{
    public static class ToExpandoObjectExtension
    {
        [UsesDefaultValue(typeof(Default), "ObjectToExpandoObjectConverter")]
        public static ExpandoObject ToExpandoObject(this object obj)
        {
            return Default.ObjectToExpandoObjectConverter.Convert(obj);
        }
    }
}
