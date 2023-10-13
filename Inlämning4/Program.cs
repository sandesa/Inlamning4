﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;

namespace Vaccination
{
    public class Program
    {
        static bool run = true;
        public static int numberOfVaccinationDoses = 0;
        public static string underAge = "Nej";
        public static string inputFile = "C:\\Windows\\Temp\\People.csv";
        public static string outputFile = "C:\\Windows\\Temp\\Vaccinations.csv";
        public static void Main()
        {
            CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;

            while (run)
            {
                Console.Clear();
                string mainMeny = "Huvudmeny";
                Presentation(mainMeny);

                //Info här
                Console.WriteLine($"Antal tillgängliga vaccindoser: {numberOfVaccinationDoses}");
                Console.WriteLine($"Vaccinering under 18 år: {underAge}");
                Console.WriteLine($"Indatafil: {inputFile}");
                Console.WriteLine($"Utdatafil: {outputFile}");


                Console.WriteLine();
                int mainMenu = ShowMenu("Vad vill du göra?", new[]
                {
                    "Skapa prioriteringsordning",
                    "Ändra antal vaccindoser",
                    "Ändra åldersgräns",
                    "Ändra indatafil",
                    "Ändra utdatafil",
                    "Avsluta"
                });
                Console.Clear();

                if (mainMenu == 0)
                {

                }
                else if (mainMenu == 1)
                {
                    NewNumberOfDoses();
                }
                else if (mainMenu == 2)
                {
                    ChangeAge();   
                }
                else if (mainMenu == 3)
                {
                    ChangeFile(inputFile);
                }
                else if (mainMenu == 4)
                {
                    ChangeFile(outputFile);
                }
                else if (mainMenu == 5)
                {
                    Exit();
                }
            }



        }

        // Create the lines that should be saved to a CSV file after creating the vaccination order.
        //
        // Parameters:
        //
        // input: the lines from a CSV file containing population information
        // doses: the number of vaccine doses available
        // vaccinateChildren: whether to vaccinate people younger than 18
        public static string[] CreateVaccinationOrder(string[] input, int doses, bool vaccinateChildren)
        {
            // Replace with your own code.
            return new string[0];
        }
        public static void NewNumberOfDoses()
        {
            Presentation("Ändra antal vaccindoser");
            Console.Write("Ange nytt antal doser: ");
            numberOfVaccinationDoses = int.Parse(Console.ReadLine());
        }
        public static void ChangeAge()
        {
            Presentation("Ändra åldersgräns");
            int underTheAge = ShowMenu("Ska personer under 18 vaccineras?", new[]
            {
                        "Ja",
                        "Nej"
                    });
            underAge = (underTheAge == 0) ? "Ja" : "Nej";
        }

        public static void Presentation (string input)
        {
            Console.WriteLine(input);
            for (int i = 0; i < input.Length; i++)
            {
                Console.Write("-");
            }
            Console.WriteLine();
        }
        public static void ChangeFile(string oldFile)
        {
            if (oldFile == inputFile)
            {
                Presentation("Ändra indatafil");
                Console.Write("Ange ny sökväg: ");
                inputFile = Console.ReadLine();
            }
            else
            {
                Presentation("Ändra utdatafil");
                Console.Write("Ange ny sökväg: ");
                outputFile = Console.ReadLine();
            }
        }

        public static void Exit ()
        {
            Console.WriteLine("Hejdå min vän!");
            Thread.Sleep(1000);
            run = false;
        }

        public static int ShowMenu(string prompt, IEnumerable<string> options)
        {
            if (options == null || options.Count() == 0)
            {
                throw new ArgumentException("Cannot show a menu for an empty list of options.");
            }

            Console.WriteLine(prompt);

            // Hide the cursor that will blink after calling ReadKey.
            Console.CursorVisible = false;

            // Calculate the width of the widest option so we can make them all the same width later.
            int width = options.Max(option => option.Length);

            int selected = 0;
            int top = Console.CursorTop;
            for (int i = 0; i < options.Count(); i++)
            {
                // Start by highlighting the first option.
                if (i == 0)
                {
                    Console.BackgroundColor = ConsoleColor.Blue;
                    Console.ForegroundColor = ConsoleColor.White;
                }

                var option = options.ElementAt(i);
                // Pad every option to make them the same width, so the highlight is equally wide everywhere.
                Console.WriteLine("- " + option.PadRight(width));

                Console.ResetColor();
            }
            Console.CursorLeft = 0;
            Console.CursorTop = top - 1;

            ConsoleKey? key = null;
            while (key != ConsoleKey.Enter)
            {
                key = Console.ReadKey(intercept: true).Key;

                // First restore the previously selected option so it's not highlighted anymore.
                Console.CursorTop = top + selected;
                string oldOption = options.ElementAt(selected);
                Console.Write("- " + oldOption.PadRight(width));
                Console.CursorLeft = 0;
                Console.ResetColor();

                // Then find the new selected option.
                if (key == ConsoleKey.DownArrow)
                {
                    selected = Math.Min(selected + 1, options.Count() - 1);
                }
                else if (key == ConsoleKey.UpArrow)
                {
                    selected = Math.Max(selected - 1, 0);
                }

                // Finally highlight the new selected option.
                Console.CursorTop = top + selected;
                Console.BackgroundColor = ConsoleColor.Blue;
                Console.ForegroundColor = ConsoleColor.White;
                string newOption = options.ElementAt(selected);
                Console.Write("- " + newOption.PadRight(width));
                Console.CursorLeft = 0;
                // Place the cursor one step above the new selected option so that we can scroll and also see the option above.
                Console.CursorTop = top + selected - 1;
                Console.ResetColor();
            }

            // Afterwards, place the cursor below the menu so we can see whatever comes next.
            Console.CursorTop = top + options.Count();

            // Show the cursor again and return the selected option.
            Console.CursorVisible = true;
            return selected;
        }
    }

    [TestClass]
    public class ProgramTests
    {
        [TestMethod]
        public void ExampleTest()
        {
            // Arrange
            string[] input =
            {
                "19720906-1111,Elba,Idris,0,0,1",
                "8102032222,Efternamnsson,Eva,1,1,0"
            };
            int doses = 10;
            bool vaccinateChildren = false;

            // Act
            string[] output = Program.CreateVaccinationOrder(input, doses, vaccinateChildren);

            // Assert
            Assert.AreEqual(output.Length, 2);
            Assert.AreEqual("19810203-2222,Efternamnsson,Eva,2", output[0]);
            Assert.AreEqual("19720906-1111,Elba,Idris,1", output[1]);
        }
    }
}