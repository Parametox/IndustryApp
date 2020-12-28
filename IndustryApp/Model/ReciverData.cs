using System;
using System.Collections.Generic;
using System.Text;

namespace IndustryApp.Model
{
    public class ReciverData
    {
        public string Temperature { get; set; }
        public string Address { get; set; }
        public string Date { get; set; }

        public ReciverData()
        {
            this.Date = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        }
    }
}
