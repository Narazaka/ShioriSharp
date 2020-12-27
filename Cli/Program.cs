using System;
using ShioriSharp;
using ShioriSharp.Message;

namespace Cli {
    class Program {
        static void Main(string[] args) {
            Console.WriteLine(Request.GET(new() { Reference1 = "1" }).ToString());
            Console.WriteLine(Response.OK(new() { Reference1 = "1" }).ToString());
            Console.WriteLine("Hello World!");
        }
    }
}
