using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;

namespace Vaccinations
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

    public class PersonToVaccinate
    {
        public string SocialSecurityNumber { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public int NumberOfVaccinations { get; set; }

        public PersonToVaccinate(string socialSecurityNumber, string lastName, string firstName, int numberOfVaccinations)
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
        static bool executeDataTransformation = true;

        static List<Person> people = new();
        static List<PersonToVaccinate> priorityList = new();
        static int numberOfVaccinationDoses = 0;
        static bool underAge = false;
        static string inputFilePath = @"C:\Windows\Temp\People.csv";
        static string outputFilePath = @"C:\Windows\Temp\Vaccinations.csv";

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
                executeDataTransformation = true;
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
                Console.WriteLine();
            }
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

        public static void VaccinationOrder()
        {
            try
            {
                if (File.Exists(inputFilePath))
                {
                    string[] outPutData = CreateVaccinationOrder(File.ReadAllLines(inputFilePath), numberOfVaccinationDoses, underAge);
                    if (Directory.Exists(Path.GetDirectoryName(outputFilePath)))
                    {
                        if (File.Exists(outputFilePath) && executeDataTransformation)
                        {
                            int warning = ShowMenu("Varning: utdatafilen " + outputFilePath + " finns redan. Vill du skriva över den?", new[]
                            {
                                    "Ja",
                                    "Nej"
                                });
                            executeDataTransformation = warning == 0;
                        }
                        if (executeDataTransformation)
                        {
                            File.WriteAllLines(outputFilePath, outPutData);
                            Console.WriteLine("Resultatet har sparats i " + outputFilePath);
                        }
                    }
                    else
                    {
                        Console.WriteLine("Mappen för utdatafilen ej funnen: " + Path.GetDirectoryName(outputFilePath));
                        Console.WriteLine("Vänligen skriv in en ny sökväg för utdatafilen: ");
                        ChangeFile(outputFilePath);
                    }
                }
                else
                {
                    Console.WriteLine("Indatafil ej funnen: " + inputFilePath);
                    Console.WriteLine("Vänligen skriv in en ny sökväg för indatafilen: ");
                    ChangeFile(inputFilePath);
                }
            }
            catch (IOException ex)
            {
                Console.WriteLine("Ett fel uppstod under bearbetningen av filerna: " + ex.Message);
            }
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


        public static string SocialSecurityNumberFormat(string socialSecurityNumber)
        {
            try
            {
                int hyphenIndex = socialSecurityNumber.Length - 5;
                if (socialSecurityNumber[hyphenIndex] != '-')
                {
                    socialSecurityNumber = socialSecurityNumber.Insert(hyphenIndex + 1, "-");
                }
                if (socialSecurityNumber.Length == 13 && (socialSecurityNumber.StartsWith("19") || socialSecurityNumber.StartsWith("20")))
                {
                    return socialSecurityNumber;
                }
                else
                {
                    int birthYear = int.Parse(socialSecurityNumber.Substring(0, 2));
                    if (birthYear < 23)
                    {
                        socialSecurityNumber = 20 + socialSecurityNumber;
                    }
                    else
                    {
                        socialSecurityNumber = 19 + socialSecurityNumber;
                    }
                }
                return socialSecurityNumber;
            }
            catch (FormatException ex)  
            {
                Console.WriteLine("Felaktigt format: " + ex.Message);
                executeDataTransformation = false;
                return string.Empty;
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
                else if (p1.HealthEmployee != 1 && p2.HealthEmployee == 1)
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
                int ageCompare = birthDateP1 - birthDateP2;

                if (ageCompare < 0)
                {
                    return -1;
                }
                else if (ageCompare > 0)
                {
                    return 1;
                }
                return 0;
            });
        }


        public static string[] CreateVaccinationOrder(string[] input, int doses, bool vaccinateChildren)
        {
            foreach (string line in input)
            {
                string[] values = line.Split(',');
                if (values.Length != 6)
                {
                    Console.WriteLine("Felaktig indata: " + line);
                    executeDataTransformation = false;
                    continue;
                }
                string socialSecutiryNumber = SocialSecurityNumberFormat(values[0]);
                if (socialSecutiryNumber.Length != 13)
                {
                    Console.WriteLine("Felaktigt indata av personnummer: " + line);
                    executeDataTransformation = false;
                    continue;
                }
                if (!int.TryParse(values[3], out int healthEmp) || (healthEmp != 1 && healthEmp != 0)
                    || !int.TryParse(values[4], out int riskGr) || (riskGr != 1 && riskGr != 0)
                    || !int.TryParse(values[5], out int recentInf) || (recentInf != 1 && recentInf != 0))
                {
                    Console.WriteLine("Felaktiga värden: " + line);
                    executeDataTransformation = false;
                    continue;
                }
                string lastName = values[1];
                string firstName = values[2];
                int healthEmployee = int.Parse(values[3]);
                int riskGroup = int.Parse(values[4]);
                int recentInfections = int.Parse(values[5]);
                Person person = new(firstName, lastName, socialSecutiryNumber, healthEmployee, riskGroup, recentInfections);
                people.Add(person);
            }

            SortListOfPeople(people);

            int numberOfVaccinesLeft = doses;
            int index = 0;
            foreach (Person person in people)
            {
                if (vaccinateChildren)
                {
                    int dose = person.RecentInfection == 1 ? 1 : 2;
                    if (numberOfVaccinesLeft == 1 && dose == 1)
                    {
                        if (index != 0)
                        {
                            dose = 0;
                        }
                        PersonToVaccinate personToVaccinate = new(person.SocialSecurityNumber, person.LastName, person.FirstName, dose);
                        priorityList.Add(personToVaccinate);
                        numberOfVaccinesLeft -= dose;
                    }
                    else if (numberOfVaccinesLeft == 0)
                    {
                        dose = 0;
                        PersonToVaccinate personToVaccinate = new(person.SocialSecurityNumber, person.LastName, person.FirstName, dose);
                        priorityList.Add(personToVaccinate);
                    }
                    else
                    {
                        PersonToVaccinate personToVaccinate = new(person.SocialSecurityNumber, person.LastName, person.FirstName, dose);
                        priorityList.Add(personToVaccinate);
                        numberOfVaccinesLeft -= dose;
                    }
                    index++;
                }
                else if (!vaccinateChildren)
                {
                    int birthYear = int.Parse(person.SocialSecurityNumber.Substring(0, 4));
                    DateTime localTime = DateTime.Now;
                    if (localTime.Year - birthYear >= 18)
                    {
                        int dose = person.RecentInfection == 1 ? 1 : 2;
                        if (numberOfVaccinesLeft == 1 && dose == 1)
                        {
                            if (index != 0)
                            {
                                dose = 0;
                            }
                            PersonToVaccinate personToVaccinate = new(person.SocialSecurityNumber, person.LastName, person.FirstName, dose);
                            priorityList.Add(personToVaccinate);
                            numberOfVaccinesLeft -= dose;
                        }
                        else if (numberOfVaccinesLeft == 0)
                        {
                            dose = 0;
                            PersonToVaccinate personToVaccinate = new(person.SocialSecurityNumber, person.LastName, person.FirstName, dose);
                            priorityList.Add(personToVaccinate);
                        }
                        else
                        {
                            PersonToVaccinate personToVaccinate = new(person.SocialSecurityNumber, person.LastName, person.FirstName, dose);
                            priorityList.Add(personToVaccinate);
                            numberOfVaccinesLeft -= dose;
                        }
                    }
                    index++;
                }
            }

            List<string> outputList = new();
            foreach (PersonToVaccinate personToVaccinate in priorityList)
            {
                string person = personToVaccinate.SocialSecurityNumber + "," + personToVaccinate.LastName + "," + personToVaccinate.FirstName +
                                "," + personToVaccinate.NumberOfVaccinations;
                outputList.Add(person);
            }
            string[] outputArray = outputList.ToArray();

            return outputArray;
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
        public void TestVaccinateChildren()
        {
            // Arrange
            string[] input =
            {
                "19720906-1111,Smith,John,0,0,1",
                "8102032222,Johnson,Emily,1,1,0",
                "1210021122,Oscarsson,Oskar,1,1,1"
            };
            int doses = 100;
            bool vaccinateChildren = true;

            // Act
            string[] output = Program.CreateVaccinationOrder(input, doses, vaccinateChildren);

            // Assert
            Assert.AreEqual(3, output.Length);
            Assert.AreEqual("19810203-2222,Johnson,Emily,2", output[0]);
            Assert.AreEqual("20121002-1122,Oscarsson,Oskar,1", output[1]);
            Assert.AreEqual("19720906-1111,Smith,John,1", output[2]);
        }

        [TestMethod]
        public void TestNotVaccinateChildren()
        {
            // Arrange
            string[] input =
            {
                "19720906-1111,Smith,John,0,0,1",
                "8102032222,Johnson,Emily,0,0,0",
                "1210021122,Oscarsson,Oskar,1,1,1"
            };
            int doses = 100;
            bool vaccinateChildren = false;

            // Act
            string[] output = Program.CreateVaccinationOrder(input, doses, vaccinateChildren);

            // Assert
            Assert.AreEqual(2, output.Length);
            Assert.AreEqual("19720906-1111,Smith,John,1", output[0]);
            Assert.AreEqual("19810203-2222,Johnson,Emily,2", output[1]);
        }

        [TestMethod]
        public void TestInvalidInput()
        {
            // Arrange
            string[] input =
            {
                "InvalidInput",
                "1-2-3-4,Jonasson,Jonas,0,1,0",
            };
            int doses = 10;
            bool vaccinateChildren = false;

            // Act
            string[] output = Program.CreateVaccinationOrder(input, doses, vaccinateChildren);

            // Assert
            Assert.AreEqual(0, output.Length);
        }

        [TestMethod]
        public void TestZeroDoses()
        {
            // Arrange
            string[] input =
            {
                "19720906-1111,Smith,John,0,0,1"
            };
            int doses = 0;
            bool vaccinateChildren = true;

            // Act
            string[] output = Program.CreateVaccinationOrder(input, doses, vaccinateChildren);

            // Assert
            Assert.AreEqual(1, output.Length);
            Assert.AreEqual("19720906-1111,Smith,John,0", output[0]);
        }

        [TestMethod]
        public void TestEdgeCase()
        {
            // Arrange
            string[] input =
            {
                "19720906-1111,Smith,John,1,1,1",
                "8102032222,Johnson,Emily,1,1,1"
            };
            int doses = 1;
            bool vaccinateChildren = true;

            // Act
            string[] output = Program.CreateVaccinationOrder(input, doses, vaccinateChildren);

            // Assert
            Assert.AreEqual(2, output.Length);
            Assert.AreEqual("19720906-1111,Smith,John,1", output[0]);
            Assert.AreEqual("19810203-2222,Johnson,Emily,0", output[1]);
        }

        [TestMethod]
        public void TestNoInputData()
        {
            //Arrange
            string[] input = {};
            int doses = 10;
            bool vaccinateChildren = true;

            // Act
            string[] output = Program.CreateVaccinationOrder(input, doses, vaccinateChildren);
            
            // Assert
            Assert.AreEqual(0, output.Length);
        }

        
    }
}