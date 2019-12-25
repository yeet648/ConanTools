using System;
using System.IO;
using System.Text.RegularExpressions;

namespace MoonPhaseSettings
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine(DateTime.UtcNow.ToShortDateString() + " " + DateTime.UtcNow.ToShortTimeString() + ":Running Moon Harvest Multiplier...");

            //check for required arguements or show syntax
            if (args.Length <= 0) { Syntax(); return; }



            //confirm required setting files exist
            string file = args[0];
            if (!File.Exists(file))
            {
                Console.WriteLine("ERROR: seeting file '" + file + "' does not exist.");
                return;
            }

            //get moon phase and set associated variables
            Harvest HarvestDate = new Harvest(DateTime.UtcNow);
            Console.WriteLine("Date: " + HarvestDate.HarvestDate.ToShortDateString() + "\t" + HarvestDate.HarvestMultiplier.ToString() + "x," + HarvestDate.MoonPhase);

            try
            {
                //update settings file
                Console.WriteLine(DateTime.UtcNow.ToShortDateString() + " " + DateTime.UtcNow.ToShortTimeString() + ": Updating setting file: " + file);
                string inText = File.ReadAllText(file);
                string outText = inText;
                //text = text.Replace("FOO", "BAR");
                outText = Regex.Replace(outText, "ServerMessageOfTheDay=.*", "ServerMessageOfTheDay=" + HarvestDate.MessageOfTheDay + Environment.NewLine);
                outText = Regex.Replace(outText, "HarvestAmountMultiplier=.*", "HarvestAmountMultiplier=" + HarvestDate.HarvestMultiplier + Environment.NewLine);

                File.WriteAllText(file, outText);
            }
            catch (Exception e)
            {
                Console.WriteLine("ERROR: " + e.Message);
                return;
            }

        }

        private static void Syntax()
        {
            Console.WriteLine();
            Console.WriteLine("Use to update Conan Exiles settings based on real world lunar phase.");
            Console.WriteLine("For instance during a real world full moon the harvest multiplier could be increased ");
            Console.WriteLine("and during new moon the harvest multiplier could be decreased.");
            Console.WriteLine();
            Console.WriteLine("Usage: " + System.AppDomain.CurrentDomain.FriendlyName + ".exe [path to Conan Exiles ServerSettings.ini]");
            Console.WriteLine();
            Console.WriteLine("Example: " + System.AppDomain.CurrentDomain.FriendlyName + ".exe c:\\Exiles\\ConanSandbox\\Saved\\Config\\WindowsServer\\ServerSettings.ini");

        }

        public class Harvest
        {

            public DateTime HarvestDate { get; set; }
            public double HarvestMultiplier { get; set; }
            public string MessageOfTheDay { get; set; }
            public int MoonPhaseCode { get; set; }
            public string MoonPhase { get; set; }

            public Harvest(DateTime harvestDate)
            {
                HarvestDate = harvestDate;
                MoonPhaseCode = this.GetMoonPhase(harvestDate);
                HarvestMultiplier = GetHarvestMultiplier(this.GetMoonPhase(harvestDate));

            }

            private double GetHarvestMultiplier(int moonPhase)
            {
                /*
                 New Moon 1 (0, 29)
                 Waxing Crescent 1.5 (1-5)
                 First Quarter 3 (6-9)
                 Waxing Gibbous 4.5 (10-14)
                 Full 6 (15,16)
                 Waning Gibbous 4.5 (17-20)
                 Last Quarter 3 (21,22,23)
                 Waning Crescent 1.5 (24-28)
                 */
                double multiplier = 1;
                this.MoonPhase = "";
                this.MessageOfTheDay = "";
                //new moon
                if (moonPhase == 0 || moonPhase == 29)
                {
                    multiplier = 1;
                    this.MoonPhase = "New Moon";
                }

                //waxing crescent
                if (1 <= moonPhase && moonPhase <= 5)
                {
                    multiplier = 1.5;
                    this.MoonPhase = "Waxing Crescent Moon";
                }

                //first quarter
                if (6 <= moonPhase && moonPhase <= 9)
                {
                    multiplier = 3;
                    this.MoonPhase = "First Quarter Moon";
                }
                //waxing gibbous
                if (10 <= moonPhase && moonPhase <= 14)
                {
                    multiplier = 4.5;
                    this.MoonPhase = "Waxing Gibbous Moon";
                }
                //full
                if (15 <= moonPhase && moonPhase <= 16)
                {
                    multiplier = 6;
                    this.MoonPhase = "Full Moon";
                }
                //waning gibbous
                if (17 <= moonPhase && moonPhase <= 20)
                {
                    multiplier = 4.5;
                    this.MoonPhase = "Waning Gibbous Moon";
                }
                //last quarter
                if (21 <= moonPhase && moonPhase <= 23)
                {
                    multiplier = 3;
                    this.MoonPhase = "Last Quarter Moon";
                }
                //waning crescent
                if (24 <= moonPhase && moonPhase <= 28)
                {
                    multiplier = 1.5;
                    this.MoonPhase = "Waning Crescent Moon";
                }

                this.MessageOfTheDay = this.MoonPhase + ": " + multiplier.ToString() + "x harvest multiplier";


                return multiplier;
            }

            private int GetMoonPhase(DateTime dt)
            {
                //thank you http://ben-daglish.net/moon.shtml
                //returns 0 for new moon 15 for full moon
                int year, month, day;
                year = dt.Year;
                month = dt.Month;
                day = dt.Day;
                int r = year % 100;
                r %= 19;
                if (r > 9) { r -= 19; }
                r = ((r * 11) % 30) + month + day;
                if (month < 3) { r += 2; }
                r -= (int)((year < 2000) ? 4 : 8.3);
                r = (int)Math.Floor(r + 0.5) % 30;

                return (r < 0) ? r + 30 : r;

            }

        }

    }
}
