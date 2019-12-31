using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.RegularExpressions;
using Mono.Options;

namespace MoonPhaseSettings
{
    public class MoonPhaseSettings
    {
        
        public static int Main(string[] args)
        {
            #region parse syntax
            // these variables will be set when the command line is parsed
            string filePath = null;
            //options
            var verbosity = 0;
            
            //default to 0 so we always get at least todays moon phase settings. 
            int days = 0;

            //default to UtC now or else use what is passed in
            DateTime targetDate = DateTime.UtcNow;
            bool shouldShowHelp = false;
            if (args.Length == 0) { shouldShowHelp = true; }

            // these are the available options, not that they set the variables
            OptionSet options = new OptionSet {
                { "f|file=", "the path to settings file", n => filePath = n },
                { "d|days=", "number of days to preview moon phases", (int d) => days = d },
                { "dt|date=", "specific date to show moon phase", (DateTime dt) => targetDate = dt },
                { "v", "increase debug message verbosity", v => { if (v != null) ++verbosity; } },
                { "h|help", "show this message and exit", h => shouldShowHelp = h != null },
            };
            List<string> extra;

            try
            {
                // parse the command line
                extra = options.Parse(args);
            }
            catch (OptionException e)
            {
                // parse error so give help message
                Console.Write("ERROR: ");
                Console.WriteLine(e.Message);
                Console.WriteLine("Try '" + System.AppDomain.CurrentDomain.FriendlyName + ".exe --help' for more information.");
                return 5;
            }

            #endregion
            if (shouldShowHelp)
            {
                return (Syntax(options));
            }

            //start
            Console.WriteLine(DateTime.UtcNow.ToShortDateString() + " " + DateTime.UtcNow.ToShortTimeString() + ":Running Moon Harvest Multiplier...");
            Harvest HarvestDate = null;


            //preview moon changes based on number of days or a specific date

            if (days != 0)
            {
                filePath = null;   //disable filepath so we don't update the settings file when moon phase preview is enabled 
                for (int count = 0; count <= days; count++)
                {
                    //get moon phase and set associated variables
                    Console.WriteLine();
                    HarvestDate = new Harvest(targetDate.AddDays(count));
                    showSettings(HarvestDate);
                }
                return 15;
            }
            //if file path was provided then update file with new settings
            if (filePath != null)
            {
                //confirm required setting files exist
                if (!File.Exists(filePath))
                {
                    Console.WriteLine("ERROR: setting file '" + filePath + "' does not exist.");
                    return 20;
                }

                try
                {
                    HarvestDate = new Harvest(targetDate);
                    showSettings(HarvestDate);
                    //update settings file
                    Console.WriteLine(DateTime.UtcNow.ToShortDateString() + " " + DateTime.UtcNow.ToShortTimeString() + ": Updating setting file: " + filePath);
                    string inText = File.ReadAllText(filePath);
                    string outText = inText;
                    outText = updateSettings(outText, @"ServerMessageOfTheDay=.*\n", "ServerMessageOfTheDay=" + HarvestDate.MessageOfTheDay + Environment.NewLine);
                    outText = updateSettings(outText, @"HarvestAmountMultiplier=.*\n", "HarvestAmountMultiplier=" + HarvestDate.HarvestMultiplier + Environment.NewLine);
                    outText = updateSettings(outText, @"NPCDamageMultiplier=.*\n", "NPCDamageMultiplier=" + HarvestDate.NPCDamageMultiplier + Environment.NewLine);
                    outText = updateSettings(outText, @"NPCDamageTakenMultiplier=.*\n", "NPCDamageTakenMultiplier=" + HarvestDate.NPCDamageTakenMultiplier + Environment.NewLine);
                    File.WriteAllText(filePath, outText);
                }
                catch (Exception e)
                {
                    Console.WriteLine("ERROR: updating settings file " + e.Message);
                    return 30;
                }
            }
            return 0;

        }

        private static string updateSettings(string inText, string searchPattern, string replacePattern)
        {
            string outText = inText;

            Regex regex = new Regex(searchPattern);
            Match match = regex.Match(inText);
            if (match.Success)
            {
                Console.WriteLine("UPDATING VALUE: " + replacePattern);
                outText = Regex.Replace(inText, searchPattern, replacePattern);
            }
            else
            {
                Console.WriteLine("ADDING VALUE: " + replacePattern);
                outText = inText + replacePattern;
            }

            return outText;
        }

        private static void showSettings(Harvest HarvestDate)
        {
            Console.WriteLine("Date: " + HarvestDate.HarvestDate.ToShortDateString() + "\t" + HarvestDate.HarvestMultiplier.ToString() + "x," + HarvestDate.MoonPhase);
            Console.WriteLine("ServerMessageOfTheDay= " + HarvestDate.MessageOfTheDay);
            Console.WriteLine("HarvestAmountMultiplier=" + HarvestDate.HarvestMultiplier);
            Console.WriteLine("NPCDamageMultiplier=" + HarvestDate.NPCDamageMultiplier);
            Console.WriteLine("NPCDamageTakenMultiplier=" + HarvestDate.NPCDamageTakenMultiplier);
        }

        private static int Syntax(OptionSet optional)
        {
            // show some app description message
            Console.WriteLine();
            Console.WriteLine("Use to update Conan Exiles settings based on real world lunar phase.");
            Console.WriteLine("For instance during a real world full moon the harvest multiplier could be increased ");
            Console.WriteLine("and during new moon the harvest multiplier could be decreased.");
            Console.WriteLine();
            Console.WriteLine("Optional: Preview moon phases x number of days ahead.  If using this option settings file will not be updates.");
            Console.WriteLine();
            Console.WriteLine("Usage: " + System.AppDomain.CurrentDomain.FriendlyName + ".exe [path to Conan Exiles ServerSettings.ini]");
            Console.WriteLine();
            Console.WriteLine("Example: " + System.AppDomain.CurrentDomain.FriendlyName + ".exe c:\\Exiles\\ConanSandbox\\Saved\\Config\\WindowsServer\\ServerSettings.ini");
            Console.WriteLine();

            // output the options
            Console.WriteLine("Options:");
            optional.WriteOptionDescriptions(Console.Out);


            return 10;
        }

        public class Harvest
        {

            public DateTime HarvestDate { get; set; }
            public double HarvestMultiplier { get; set; }
            public string MessageOfTheDay { get; set; }
            public int MoonPhaseCode { get; set; }
            public string MoonPhase { get; set; }
            public double NPCDamageMultiplier { get; set; }
            public double NPCDamageTakenMultiplier { get; set; }

            public Harvest(DateTime harvestDate)
            {
                HarvestDate = harvestDate;
                MoonPhaseCode = this.GetMoonPhase(harvestDate);
                SetMoonPhaseMultiplier(this.GetMoonPhase(harvestDate));

            }

            private void SetMoonPhaseMultiplier(int moonPhase)
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

                this.MoonPhase = "";
                this.MessageOfTheDay = "";
                string NPCDamageDesc = "";

                //new moon
                if (moonPhase == 0 || moonPhase == 29)
                {
                    this.MoonPhase = "New Moon";
                    this.HarvestMultiplier = 1;
                    this.NPCDamageMultiplier = 1;
                    this.NPCDamageTakenMultiplier = 1;
                    NPCDamageDesc = "NPC's are weakest during the New Moon.";
                }

                //waxing crescent
                if (1 <= moonPhase && moonPhase <= 5)
                {
                    this.MoonPhase = "Waxing Crescent Moon";
                    this.HarvestMultiplier = 1.5;
                    this.NPCDamageMultiplier = 1.2;
                    this.NPCDamageTakenMultiplier = 0.9;
                    NPCDamageDesc = "NPC's are weaker during the Crescent Moon.";
                }

                //first quarter
                if (6 <= moonPhase && moonPhase <= 9)
                {
                    this.MoonPhase = "First Quarter Moon";
                    this.HarvestMultiplier = 3;
                    this.NPCDamageMultiplier = 1.4;
                    this.NPCDamageTakenMultiplier = 0.8;
                    NPCDamageDesc = "NPC's are gaining strength during this moon phase.";
                }
                //waxing gibbous
                if (10 <= moonPhase && moonPhase <= 14)
                {
                    this.MoonPhase = "Waxing Gibbous Moon";
                    this.HarvestMultiplier = 4.5;
                    this.NPCDamageMultiplier = 1.5;
                    this.NPCDamageTakenMultiplier = 0.7;
                    NPCDamageDesc = "Beware, NPC's grow even stronger during the Gibbous Moon.";
                }
                //full
                if (15 <= moonPhase && moonPhase <= 16)
                {
                    this.MoonPhase = "Full Moon";
                    this.HarvestMultiplier = 6;
                    this.NPCDamageMultiplier = 2;
                    this.NPCDamageTakenMultiplier = 0.5;
                    NPCDamageDesc = "NPC's are at their peak power during the Full Moon!";
                }
                //waning gibbous
                if (17 <= moonPhase && moonPhase <= 20)
                {
                    this.MoonPhase = "Waning Gibbous Moon";
                    this.HarvestMultiplier = 4.5;
                    this.NPCDamageMultiplier = 1.5;
                    this.NPCDamageTakenMultiplier = 0.7;
                    NPCDamageDesc = "Beware, NPC's grow even stronger during the Gibbous Moon.";
                }
                //last quarter
                if (21 <= moonPhase && moonPhase <= 23)
                {
                    this.MoonPhase = "Last Quarter Moon";
                    this.HarvestMultiplier = 3;
                    this.NPCDamageMultiplier = 1.4;
                    this.NPCDamageTakenMultiplier = 0.8;
                    NPCDamageDesc = "NPC's are gaining strength during this moon phase.";
                }
                //waning crescent
                if (24 <= moonPhase && moonPhase <= 28)
                {
                    this.MoonPhase = "Waning Crescent Moon";
                    this.HarvestMultiplier = 1.5;
                    this.NPCDamageMultiplier = 1.2;
                    this.NPCDamageTakenMultiplier = 0.9;
                    NPCDamageDesc = "NPC's are weaker during the Crescent Moon.";

                }

                this.MessageOfTheDay = this.MoonPhase + ": " + 
                    this.HarvestMultiplier.ToString() + "x harvest multiplier.  " +
                    NPCDamageDesc;


                return ;
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
