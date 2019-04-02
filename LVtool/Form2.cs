using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LVtool
{
    public partial class Form2 : Form
    {
        public int select_mode = -1;

        public Form2()
        {
            InitializeComponent();

        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (radioButton1.Checked) select_mode = 1;
            else if (radioButton2.Checked) select_mode = 2;
            else if (radioButton3.Checked) select_mode = 3;
            else select_mode = -1;

        }
    }
}
