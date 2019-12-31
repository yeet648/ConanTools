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
    public class InputTests
    {
        [TestMethod()]
        public void Input_FileNotExist()
        {
            //check for input when file does not exist

            string[] args = { "-f", Path.GetRandomFileName() };
            int expected = 20;
            int result = MoonPhaseSettings.Main(args);
            Assert.AreEqual(expected, result);

        }

    }

    [TestClass()]
    public class MoonPhaseTests
    {
        //check for expected outputs for all moon phases
        [TestMethod()]
        public void NewMoon()
        {
            validateMoonPhaseOutputs("New Moon", DateTime.Parse("1/23/2020"), 1, 1, 1);
        }
        [TestMethod()]
        public void Waxing_Crescent_Moon()
        {
            validateMoonPhaseOutputs("Waxing Crescent Moon", DateTime.Parse("1/26/2020"), 1.5, 1.2, 0.9);
        }

        [TestMethod()]
        public void First_Quarter_Moon()
        {
            validateMoonPhaseOutputs("First Quarter Moon", DateTime.Parse("1/30/2020"), 3, 1.4, 0.8);
        }

        [TestMethod()]
        public void Waxing_Gibbous_Moon()
        {
            validateMoonPhaseOutputs("Waxing Gibbous Moon", DateTime.Parse("2/3/2020"), 4.5, 1.5, 0.7);
        }

        [TestMethod()]
        public void Full_Moon()
        {
            validateMoonPhaseOutputs("Full Moon", DateTime.Parse("2/8/2020"), 6, 2, 0.5);
        }

        [TestMethod()]
        public void Waning_Gibbous_Moon()
        {
            validateMoonPhaseOutputs("Waning Gibbous Moon", DateTime.Parse("2/10/2020"), 4.5, 1.5, 0.7);
        }
        [TestMethod()]
        public void Last_Quarter_Moon()
        {
            validateMoonPhaseOutputs("Last Quarter Moon", DateTime.Parse("2/14/2020"), 3, 1.4, 0.8);
        }
        [TestMethod()]
        public void Waning_Crescent_Moon()
        {
            validateMoonPhaseOutputs("Waning Crescent Moon", DateTime.Parse("2/17/2020"), 1.5, 1.2, 0.9);
        }


        public void validateMoonPhaseOutputs (string expectedMoonPhase, DateTime knownNewMoonPhase, double expectedHarvestMultiplier, double expectedNPCDamageMultiplier, double expectedNPCDamageTakenMultiplier)
        {
            MoonPhaseSettings.Harvest result = new MoonPhaseSettings.Harvest(knownNewMoonPhase);

            Assert.AreEqual(expectedHarvestMultiplier, result.HarvestMultiplier);
            Assert.AreEqual(expectedNPCDamageMultiplier, result.NPCDamageMultiplier);
            Assert.AreEqual(expectedNPCDamageTakenMultiplier, result.NPCDamageTakenMultiplier);
            Assert.AreEqual(expectedMoonPhase, result.MoonPhase);

        }

    }

    
}