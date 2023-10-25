﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Transactions;

namespace Vaccination
{
    public class Person
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string SocialSecurityNumber { get; set; }
        public int HealthEmployee { get; set; }
        public int RiskGroup { get; set; }
        public int RecentInfection { get; set; }

        public Person(string firstName, string lastName, string socialSecurityNumber, int healthEmployee, int riskGroup, int recentInfection)
        {
            FirstName = firstName;
            LastName = lastName;
            SocialSecurityNumber = socialSecurityNumber;
            HealthEmployee = healthEmployee;
            RiskGroup = riskGroup;
            RecentInfection = recentInfection;
        }
    }

    public class OrderOfPriority
    {
        public string SocialSecurityNumber { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public int NumberOfVaccinations { get; set; }

        public OrderOfPriority(string socialSecurityNumber, string lastName, string firstName, int numberOfVaccinations)
        {
            SocialSecurityNumber = socialSecurityNumber;
            LastName = lastName;
            FirstName = firstName;
            NumberOfVaccinations = numberOfVaccinations;
        }
    }

    public class Program
    {
        static bool run = true;

        public static List<Person> listOfPeople = new();
        public static List<OrderOfPriority> listOfPriorities = new();
        public static int numberOfVaccinationDoses = 0;
        public static bool underAge = false;
        public static string inputFilePath = @"C:\Windows\Temp\People.csv";
        public static string outputFilePath = @"C:\Windows\Temp\Vaccinations.csv";

        static string[] mainMenuOptions =
        {
            "Skapa prioriteringsordning",
            "Ändra antal vaccindoser",
            "Ändra åldersgräns",
            "Ändra indatafil",
            "Ändra utdatafil",
            "Avsluta"};
        public static void Main()
        {
            CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;

            while (run)
            {
                Console.Clear();
                string vaccinateChildren = underAge ? "Ja" : "Nej";

                Presentation("Huvudmeny");

                //Info här:
                Console.WriteLine($"Antal tillgängliga vaccindoser: {numberOfVaccinationDoses}");
                Console.WriteLine($"Vaccinering under 18 år: {vaccinateChildren}");
                Console.WriteLine($"Indatafil: {inputFilePath}");
                Console.WriteLine($"Utdatafil: {outputFilePath}");
                Console.WriteLine();

                int mainMenuIndex = ShowMenu("Vad vill du göra?", mainMenuOptions);
                Console.Clear();

                Console.WriteLine();

                Presentation(mainMenuOptions[mainMenuIndex]);

                if (mainMenuIndex == 0)
                {
                    VaccinationOrder();
                }
                else if (mainMenuIndex == 1)
                {
                    NewNumberOfDoses();
                }
                else if (mainMenuIndex == 2)
                {
                    ChangeAge();
                }
                else if (mainMenuIndex == 3)
                {
                    ChangeFile(inputFilePath);
                }
                else if (mainMenuIndex == 4)
                {
                    ChangeFile(outputFilePath);
                }
                else if (mainMenuIndex == 5)
                {
                    Exit();
                }
            }
        }

        public static void VaccinationOrder()
        {
            string fullPath = Path.Combine(Environment.CurrentDirectory, inputFilePath);
            try
            {

                if (File.Exists(fullPath))
                {
                    string[] outPut = CreateVaccinationOrder(File.ReadAllLines(inputFilePath), numberOfVaccinationDoses, underAge);
                    File.WriteAllLines(outputFilePath, outPut);
                    Console.WriteLine("Resultatet har sparats i " + outputFilePath);
                }
                else
                {
                    Console.WriteLine("Input fil ej funnen: " + fullPath);
                }
            }
            catch (IOException ex)
            {
                Console.WriteLine("Ett fel uppstod under bearbetningen av filerna: " + ex.Message);
            }
            Thread.Sleep(3000);

        }

        public static string[] CreateVaccinationOrder(string[] input, int doses, bool vaccinateChildren)
        {
            string[] prio = new string[input.Length];
            try
            {
                foreach (string line in input)
                {
                    string[] values = line.Split(',');
                    string socialSecutiryNumber = SocialSecurityNumber(values);
                    string lastName = values[1];
                    string firstName = values[2];
                    int healthEmployee = int.Parse(values[3]);
                    int riskGroup = int.Parse(values[4]);
                    int recentInfections = int.Parse(values[5]);
                    Person person = new(firstName, lastName, socialSecutiryNumber, healthEmployee, riskGroup, recentInfections);
                    listOfPeople.Add(person);
                }
                SortListOfPeople(listOfPeople);

                int tempVarDoses = doses;

                foreach (Person person in listOfPeople)
                {

                    if (vaccinateChildren)
                    {
                        int dose = person.RecentInfection == 1 ? 1 : 2;
                        if (tempVarDoses == 1 && dose == 1)
                        {
                            OrderOfPriority order = new(person.SocialSecurityNumber, person.LastName, person.FirstName, dose);
                            listOfPriorities.Add(order);
                            tempVarDoses -= dose;
                        }
                        else if (tempVarDoses < 1)
                        {
                            dose = 0;
                            OrderOfPriority order = new(person.SocialSecurityNumber, person.LastName, person.FirstName, dose);
                            listOfPriorities.Add(order);
                        }
                        else
                        {
                            OrderOfPriority order = new(person.SocialSecurityNumber, person.LastName, person.FirstName, dose);
                            listOfPriorities.Add(order);
                            tempVarDoses -= dose;
                        }
                    }
                    else if (!vaccinateChildren)
                    {
                        int birthYear = int.Parse(person.SocialSecurityNumber.Substring(0, 4));
                        DateTime localTime = DateTime.Now;
                        if (localTime.Year - birthYear >= 18)
                        {
                            int dose = person.RecentInfection == 1 ? 1 : 2;
                            if (tempVarDoses == 1 && dose == 1)
                            {
                                OrderOfPriority order = new(person.SocialSecurityNumber, person.LastName, person.FirstName, dose);
                                listOfPriorities.Add(order);
                                tempVarDoses -= dose;
                            }
                            else if (tempVarDoses < 1)
                            {
                                dose = 0;
                                OrderOfPriority order = new(person.SocialSecurityNumber, person.LastName, person.FirstName, dose);
                                listOfPriorities.Add(order);
                            }
                            else
                            {
                                OrderOfPriority order = new(person.SocialSecurityNumber, person.LastName, person.FirstName, dose);
                                listOfPriorities.Add(order);
                                tempVarDoses -= dose;
                            }
                        }
                    }
                }
                List<string> list = new();
                foreach (OrderOfPriority order in listOfPriorities)
                {
                    string person = order.SocialSecurityNumber + "," + order.LastName + "," + order.FirstName + "," + order.NumberOfVaccinations;
                    list.Add(person);
                }
                string[] OutpurArray = list.ToArray();

                return OutpurArray;
            }
            catch (FormatException ex)
            {
                Console.WriteLine("Error parsing data: " + ex.Message);
                return new string[0];
            }
        }


        public static void SortListOfPeople(List<Person> list)
        {
            list.Sort((p1, p2) =>
            {
                if (p1.HealthEmployee == 1 && p2.HealthEmployee != 1)
                {
                    return -1;
                }
                else if (!(p1.HealthEmployee == 1) && p2.HealthEmployee == 1)
                {
                    return 1;
                }
                int birthYearP1 = int.Parse(p1.SocialSecurityNumber.Substring(0, 4));
                int birthYearP2 = int.Parse(p2.SocialSecurityNumber.Substring(0, 4));
                DateTime localTime = DateTime.Now;

                if (localTime.Year - birthYearP1 >= 65 && !(localTime.Year - birthYearP2 >= 65))
                {
                    return -1;
                }
                else if (!(localTime.Year - birthYearP1 >= 65) && localTime.Year - birthYearP2 >= 65)
                {
                    return 1;
                }

                if (p1.RiskGroup == 1 && p2.RiskGroup != 1)
                {
                    return -1;
                }
                else if (!(p1.RiskGroup == 1) && p2.RiskGroup == 1)
                {
                    return 1;
                }

                int birthDateP1 = int.Parse(p1.SocialSecurityNumber.Substring(0, 8));
                int birthDateP2 = int.Parse(p2.SocialSecurityNumber.Substring(0, 8));
                int compareAges = birthDateP1 - birthDateP2;

                if (compareAges < 0)
                {
                    return -1;
                }
                else if (compareAges > 0)
                {
                    return 1;
                }
                return 0;
            });
        }

        public static void NewNumberOfDoses()
        {
            while (true)
            {
                try
                {
                    Console.Write("Ange nytt antal doser: ");
                    int newDoses = int.Parse(Console.ReadLine());

                    if (newDoses >= 0)
                    {
                        numberOfVaccinationDoses = newDoses;
                        break;
                    }
                    else
                    {
                        Console.Clear();
                        Console.WriteLine("Antal doser måste vara ett postivit heltal");
                    }
                }
                catch (FormatException)
                {
                    Console.Clear();
                    Console.WriteLine("Ogiltig inmating. Ange ett heltal.");
                }
            }
        }

        public static void ChangeAge()
        {
            int vaccinateChildren = ShowMenu("Ska personer under 18 vaccineras?", new[]
            {
                        "Ja",
                        "Nej"
                    });
            underAge = (vaccinateChildren == 0);
        }

        public static void ChangeFile(string oldFile)
        {
            if (oldFile == inputFilePath)
            {
                Console.Write("Ange ny sökväg: ");
                inputFilePath = Console.ReadLine();
            }
            else
            {
                Console.Write("Ange ny sökväg: ");
                outputFilePath = Console.ReadLine();
            }
        }

        public static void Exit()
        {
            Console.WriteLine("Hejdå min vän!");
            Thread.Sleep(1000);
            run = false;
        }

        public static string SocialSecurityNumber(string[] value)
        {
            string socialSecurityNumber = "";
            int firstVal = int.Parse(value[0].Remove(2));
            if (!value[0].Contains('-'))
            {
                socialSecurityNumber = value[0].Insert(value[0].Length - 4, "-");

                if (!value[0].StartsWith("19"))
                {
                    if (firstVal < 23)
                    {
                        socialSecurityNumber = 20 + socialSecurityNumber;
                    }
                    else
                    {
                        socialSecurityNumber = 19 + socialSecurityNumber;
                    }
                }
            }
            else if (!value[0].StartsWith("19"))
            {
                if (firstVal < 23)
                {
                    socialSecurityNumber = 20 + value[0];
                }
                else
                {
                    socialSecurityNumber = 19 + value[0];
                }
            }
            else
            {
                socialSecurityNumber = value[0];
            }
            return socialSecurityNumber;
        }


        public static void Presentation(string input)
        {
            Console.WriteLine(input);
            for (int i = 0; i < input.Length; i++)
            {
                Console.Write("-");
            }
            Console.WriteLine();
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