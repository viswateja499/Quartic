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
            processor.ReadAndSetFileAsync(path);

            processor.ReadRulesFromUser();
            Console.WriteLine("Validating");
            processor.ValidateData();
            Console.ReadLine();
        }
    }
}
