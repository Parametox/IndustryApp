using System;
using System.Collections.Generic;
using System.Text;

namespace IndustryApp.Model
{
    public class ChartSelector
    {
        public short ID { get; set; }
        public string Name { get; set; }

        private static short counter = 1;
         
        public ChartSelector()
        {
            //this.ID = counter++;
            this.Name = "";
        }
    }
}
