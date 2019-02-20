using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace WpfApp1
{
    class WriteBinFile
    {
        public WriteBinFile()
        {
        }


        public static void writeFile(String data)
        {
            FileStream writeStream;
            try
            {
                writeStream = new FileStream("TestStream", FileMode.Append);
                BinaryWriter writeBinary = new BinaryWriter(writeStream);
                    writeBinary.Write(data);

                writeBinary.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
    }
}
