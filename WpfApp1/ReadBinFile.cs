using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1
{
    class ReadBinFile
    {
     
        public ReadBinFile()
        {
        }

        public static List<String> readFile(String filename)
        {
            List<String> content = new List<string>();
            int counter = 0;
            string line;

            // Read the file and display it line by line.
            System.IO.StreamReader file =
               new System.IO.StreamReader(filename);
            while ((line = file.ReadLine()) != null)
            {
                content.Add(line);
                counter++;
            }

            file.Close();

            // Suspend the screen.
            return content;
        }
    }
}
