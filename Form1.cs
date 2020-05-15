using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace $safeprojectname$
{
    public partial class Form1 : Form
    {
        class MyDictionary
        {
            public string key { get; set; }
            public int listIndex { get; set; }
        }

        List<MyDictionary> twoLetter = new List<MyDictionary>();
        List<MyDictionary> threeLetter = new List<MyDictionary>();
        List<string> names = new List<string>();
        Stopwatch loadWatch = new Stopwatch();
        Stopwatch createWatch = new Stopwatch();

        public Form1()
        {
            //Console.InputEncoding = System.Text.Encoding.UTF8;
            //Console.OutputEncoding = System.Text.Encoding.UTF8;
            InitializeComponent();
            openFileDialog1.Title = "wybierz plik txt";
            openFileDialog1.InitialDirectory = Environment.CurrentDirectory;
            openFileDialog1.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
            openFileDialog1.ShowDialog();
            loadWatch.Start();
            var fileLines = from line in File.ReadLines(openFileDialog1.FileName, Encoding.UTF8)
                            where (!string.IsNullOrWhiteSpace(line))
                            select new
                            {
                                Line = line.Split(' ')
                            };
            loadWatch.Stop();

            createWatch.Start();
            foreach (var item in fileLines)
            {
                var str = item.Line[1];
                names.Add(str);
                var count = names.Count - 1;
                twoLetter.Add(new MyDictionary { key = str.Substring(0, 2), listIndex = count });
                str += ((str.Length < 4)?"xyz" : "");
                threeLetter.Add(new MyDictionary { key = str.Substring(0, 3), listIndex = count });
            }
            createWatch.Stop();

            label1.Text = "Wczytywanie: " + loadWatch.Elapsed;
            label2.Text = "Struktury: " + createWatch.Elapsed;
        }

        private void comboBox1_TextChanged(object sender, EventArgs e)
        {
            var help = comboBox1.SelectionStart;
            comboBox1.Items.Clear();
            comboBox1.SelectionStart = help;
            var text = comboBox1.Text;
            var length = text.Length;
            switch (length)
            {
                case 0:
                    break;
                case 1:
                    break;
                case 2:
                    PopulateTwo(text);
                    break;
                case 3:
                    PopulateThree(text);
                    break;
                default:
                    PopulateMore(text);
                    break;
            }
            comboBox1.DroppedDown = true;
            Cursor.Current = Cursors.Default;
        }

        private void PopulateTwo(string text)
        {
            ILookup<string, int> elementLookup = twoLetter.ToLookup(
                p => p.key,
                p => p.listIndex
            );
            IEnumerable<int> index = elementLookup[text];
            foreach (var item in index)
            {
                comboBox1.Items.Add(names[item]);
            }
        }

        private void PopulateThree(string text)
        {
            ILookup<string, int> elementLookup = threeLetter.ToLookup(
                p => p.key,
                p => p.listIndex
            );
            IEnumerable<int> index = elementLookup[text];
            foreach (var item in index)
            {
                comboBox1.Items.Add(names[item]);
            }
        }

        private void PopulateMore(string text)
        {
            IEnumerable<string> name =
                from surname in names
                where surname.Contains(text)
                select surname;

            foreach (var item in name)
            {
                comboBox1.Items.Add(item);
            }
        }
    }
}
