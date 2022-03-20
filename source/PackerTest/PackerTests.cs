using NUnit.Framework;
using System.IO;
using System.Text;

namespace PackerTest
{
    public class PackerTests
    {
        [SetUp]
        public void Setup()
        {
        }

        [TestCase("Tests/all")]
        [TestCase("Tests/indexOrder")]
        [TestCase("Tests/lighter")]
        [TestCase("Tests/mixed")]
        [TestCase("Tests/zero")]
        public void Packer_Test(string file)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var line in File.ReadAllLines($"{file}_output"))
            {
                sb.AppendLine(line);
            }

            string result = Packer.Packer.Pack($"{file}_input");
            string expected = sb.ToString();

            Assert.AreEqual(expected, result); 
        }
    }
}