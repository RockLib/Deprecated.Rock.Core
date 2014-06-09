using System;
using Rock.Defaults;
using Rock.Defaults.Implementation;

namespace Rock.Serialization
{
    public static class ToXmlExtension
    {
        [UsesDefaultValue(typeof(Default), "XmlSerializer")]
        public static string ToXml(this object item)
        {
            if (item == null)
            {
                throw new ArgumentNullException("item");
            }

            return Default.XmlSerializer.SerializeToString(item, item.GetType());
        }
    }
}