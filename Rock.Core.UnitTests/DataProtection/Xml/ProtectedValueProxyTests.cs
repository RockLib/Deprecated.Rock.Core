using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Xml.Serialization;
using NUnit.Framework;
using Rock.DataProtection.Xml;
using Rock.Serialization;

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

        [Test]
        public void CanNestInsideAnotherProxyValue()
        {
            var xml = @"<Qux>
  <Baz type='Rock.Core.UnitTests.DataProtection.Xml.Baz, Rock.Core.UnitTests'>
    <Bar type='Rock.DataProtection.Xml.UnprotectedValue, Rock.Core'
         value='SGVsbG8sIHdvcmxkIQ==' />
  </Baz>
</Qux>";
            var serializer = new XmlSerializer(typeof(Qux));
            var qux = (Qux)serializer.Deserialize(new StringReader(xml));
            var baz = qux.Baz.CreateInstance();
            Assert.That(() => baz.Bar.CreateInstance(), Throws.Nothing);
        }
    }

    public class Foo
    {
        public ProtectedValueProxy Bar { get; set; }
    }

    public interface IBaz
    {
        ProtectedValueProxy Bar { get; }
    }

    public class Baz : IBaz
    {
        public ProtectedValueProxy Bar { get; set; }
    }

    public class Qux
    {
        public XmlDeserializationProxy<IBaz> Baz { get; set; }
    }
}