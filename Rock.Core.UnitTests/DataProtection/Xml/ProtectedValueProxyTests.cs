using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Xml.Serialization;
using NUnit.Framework;
using Rock.DataProtection.Xml;
using Rock.DataProtection;

namespace Rock.Core.UnitTests.DataProtection.Xml
{
    public class ProtectedValueProxyTests
    {
        [Test]
        public void TheDefaultTypeIsUnprotected()
        {
            var value = Encoding.UTF8.GetBytes("Hello, world!");
            var xml = string.Format("<Foo><Bar value=\"{0}\" /></Foo>", Convert.ToBase64String(value));
            var foo = (Foo)new XmlSerializer(typeof(Foo)).Deserialize(new StringReader(xml));

            var bar = foo.Bar.CreateInstance();
            
            Assert.That(bar, Is.InstanceOf<UnprotectedValue>());
            Assert.That(bar.GetValue(), Is.EqualTo(value));
        }

        [Test]
        public void CanSpecifyTextValueForUnprotectedValue()
        {
            var value = "Hello, world!";
            var xml = string.Format("<Foo><Bar text=\"{0}\" /></Foo>", value);
            var foo = (Foo)new XmlSerializer(typeof(Foo)).Deserialize(new StringReader(xml));

            var bar = foo.Bar.CreateInstance();

            Assert.That(bar, Is.InstanceOf<UnprotectedValue>());
            Assert.That(bar.GetValue(), Is.EqualTo(Encoding.UTF8.GetBytes(value)));
        }

        [Test]
        public void DPAPIProtectedValueCanBeSpecified()
        {
            var value = Encoding.UTF8.GetBytes("Hello, world!");
            var optionalEntropy = new byte[] { 1, 2, 3, 4, 5, 6 };
            var encryptedData = ProtectedData.Protect(value, optionalEntropy, DataProtectionScope.CurrentUser);
            var xml = string.Format(
@"<Foo>
  <Bar type=""Rock.DataProtection.Xml.DPAPIProtectedValue, Rock.Core""
       encryptedData=""{0}""
       optionalEntropy=""{1}""
       scope=""CurrentUser"" />
</Foo>",
                Convert.ToBase64String(encryptedData),
                Convert.ToBase64String(optionalEntropy));
            var foo = (Foo)new XmlSerializer(typeof(Foo)).Deserialize(new StringReader(xml));
            
            var bar = foo.Bar.CreateInstance();
            
            Assert.That(bar, Is.InstanceOf<DPAPIProtectedValue>());
            Assert.That(bar.GetValue(), Is.EqualTo(value));
        }

        public class Foo
        {
            public ProtectedValueProxy Bar { get; set; }
        }
    }
}