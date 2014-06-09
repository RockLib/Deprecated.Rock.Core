using System;
using Rock.Defaults;
using Rock.Defaults.Implementation;

namespace Rock.Serialization
{
    public static class ToJsonExtension
    {
        [UsesDefaultValue(typeof(Default), "JsonSerializer")]
        public static string ToJson(this object item)
        {
            if (item == null)
            {
                throw new ArgumentNullException("item");
            }

            return Default.JsonSerializer.SerializeToString(item, item.GetType());
        }
    }
}
