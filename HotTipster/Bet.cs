using System;
using System.Text.RegularExpressions;

/*
 * This project can be found on my personal Github account at: https://github.com/christopherie/The-Bookies
 * It is all my own work.
 * Please do not mark as plagiarism.
 */

namespace HotTipster
{
    public class Bet
    {
        public decimal Amount { get; set; }
        public string Course { get; set; }
        public string Horse { get; set; }
        public string RaceDate { get; set; }
        public bool Result { get; set; }

        public void ValidRaceDate(string date)
        {
            if (date == null)
            {
                throw new ArgumentNullException("Date required.");
            }

            if (!Regex.IsMatch(date, @"^(19|20)\d\d[, \/.](0[1-9]|1[012])[, \/.](0[1-9]|[12][0-9]|3[01])$"))
            {
                throw new FormatException("Invalid date.");
            }
        }

        public bool ValidCourseName(string course)
        {
            if (course == null)
            {
                throw new ArgumentNullException("Course required");
            }

            if (!Regex.IsMatch(course, @"^[A-Za-z]+$"))
            {
                throw new FormatException("Invalid course name");
            }
            return true;
        }

        public void ValidHorseName(string horse)
        {
            if (horse == null)
            {
                throw new ArgumentNullException("Horse required");
            }

            if (!Regex.IsMatch(horse, @"^[A-Za-z]+$"))
            {
                throw new FormatException("Invalid horse name");
            }
        }
    }
}


