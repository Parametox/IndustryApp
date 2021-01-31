using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Start");

            try
            {
                WCF.ServiceClient sc = new WCF.ServiceClient();
                WCF.TemperatureCollection x = sc.GetFourMunutesTemperature();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            
            }

            Console.WriteLine("Stop");
            Console.ReadKey();
        }
    }
}
