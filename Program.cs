using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace csv2ical
{
    class Program
    {

        static void Main(string[] args)
        {
            #region createics
            //FILE HEADER
            string header = "BEGIN:VCALENDAR\r\nVERSION:2.0\r\nPRODID:-//Bartosz Kuciel//WSTI iCal Schedule//PL\r\nX-WR-CALNAME:PLAN ZAJEC WSTI\r\nX-WR-TIMEZONE:Europe/Warsaw\r\n";
            string result = string.Empty;
            string add = string.Empty;
            System.IO.File.WriteAllText("cal.ical", header);
            #endregion


            const Int32 BufferSize = 128;
            using (var fileStream = File.OpenRead("4FZI.csv"))
            using (var streamReader = new StreamReader(fileStream, Encoding.UTF8, true, BufferSize))
            //Encoding.GetEncoding("Windows-1252")
            {
                string date, timest, timeend, DTSTART, DTEND;
                string subject = string.Empty, type=string.Empty, location=string.Empty, teacher=string.Empty, timeendWorking=string.Empty;
                string previousdate = string.Empty;
                Regex rgdata = new Regex(@"[0123]?\d[\/-][0123]?\d[\/-]\d{4}");
                Regex rgtimest = new Regex(@"\d\d:\d\d -");
                Regex rgtimeend = new Regex(@"(- \d\d:\d\d)|(- \d:\d\d)");
                Regex rgtimecorrect = new Regex(@"(- \d:\d\d)");
                // Regex rgsubject = new Regex(@"");

                String line;
                while ((line = streamReader.ReadLine()) != null)
                {
                    //date
                    Match md = rgdata.Match(line);
                    date = md.ToString();
                    //change order of date
                    string[] dateWorking = date.Split(new[] { '/' }, 3);
                    if (date != "")
                        date = string.Format("{2}{1}{0}", dateWorking);
                    else
                        date = previousdate;
                    //start time
                    Match mts = rgtimest.Match(line);
                    timest = mts.ToString();
                    timest = Regex.Replace(timest, " -", string.Empty);
                    timest = Regex.Replace(timest, ":", string.Empty);
                    //end time
                    Match mte = rgtimeend.Match(line);
                    timeend = mte.ToString();
                    timeend = Regex.Replace(timeend, "- ", string.Empty);
                    timeend = Regex.Replace(timeend, ":", string.Empty);
                    //case: time 8:45 instead of 08:45
                    if (Regex.IsMatch(timeend, @"(^\d\d\d$)"))
                    {
                        //timeendWorking = "0" + timeend;
                        timeend = "0" + timeend;
                      //  timeend = Regex.Replace(timeend, @"(^\d\d\d$)", timeendWorking);
                    }
                    //Subject
                    if (line != ";;;;;" && line != "")
                    {

                        string[] subjectWorking = line.Split(new[] { ';' }, 7);
                        subject = string.Format("{2}", subjectWorking);
                        System.Console.Write(subject);
                    }
                    //Type
                    if (line != "")
                    {
                        string[] typeWorking = line.Split(new[] { ';' }, 7);
                        type = string.Format("{3}", typeWorking);
                    }
                    //Teacher
                    if (line != "")
                    {
                        string[] teacherWorking = line.Split(new[] { ';' }, 7);
                        teacher = string.Format("{5}", teacherWorking);
                    }
                    //Location
                    if (line != "")
                    {
                        string[] locationWorking = line.Split(new[] { ';' }, 7);
                        location = string.Format("{4}", locationWorking);
                    }

                    //DEBUG
                    Console.WriteLine(line);
                    
                    


                    if (line != "DATA;GODZINA;MODUL;RODZAJ ZAJEC;SALA;PROWADZACY")
                        if (line != ";;;;;")
                        {
                            if (type !="" && subject !="" && teacher !="")
                            {
                                DTSTART = date + "T" + timest + "00";
                                DTEND = date + "T" + timeend + "00";


                                previousdate = date;
                                add = "BEGIN:VEVENT\r\nDTSTART:" + DTSTART + "\r\nDTEND:" + DTEND + "\r\nSUMMARY:" + type + " " + subject + "\r\nDESCRIPTION:"  + teacher + "\r\nLOCATION:" + location + "\r\nEND:VEVENT\r\n";
                                //System.Console.Write("--DEBUG--\n" + add + "\n");
                                File.AppendAllText("cal.ical", add);

                            }
                        }

                    //END FILE



                }
                string end = "END:VCALENDAR";
                File.AppendAllText("cal.ical", end);
            }

            Console.ReadLine();
        }
    }
}