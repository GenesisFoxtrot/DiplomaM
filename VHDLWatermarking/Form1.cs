using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;

namespace VHDLWatermarking
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {

            using (var sr = new StreamReader(@"D:\Space\ABCC\ABC.vhd"))
            {
                string line = sr.ReadToEnd();
                richTextBox1.Text = line;

                var commaneXST = "xst " +
                                 "-intstyle ise " +
                                 "-ifn \"mealy_4s.xst\" " +
                                 "-ofn \"mealy_4s.syr\"";
                var commandNgbuild = "ngdbuild " +
                                     "-intstyle ise " +
                                     "-dd _ngo -nt timestamp " +
                                     "-i -p xc3s50-pq208-5 mealy_4s.ngc mealy_4s.ngd";
                var map = "map " +
                          "-intstyle ise " +
                          "-p xc3s50-pq208-5 " +
                          "-cm area -ir off " +
                          "-pr off " +
                          "-c 100 " +
                          "-o mealy_4s_map.ncd mealy_4s.ngd mealy_4s.pcf";
                var netgen = "netgen " +
                             "-intstyle ise " +
                             "-s 5  -pcf mealy_4s.pcf " +
                             "-rpw 100 " +
                             "-tpw 0 " +
                             "-ar Structure " +
                             "-tm mealy_4s " +
                             "-insert_pp_buffers true" +
                             " -w -dir netgen/par " +
                             "-ofmt vhdl " +
                             "-sim mealy_4s_map.ncd mealy_4s_timesim.vhd";
                Process.Start("cmd.exe", @"/C cd D:\2\" + "&" + commaneXST + "&"
                                     +commandNgbuild + "&"
                                     + map + "&"
                                     + netgen + "&pause");
                //Process.Start(@"D:\Space\XSI\14.7\ISE_DS\ISE\bin\nt\unwrapped\netgen.exe", strCmdText);

            }
        }

        private const string RegularBeh = "(?<=begin)[\n,a-z,A-Z, _ :;()=>0-9'\"]+(?=end Structure;)";
        private const string RegularOneAction = "[a-z,A-Z_ 0-9]+ : [a-z,A-Z()=>0-9_, \"\'\n]+(?=;)";
        private const string RegularTitle = "[a-z,A-Z_ 0-9]+ : [a-z,A-Z_0-9]+";
        private const string RegularMaps = @"(?<=map( )?\()[a-zA-Z0-9=>, _""\n]+(?=\))";
        private void button2_Click(object sender, EventArgs e)
        {
            using (var sr = new StreamReader(@"D:\2\netgen\par\mealy_4s_timesim.vhd"))
            {
                string line = sr.ReadToEnd();
                var c = Regex.Matches(line, RegularBeh);
                var d = Regex.Matches(c[0].ToString(), RegularOneAction);
                string str="";
                int i = 1;
                foreach (var dd in d)
                {
                    var g = Regex.Matches(dd.ToString(), RegularTitle);
                    var title = g[0].ToString().Split(':');
                    str += (i++) + " " + title[0] + "  " + title[1] + "\n";
                    var ports = Regex.Matches(dd.ToString(), RegularMaps);
                    for (int j = 0; j < ports.Count; j++)
                    {
                        str += "\n" + ports[j] + "\n";
                    }

                }
                richTextBox1.Text = str;
            }
        }
    }
}
