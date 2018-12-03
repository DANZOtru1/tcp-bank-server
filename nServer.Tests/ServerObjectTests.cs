using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit;
using NUnit.Framework;

namespace nServer.Tests
{
    public class ServerObjectTests
    {
        [Test(Description ="Тест инициализации")]
        [Order(1)]
        public void InitialTest()
        {
            ServerObject serverObject = new ServerObject();
        }

        private static readonly object[] AddConnectionCases =
        {
            new object[]{new ClientObject(Guid.NewGuid().ToString(),new System.Net.Sockets.TcpClient(),  new ServerObject()) }
        };

        [Test(Description = "Тест добавления соединения")]
        [TestCaseSource(nameof(AddConnectionCases))]
        public void AddConnectionTest(ClientObject clientObject)
        {
            ServerObject serverObject = new ServerObject();
            serverObject.AddConnection(clientObject);
        }

        [TestCase(null)]
        [Test(Description = "Тест добавления соединения")]
        public void NegativeAddConnectionTest(ClientObject clientObject)
        {           
            ServerObject serverObject = new ServerObject();
            Assert.Throws<ArgumentNullException>(() => { serverObject.AddConnection(clientObject); });
        }

        [TestCase("wiuehfiwufiojh2323d")]
        [TestCase("ALUDSLKAUHIUWasodijoasd")]
        [TestCase("GGGGGGGGGGGGGGGGGG")]
        [TestCase("ASDwqiduweihied")]
        [TestCase("a")]
        [TestCase("wiueergergerg45g45g4weg5erhre")]
        [TestCase("wiuehfiwufiojh232asdf")]
        [Test(Description ="Тест на валидный id")]
        public void RemoveConnectionTest(string id)
        {
            ServerObject serverObject = new ServerObject();
            serverObject.RemoveConnection(id);
        }

        [TestCase("   ")]
        [TestCase("")]
        [TestCase(null)]
        [TestCase("   ")]
        [TestCase("       ")]
        [TestCase("              ")]
        [TestCase("                                ")]
        [Test(Description = "Тест на валидный id")]
        public void NegativeRemoveConnectionTest(string id)
        {
            ServerObject serverObject = new ServerObject();
            Assert.Throws<ArgumentOutOfRangeException>(() => { serverObject.RemoveConnection(id); });
        }

        private static readonly object[] SendMoneyCases =
        {
            new object[]{"wejidopwjed", decimal.MaxValue},
            new object[]{"fdsasdf", 542441234m},
            new object[]{"wsadfdsaf", 1234m},
            new object[]{"gg", 1231244444m}
        };

        [TestCaseSource(nameof(SendMoneyCases))]
        [Test(Description ="Тест отправки денег")]
        public void SendMoneyTest(string sendId, decimal sendSum)
        {
            ServerObject serverObject = new ServerObject();
            serverObject.SendMoney(sendId, sendSum);
        }

        private static readonly object[] NegativeSendMoneyCases =
        {
            new object[]{"wejidopwjed", -323.32m},
            new object[]{"fdsasdf", -123123123m},
            new object[]{"wsadfdsaf", -0.123123m},
            new object[]{"gg", decimal.MinValue}
        };

        [TestCaseSource(nameof(NegativeSendMoneyCases))]
        [Test(Description = "Тест отправки денег")]
        public void NegativeSendMoneyTest(string sendId, decimal sendSum)
        {
            ServerObject serverObject = new ServerObject();
            Assert.Throws<ArgumentOutOfRangeException>(() => { serverObject.SendMoney(sendId, sendSum); });
        }
    }
}
