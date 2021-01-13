using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTestProject1
{
    [TestClass]
    public class NTPClientTest
    {
        [TestMethod]
        public void TestMethod1()
        {
            string timeServer;
            //var timeServer = "192.168.2.198";
            //timeServer = "time.windows.com";
            timeServer = "192.168.2.198";
            var iPAddress = System.Net.IPAddress.Parse(timeServer);
            Tool.NTPClient nTPClient = new Tool.NTPClient(iPAddress);
            nTPClient.Connect(true);

            Console.ReadLine();
        }
    }
}
