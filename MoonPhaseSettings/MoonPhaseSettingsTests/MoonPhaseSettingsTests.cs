using Microsoft.VisualStudio.TestTools.UnitTesting;
using MoonPhaseSettings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace MoonPhaseSettings.Tests
{
    [TestClass()]
    public class MoonPhaseSettingsTests
    {
        [TestMethod()]
        public void Input_FileNotExist()
        {
            //check for input when fil does not exist
            string[] args = {"foo"};
            int expected = 20;
            int result =MoonPhaseSettings.Main(args);
            Assert.AreEqual(expected, result);
        }
    }
}