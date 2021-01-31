using System;
using System.Collections.Generic;
using System.Text;

namespace IndustryApp.Model
{
    public class ChartSelector
    {
        public short ID { get; set; }
        public string Name { get; set; }
         
        public ChartSelector()
        {
            this.Name = "";
        }
    }
}
