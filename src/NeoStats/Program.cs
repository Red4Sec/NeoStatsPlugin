using System;
using System.IO;
using System.Linq;
using NeoStats.Core;
using NeoStats.Extensions;

namespace NeoStats
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                args = new string[] { @"sample.json" };
            }

            var stats = args.Select(file =>
            {
                var ret = BlockStatCollection.FromBlockArray(File.ReadAllText(file));
                ret.Title = Path.GetFileNameWithoutExtension(file);

                return ret;
            }
            ).ToArray();

            File.WriteAllText("block-chart.html", stats.DoTimePerBlockChart());

            Console.WriteLine("Done!");
        }
    }
}