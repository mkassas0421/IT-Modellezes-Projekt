﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace CsordasBarna_BeadandoDolgozat
{
    public partial class FormAdminFelület : Form
    {
        List<Sajat> lista = new List<Sajat>();

        [DllImport("Gdi32.dll", EntryPoint = "CreateRoundRectRgn")]
        private static extern IntPtr CreateRoundRectRgn
        (
            int nLeftRect,     // x-coordinate of upper-left corner
            int nTopRect,      // y-coordinate of upper-left corner
            int nRightRect,    // x-coordinate of lower-right corner
            int nBottomRect,   // y-coordinate of lower-right corner
            int nWidthEllipse, // height of ellipse
            int nHeightEllipse // width of ellipse
        );

        public FormAdminFelület()
        {
            InitializeComponent();
            

            //form oldalainak kerekítése (formázás)
            this.FormBorderStyle = FormBorderStyle.None;
            Region = System.Drawing.Region.FromHrgn(CreateRoundRectRgn(0, 0, Width, Height, 20, 20));
            pnlNav.Height = btnKezdolap.Height;
            pnlNav.Top = btnKezdolap.Top;
            pnlNav.Left = btnKezdolap.Left;
            btnKezdolap.BackColor = Color.FromArgb(46, 51, 73);
        }

        private void btnKilép_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Biztosan ki szeretne lépni?","Figyelem!",MessageBoxButtons.YesNo)==DialogResult.Yes)
            {
                this.Close();
            }
        }

        private void sajátTermékekToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormSajatTermekBevitel frm = new FormSajatTermekBevitel(lista);
            if (frm.ShowDialog()==DialogResult.OK)
            {
                Sajat t = lista[lista.Count - 1];
                //lvOutput.Items.Add(t.Térköz());
                t.Courier(lvOutput, t.Térköz());

                StreamWriter wr = new StreamWriter("Termekek.txt", true, Encoding.UTF8);
                wr.WriteLine(t.FájlMentes());
                wr.Close();                                             
            }
        }

        private void beszerzettTermékekToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormBeszerzettTermekBevitel frm = new FormBeszerzettTermekBevitel(lista);
            if (frm.ShowDialog() == DialogResult.OK)
            {
                Beszerzett t = lista[lista.Count - 1] as Beszerzett;
                //lvOutput.Items.Add(t.Térköz());
                t.Courier(lvOutput, t.Térköz());

                StreamWriter wr = new StreamWriter("Termekek.txt", true, Encoding.UTF8);
                wr.WriteLine(t.FájlMentes());
                wr.Close();
            }
        }

        private void FormAdminFelület_Load(object sender, EventArgs e)
        {
            
            try
            {
                StreamReader rd = new StreamReader("Termekek.txt", Encoding.UTF8);
                while (!rd.EndOfStream)
                {
                    string[] s = rd.ReadLine().Split(';');
                    if (s.Length==5)
                    {
                        //sajat termek
                        Sajat t = new Sajat(s[0], s[1], Convert.ToInt32(s[2]), Convert.ToInt32(s[3]), s[4]);
                        t.Courier(lvOutput, t.Térköz());
                    }
                    else
                    {
                        //beszerzett termek
                        Beszerzett t = new Beszerzett(s[0], s[1], Convert.ToInt32(s[2]), Convert.ToInt32(s[3]), s[4], s[5]);
                        t.Courier(lvOutput, t.Térköz());
                    }
                }
                rd.Close();
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            lvOutput.Clear();
            StreamReader rd = new StreamReader("Termekek.txt", Encoding.UTF8);
            while (!rd.EndOfStream)
            {
                string[] s = rd.ReadLine().Split(';');
                if (s.Length == 5)
                {
                    //sajat termek
                    Sajat t = new Sajat(s[0], s[1], Convert.ToInt32(s[2]), Convert.ToInt32(s[3]), s[4]);
                    t.Courier(lvOutput, t.Térköz());
                }
            }
        }
        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            lvOutput.Clear();
            StreamReader rd = new StreamReader("Termekek.txt", Encoding.UTF8);
            while (!rd.EndOfStream)
            {
                string[] s = rd.ReadLine().Split(';');
                if (s.Length == 5)
                {
                    //sajat termek
                    Sajat t = new Sajat(s[0], s[1], Convert.ToInt32(s[2]), Convert.ToInt32(s[3]), s[4]);
                    t.Courier(lvOutput, t.Térköz());
                }
                else
                {
                    //beszerzett termek
                    Beszerzett t = new Beszerzett(s[0], s[1], Convert.ToInt32(s[2]), Convert.ToInt32(s[3]), s[4], s[5]);
                    t.Courier(lvOutput, t.Térköz());
                }
            }
            rd.Close();
        }

        private void radioButton3_CheckedChanged(object sender, EventArgs e)
        {
            lvOutput.Clear();
            StreamReader rd = new StreamReader("Termekek.txt", Encoding.UTF8);
            while (!rd.EndOfStream)
            {
                string[] s = rd.ReadLine().Split(';');
                if (s.Length == 6)
                {
                    Beszerzett t = new Beszerzett(s[0], s[1], Convert.ToInt32(s[2]), Convert.ToInt32(s[3]), s[4], s[5]);
                    t.Courier(lvOutput, t.Térköz());
                }               
            }
            rd.Close();
        }

        private void btnTörlés_Click(object sender, EventArgs e)
        {
           
            }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            panel1.BackColor = Color.FromArgb(36, 39, 38);
            panel2.BackColor = Color.FromArgb(30, 40, 49);
            radioButton2.Checked = true;
            FormSajatTermekBevitel fsajat = new FormSajatTermekBevitel();
            fsajat.ShowDialog();
        }

        private void label2_Click(object sender, EventArgs e)
        {
            
        }

        private void label1_Click(object sender, EventArgs e)
        {
            panel1.BackColor = Color.FromArgb(36,39,40);
            panel2.BackColor = Color.FromArgb(30, 40, 49);
            radioButton2.Checked = true;
            FormSajatTermekBevitel fsajat = new FormSajatTermekBevitel();
            fsajat.ShowDialog();
        }

        private void label2_Click_1(object sender, EventArgs e)
        {
            panel1.BackColor = Color.FromArgb(30, 40, 49);
            panel2.BackColor = Color.FromArgb(36, 39, 40);
            radioButton3.Checked = true;
            FormBeszerzettTermekBevitel frm = new FormBeszerzettTermekBevitel();
            frm.ShowDialog();
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            panel1.BackColor = Color.FromArgb(30, 40, 49);
            panel2.BackColor = Color.FromArgb(36, 39, 40);
            radioButton3.Checked = true;
            FormBeszerzettTermekBevitel frm = new FormBeszerzettTermekBevitel();
            frm.ShowDialog();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            btnKezdolap.BackColor = Color.FromArgb(24, 30, 54);
            pnlNav.Height = button1.Height;
            pnlNav.Top = button1.Top;
            //pnlNav.Left = button1.Left;
            button1.BackColor = Color.FromArgb(46, 51, 73);
        }

        private void btnKezdolap_Click(object sender, EventArgs e)
        {
            pnlNav.Height = btnKezdolap.Height;
            pnlNav.Top = btnKezdolap.Top;
            pnlNav.Left = btnKezdolap.Left;
            btnKezdolap.BackColor = Color.FromArgb(46,51,73);
        }

        private void btnKezdolap_Leave(object sender, EventArgs e)
        {
            btnKezdolap.BackColor = Color.FromArgb(24, 30, 54);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            btnKezdolap.BackColor = Color.FromArgb(24, 30, 54);
            pnlNav.Height = button2.Height;
            pnlNav.Top = button2.Top;
            //pnlNav.Left = button1.Left;
            button2.BackColor = Color.FromArgb(46, 51, 73);
        }

        private void button2_Leave(object sender, EventArgs e)
        {
            button2.BackColor = Color.FromArgb(24, 30, 54);
        }

        private void button1_Leave(object sender, EventArgs e)
        {
            button1.BackColor = Color.FromArgb(24, 30, 54);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            btnKezdolap.BackColor = Color.FromArgb(24, 30, 54);
            pnlNav.Height = button3.Height;
            pnlNav.Top = button3.Top;
            //pnlNav.Left = button1.Left;
            button3.BackColor = Color.FromArgb(46, 51, 73);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            btnKezdolap.BackColor = Color.FromArgb(24, 30, 54);
            pnlNav.Height = button4.Height;
            pnlNav.Top = button4.Top;
            //pnlNav.Left = button1.Left;
            button4.BackColor = Color.FromArgb(46, 51, 73);
        }

        private void button3_Leave(object sender, EventArgs e)
        {
            button3.BackColor = Color.FromArgb(24, 30, 54);
        }

        private void button4_Leave(object sender, EventArgs e)
        {
            button4.BackColor = Color.FromArgb(24, 30, 54);
        }
    }           
        }
    

