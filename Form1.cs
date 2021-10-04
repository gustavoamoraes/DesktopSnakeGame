using System;
using System.Threading;
using System.Windows.Forms;

namespace DesktopSnakeGame
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            
            InitializeComponent();
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            new Thread(GameManager.StartGameLoop).Start();
            stop_button.Enabled = true;
            start_button.Enabled = false;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            GameManager.Stop();
            start_button.Enabled = true;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            stop_button.Enabled = false;
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            GameManager.Stop();
        }
    }
}
