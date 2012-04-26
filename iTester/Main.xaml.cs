using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using DBH.Helper;
using System.Diagnostics;
using System.Threading;

namespace iTester
{
    /// <summary>
    /// Main.xaml 的交互逻辑
    /// </summary>
    public partial class Main : Window
    {
        public Main()
        {
            InitializeComponent();
           
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            TextRange textRange = new TextRange(richTextBox1.Document.ContentStart, richTextBox1.Document.ContentEnd);
            string sql = textRange.Text;
            string output = string.Empty;

            Stopwatch sw = new Stopwatch();
            sw.Start();
            int count = Convert.ToInt32(textBox1.Text);
            int tmp = count;
            if (checkBox1.IsChecked == true)
            {
                
                while (count > 0)
                {
                    //DBHelperTest.All["default"].ExecuteQuery(sql);

                    //DBHelper.ExecuteQuery(sql);
                    
                    count--;
                    textBox1.Text = count.ToString();
                }


               

            }

            sw.Stop();


            string otpt = string.Format("{0} querys finished in {1}", tmp, sw.Elapsed.ToString());
            richTextBox2.AppendText(otpt);
            richTextBox2.AppendText("\n");

            



            MessageBox.Show(textRange.Text);
        }
    }
}
