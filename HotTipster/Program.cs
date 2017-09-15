using System;
using System.Collections.Generic;
using static System.Console;
using System.Linq;
using System.IO;

/*
 * This project can be found on my personal Github account at: https://github.com/christopherie/The-Bookies
 * It is all my own work.
 * Please do not mark as plagiarism.
 */

namespace HotTipster
{
    delegate void PrintReport(List<Bet> bets);
    class Program
    {
        // Directory and File details
        public static readonly string DIRECTORY_PATH = $@"C:\Users\predator\Documents\Visual Studio 2015\Projects\AdvancedProgramming\HotTipster\HotTipster\";
        public static readonly string FILE_NAME = "bet.bin";
        public static readonly string FILE_NAME2 = "totals.txt";
        public static readonly string FILE_NAME3 = "popularRaceCourse.txt";
        public static readonly string FILE_NAME4 = "betsInDateOrder.txt";
        public static readonly string FILE_NAME5 = "highestWonAndLost.txt";
        public static readonly string FILE_NAME6 = "winRatio.txt";

        static void Main(string[] args)
        {
            // Variables
            bool exit = false, valid = false, result, output;
            decimal amount;
            int option;
            string course, horse, raceDate;

            // Menu and user input
            do
            {
                Bet bet = new Bet();
                // User menu option
                Write("Enter 1 to add new bet OR 2 to exit: ");
                output = int.TryParse(ReadLine().Trim(), out option);
                while (valid == false)
                {
                    if (output == true)
                    {
                        valid = true;
                    }
                    else
                    {
                        valid = false;
                        Write("Enter 1 to add new bet OR 2 to exit: ");
                        output = int.TryParse(ReadLine().Trim(), out option);
                    }
                }

                if (option == 1)
                {
                    // Course name
                    valid = false;
                    Write("Enter course name: ");
                    course = ReadLine().Trim();
                    while (valid == false)
                    {
                        try
                        {
                            bet.ValidCourseName(course);
                            valid = true;
                        }
                        catch (Exception ex)
                        {
                            WriteLine(ex.Message);
                            valid = false;
                            Write("Enter course name: ");
                            course = ReadLine().Trim();
                        }
                    }

                    // Horse name
                    Write("Enter horse name: ");
                    horse = ReadLine().Trim();
                    valid = false;
                    while (valid == false)
                    {
                        try
                        {
                            bet.ValidHorseName(horse);
                            valid = true;
                        }
                        catch (Exception ex)
                        {
                            WriteLine(ex.Message);
                            valid = false;
                            Write("Enter horse name: ");
                            horse = ReadLine().Trim();
                        }
                    }

                    // Race date
                    Write("Enter race date (YYYY,MM,DD): ");
                    raceDate = ReadLine().Trim();
                    valid = false;
                    while (valid == false)
                    {
                        try
                        {
                            bet.ValidRaceDate(raceDate);
                            valid = true;
                        }
                        catch (Exception ex)
                        {
                            WriteLine(ex.Message);
                            valid = false;
                            Write("Enter race date (YYYY,MM,DD): ");
                            raceDate = ReadLine().Trim();
                        }
                    }

                    // Bet amount
                    Write("Enter bet amount: ");
                    output = decimal.TryParse(ReadLine().Trim(), out amount);
                    valid = false;
                    while (valid == false)
                    {
                        if (output == true)
                        {
                            valid = true;
                        }
                        else
                        {
                            valid = false;
                            Write("Enter bet amount: ");
                            output = decimal.TryParse(ReadLine().Trim(), out amount);
                        }
                    }

                    // Win or loss
                    Write("Enter result (true for win, false for loss): ");
                    output = bool.TryParse(ReadLine().Trim(), out result);
                    valid = false;
                    while (valid == false)
                    {
                        if (output == true)
                        {
                            valid = true;
                        }
                        else
                        {
                            valid = false;
                            Write("Enter result (true for win, false for loss): ");
                            output = bool.TryParse(ReadLine().Trim(), out result);
                        }
                    }

                    // Write to binary file
                    WriteToBinaryFile(course, horse, raceDate, amount, result);
                }
                else if (option == 2)
                {
                    exit = true;
                }
                else
                {
                    WriteLine("Invalid option");
                }
            } while (!exit);

            // Read from binary file
            ReadFromBinaryFile();

            ReadLine();
        }

        private static void ReadFromBinaryFile()
        {
            using (Stream fs = new FileStream($@"{DIRECTORY_PATH}{FILE_NAME}", FileMode.Open))
            using (BinaryReader br = new BinaryReader(fs))
            {
                List<Bet> bets = new List<Bet>();
                while (br.PeekChar() != -1)
                {
                    string course = br.ReadString();
                    string horse = br.ReadString();
                    string raceDate = br.ReadString();
                    decimal amount = br.ReadDecimal();
                    bool result = br.ReadBoolean();
                    bets.Add(new Bet() { Course = course, Horse = horse, RaceDate = raceDate, Amount = amount, Result = result });
                }

                PrintReport pr;

                // Print to win ratio report
                pr = PrintWinRatioReport;
                pr(bets);
                // Print to most popular course report
                pr = PrintMostPopularCourseReport;
                pr(bets);
                // Print to bet dates report
                pr = PrintBetDatesReport;
                pr(bets);
                // Print bet with highest amount lost and
                // bet with highest amount won
                pr = PrintHighestWonLostReport;
                pr(bets);
                // Print totals
                pr = PrintTotals;
                pr(bets);
            }
        }

        private static void PrintTotals(List<Bet> bets)
        {
            var totals = from bet in bets
                         orderby bet.RaceDate
                         let k = new
                         {
                             Year = bet.RaceDate.Substring(0, 4),
                         }
                         group bet by k into t
                         select new
                         {
                             Year = t.Key.Year,
                             Loss = t.Where(m => m.Result == false).Sum(m => m.Amount),
                             Wins = t.Where(m => m.Result == true).Sum(m => m.Amount)
                         };

            using (Stream fs = new FileStream($@"{DIRECTORY_PATH}{FILE_NAME2}", FileMode.OpenOrCreate))
            using (TextWriter tw = new StreamWriter(fs))
            {
                tw.WriteLine("YEAR\tTOTAL WON\tTOTAL LOST");
                foreach (var total in totals)
                {
                    tw.WriteLine(total.Year + "\t" + total.Wins + "\t\t" + total.Loss);
                }
            }
        }

        private static void PrintHighestWonLostReport(List<Bet> bets)
        {
            // Highest lost
            var highestLossAmount = bets.Where(m => m.Result == false).Max(m => m.Amount);
            var highestLoss = bets.Where(m => m.Amount == highestLossAmount).First();

            // highest won
            var highestWinAmount = bets.Where(m => m.Result == true).Max(m => m.Amount);
            var highestWin = bets.Where(m => m.Amount == highestWinAmount).First();

            using (Stream fs = new FileStream($@"{DIRECTORY_PATH}{FILE_NAME5}", FileMode.OpenOrCreate))
            using (TextWriter tw = new StreamWriter(fs))
            {
                tw.WriteLine($"Highest amount lost on a bet is {highestLoss.Amount}");
                tw.WriteLine($"Highest amount won on a bet is {highestWin.Amount}");
            }
        }

        private static void PrintBetDatesReport(List<Bet> bets)
        {
            var dates = bets.OrderBy(m => m.RaceDate);
            using (Stream fs = new FileStream($@"{DIRECTORY_PATH}{FILE_NAME4}", FileMode.OpenOrCreate))
            using (TextWriter tw = new StreamWriter(fs))
            {
                foreach (var date in dates)
                {
                    tw.WriteLine($"Course: {date.Course}");
                    tw.WriteLine($"Horse: {date.Horse}");
                    tw.WriteLine($"Date: {date.RaceDate}");
                    tw.WriteLine($"Amount: {date.Amount}");
                    tw.WriteLine($"Result: {date.Result}");
                    tw.WriteLine();
                }
            }
        }

        private static void PrintMostPopularCourseReport(List<Bet> bets)
        {
            var course = bets.GroupBy(m => m.Course).OrderByDescending(m => m.Count()).First().Key;
            using (Stream fs = new FileStream($@"{DIRECTORY_PATH}{FILE_NAME3}", FileMode.OpenOrCreate))
            using (TextWriter tw = new StreamWriter(fs))
            {
                tw.Write($"Most bets were placed at {course.ToString()}");
            }
        }

        private static void PrintWinRatioReport(List<Bet> bets)
        {
            var races = bets.Count();
            var wins = bets.Where(m => m.Result == true).Count();
            string ratio = wins.ToString() + "/" + races.ToString();
            using (Stream fs = new FileStream($@"{DIRECTORY_PATH}{FILE_NAME6}", FileMode.OpenOrCreate))
            using (TextWriter tw = new StreamWriter(fs))
            {
                tw.Write($"Win ratio is {ratio}");
            }
        }

        private static void WriteToBinaryFile(string course, string horse,
            string raceDate, decimal amount, bool result)
        {
            using (Stream fs = new FileStream($@"{DIRECTORY_PATH}{FILE_NAME}",
                FileMode.OpenOrCreate))
            using (BinaryWriter bw = new BinaryWriter(fs))
            {
                fs.Seek(0, SeekOrigin.End);
                Bet bet = new Bet();
                bet.Course = course;
                bet.Horse = horse;
                bet.RaceDate = raceDate;
                bet.Amount = amount;
                bet.Result = result;
                bw.Write(bet.Course);
                bw.Write(bet.Horse);
                bw.Write(bet.RaceDate);
                bw.Write(bet.Amount);
                bw.Write(bet.Result);
            }
        }
    }
}
