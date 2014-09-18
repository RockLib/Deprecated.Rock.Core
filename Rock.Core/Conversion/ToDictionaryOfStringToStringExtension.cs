using System.Collections.Generic;
using Rock.Defaults;
using Rock.Defaults.Implementation;

namespace Rock.Conversion
{
    public static class ToDictionaryOfStringToStringExtension
    {
        [UsesDefaultValue(typeof(Default), "ObjectToDictionaryOfStringToStringConverter")]
        public static IDictionary<string, string> ToDictionaryOfStringToString(this object obj)
        {
            return Default.ObjectToDictionaryOfStringToStringConverter.Convert(obj);
        }
    }
}
