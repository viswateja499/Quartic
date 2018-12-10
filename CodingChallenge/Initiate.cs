using System;

namespace CodingChallenge
{
    public class Initiate
    {
        public static void Main()
        {
            Console.WriteLine("Please Enter File Path");
            string path = Console.ReadLine();
            Processor processor = new Processor();
            while (!processor.ReadAndSetFile(path))
            {
                Console.WriteLine("Please Enter File Path");
                path = Console.ReadLine();
            }

            processor.ReadRulesFromUser();
            Console.WriteLine("Validating");
            processor.ValidateData();

            Console.ReadLine();
        }
    }
}
