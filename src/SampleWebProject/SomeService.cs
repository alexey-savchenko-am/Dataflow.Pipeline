using System;
using System.Linq;

namespace SampleWebProject
{
    public class SomeService
        : ISomeService
    {
        private Random random = new Random();
        
        public string GetSomeString(int len)
        {
            return RandomString(len);
        }
        
        public string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }
        
    }
}