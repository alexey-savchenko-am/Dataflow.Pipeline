using System;

namespace SampleProject
{
    class Program
    {
        static void Main(string[] args)
        {
            var app = new Application();
            var result = app.StartAsync().GetAwaiter().GetResult();
            
            Console.WriteLine(result);
            Console.ReadKey();
        }
    }
}