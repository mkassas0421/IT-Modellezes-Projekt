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
using System.Collections;
using System.Collections.Generic;

namespace CsordasBarna_BeadandoDolgozat
{
    public partial class FormAdminFelület : Form
    {
        List<Sajat> lista = new List<Sajat>();
        static int beolvasottTermekekSzama;
        static Stack<Sajat> termekek = new Stack<Sajat>();

        private bool isCollapsed;
        private bool btnUjtermClicked = false;
        private bool btnUjidegenClicked = false;
        int szamlalo = 0; //a lenyíló menüt kattintásait számlálja, hogy minden második kattintásra összecsukódjon a lenyiló menü a terméklista gombbnál

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
            pictureBox6.BackColor = Color.FromArgb(46, 51, 73);
        }
        protected override void WndProc(ref Message m)
        {
            switch (m.Msg)
            {
                case 0x84:
                    base.WndProc(ref m);
                    if ((int)m.Result == 0x1)
                        m.Result = (IntPtr)0x2;
                    return;
            }

            base.WndProc(ref m);

        }

        private void btnKilép_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Biztosan ki szeretne lépni?", "Figyelem!", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                this.Close();
            }
        }

        private void sajátTermékekToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormSajatTermekBevitel frm = new FormSajatTermekBevitel(lista);
            if (frm.ShowDialog() == DialogResult.OK)
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
                //ez a két lista egyelőre nincs használatban csak elmentődnek külön-külön a két osztály beolvasott tagjai
                List<Sajat> beolvasottSajatTermekLista = new List<Sajat>();
                List<Beszerzett> beolvasottBeszerzettTermekLista = new List<Beszerzett>();

                Stack<Sajat> stack = new Stack<Sajat>();
                beolvasottTermekekSzama = 0;
                StreamReader rd = new StreamReader("Termekek.txt", Encoding.UTF8);
                while (!rd.EndOfStream)
                {
                    string[] s = rd.ReadLine().Split(';');
                    if (s.Length == 5)
                    {
                        //sajat termek
                        Sajat t = new Sajat(s[0], s[1], Convert.ToInt32(s[2]), Convert.ToInt32(s[3]), s[4]);
                        beolvasottSajatTermekLista.Add(t);
                        stack.Push(t);
                        beolvasottTermekekSzama++;
                        t.Courier(lvOutput, t.Térköz());

                    }
                    else
                    {
                        //beszerzett termek
                        Beszerzett t = new Beszerzett(s[0], s[1], Convert.ToInt32(s[2]), Convert.ToInt32(s[3]), s[4], s[5]);
                        beolvasottBeszerzettTermekLista.Add(t);
                        stack.Push(t);
                        beolvasottTermekekSzama++;
                        t.Courier(lvOutput, t.Térköz());

                    }
                }
                rd.Close();
                //lvLegutobbi feltöltése Stackből
                int n = stack.Count;
                for (int i = 0; i < n; i++)
                {
                    Sajat obj = stack.Peek();
                    termekek.Push(obj);
                    Sajat sajat = stack.Pop();
                    sajat.Courier(lvLegutobbi, sajat.LegutobbiListabaKiiratas(sajat));
                }
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
            radioButton2.Checked = true;
            FormSajatTermekBevitel fsajat = new FormSajatTermekBevitel();
            fsajat.ShowDialog();

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

            radioButton2.Checked = true;
            FormSajatTermekBevitel fsajat = new FormSajatTermekBevitel();
            fsajat.ShowDialog();
        }

        private void label2_Click_1(object sender, EventArgs e)
        {
            radioButton3.Checked = true;
            FormBeszerzettTermekBevitel frm = new FormBeszerzettTermekBevitel();
            frm.ShowDialog();
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {

            radioButton3.Checked = true;
            FormBeszerzettTermekBevitel frm = new FormBeszerzettTermekBevitel();
            frm.ShowDialog();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            szamlalo++;

            comboboxFelvitt.Visible = true;
            lblLegutobbFelvitt.Enabled = true;
            lblLegutobbFelvitt.Visible = true;
            btnKezdolap.BackColor = Color.FromArgb(24, 30, 54);
            pnlNav.Height = button1.Height;
            pnlNav.Top = button1.Top;
            pnlNav.Left = button1.Left;
            button1.BackColor = Color.FromArgb(46, 51, 73);


            //panelek megjelenítése
            lvLegutobbi.Enabled = true;
            lvLegutobbi.Visible = true;
            lvOutput.Visible = true;
            radioButton1.Visible = true;
            radioButton2.Visible = true;
            radioButton3.Visible = true;

            //2 új button lenyitása a terméklista gomb alá
            btnUjtermek.Visible = true;
            btnUjidegentermek.Visible = true;
            btnUjtermek.Location = new Point(0, 250);
            btnUjidegentermek.Location = new Point(0, 300);
            button2.Location = new Point(0, 350);
            button3.Location = new Point(0, 400);
            pictureBox4.Location = new Point(175, 363);
            pictureBox5.Location = new Point(175, 413);


            if (szamlalo % 2 == 0) {
                btnUjtermek.Visible = false;
                btnUjidegentermek.Visible = false;
                button2.Location = new Point(0, 250);
                button3.Location = new Point(0, 300);
                pictureBox4.Location = new Point(175, 263);
                pictureBox5.Location = new Point(175, 313);
            }


            //timer1.Start();
            //drop_down(sender,e);
        }

        private void btnKezdolap_Click(object sender, EventArgs e)
        {
            //Termékfelvitelpanelen lévő dolgok eltűntetése
            pnlFelvitel.Visible = false;
            lblLegutobbFelvitt.Visible = false;
            lvLegutobbi.Visible = false;
            lvOutput.Visible = false;
            lvOutput.Visible = false;
            radioButton1.Visible = false;
            radioButton2.Visible = false;
            radioButton3.Visible = false;

            szamlalo = 0;
            //Terméklista leugró fül visszacsukása
            if (szamlalo % 2 == 0)
            {
                btnUjtermek.Visible = false;
                btnUjidegentermek.Visible = false;
                button2.Location = new Point(0, 250);
                button3.Location = new Point(0, 300);
                pictureBox4.Location = new Point(175, 263);
                pictureBox5.Location = new Point(175, 313);
            }


            pictureBox6.BackColor = Color.Transparent;

            pnlNav.Height = btnKezdolap.Height;
            pnlNav.Top = btnKezdolap.Top;
            pnlNav.Left = btnKezdolap.Left;
            btnKezdolap.BackColor = Color.FromArgb(46, 51, 73);
        }

        private void btnKezdolap_Leave(object sender, EventArgs e)
        {
            btnKezdolap.BackColor = Color.FromArgb(24, 30, 54);
            pictureBox6.BackColor = Color.FromArgb(24, 30, 54);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            //Termékfelvitelpanelen lévő dolgok eltűntetése
            pnlFelvitel.Visible = false;
            lblLegutobbFelvitt.Visible = false;
            lvLegutobbi.Visible = false;
            lvOutput.Visible = false;
            lvOutput.Visible = false;
            radioButton1.Visible = false;
            radioButton2.Visible = false;
            radioButton3.Visible = false;
            szamlalo = 0;
            //Terméklista leugró fül visszacsukása
            if (szamlalo % 2 == 0)
            {
                btnUjtermek.Visible = false;
                btnUjidegentermek.Visible = false;
                button2.Location = new Point(0, 250);
                button3.Location = new Point(0, 300);
                pictureBox4.Location = new Point(175, 263);
                pictureBox5.Location = new Point(175, 313);
            }

            btnKezdolap.BackColor = Color.FromArgb(24, 30, 54);
            pnlNav.Height = button2.Height;
            pnlNav.Top = button2.Top;
            //pnlNav.Left = button1.Left;
            button2.BackColor = Color.FromArgb(46, 51, 73);
            pictureBox4.BackColor = Color.FromArgb(46, 51, 73);
        }

        private void button2_Leave(object sender, EventArgs e)
        {
            button2.BackColor = Color.FromArgb(24, 30, 54);
            pictureBox4.BackColor = Color.FromArgb(24, 30, 54);
        }

        private void button1_Leave(object sender, EventArgs e)
        {
            szamlalo++;
            button1.BackColor = Color.FromArgb(24, 30, 54);



            //leugró 2 button összecsukása
            /*ide kell egy feltételes elágazás, hogy ez csak akkor következzen be hogyha nem a btnUjtermek_Click vagy nem a btnUjidegentermek_Click-re megyünk
            mert különben nem lehet megnyitni az adatfelvivős formokat*/
            if (btnUjidegenClicked || btnUjtermClicked)
            {
                btnUjtermek.Visible = false;
                btnUjidegentermek.Visible = false;
                button2.Location = new Point(0, 250);
                button3.Location = new Point(0, 300);
                pictureBox4.Location = new Point(175, 263);
                pictureBox5.Location = new Point(175, 313);
            }
            btnUjidegenClicked = false;
            btnUjtermClicked = false;


        }

        private void button3_Click(object sender, EventArgs e)
        {
            //Termékfelvitelpanelen lévő dolgok eltűntetése
            pnlFelvitel.Visible = false;
            lvLegutobbi.Visible = false;
            lblLegutobbFelvitt.Visible = false;
            lvOutput.Visible = false;
            lvOutput.Visible = false;
            radioButton1.Visible = false;
            radioButton2.Visible = false;
            radioButton3.Visible = false;
            szamlalo = 0;
            //Terméklista leugró fül visszacsukása
            if (szamlalo % 2 == 0)
            {
                btnUjtermek.Visible = false;
                btnUjidegentermek.Visible = false;
                button2.Location = new Point(0, 250);
                button3.Location = new Point(0, 300);
                pictureBox4.Location = new Point(175, 263);
                pictureBox5.Location = new Point(175, 313);
            }


            btnKezdolap.BackColor = Color.FromArgb(24, 30, 54);
            pnlNav.Height = button3.Height;
            pnlNav.Top = button3.Top;
            //pnlNav.Left = button1.Left;
            button3.BackColor = Color.FromArgb(46, 51, 73);
            pictureBox5.BackColor = Color.FromArgb(46, 51, 73);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            //Termékfelvitelpanelen lévő dolgok eltűntetése
            pnlFelvitel.Visible = false;
            lblLegutobbFelvitt.Visible = false;
            lvLegutobbi.Visible = false;
            lvOutput.Visible = false;
            lvOutput.Visible = false;
            radioButton1.Visible = false;
            radioButton2.Visible = false;
            radioButton3.Visible = false;
            szamlalo = 0;
            //Terméklista leugró fül visszacsukása
            if (szamlalo % 2 == 0)
            {
                btnUjtermek.Visible = false;
                btnUjidegentermek.Visible = false;
                button2.Location = new Point(0, 250);
                button3.Location = new Point(0, 300);
                pictureBox4.Location = new Point(175, 263);
                pictureBox5.Location = new Point(175, 313);
            }


            btnKezdolap.BackColor = Color.FromArgb(24, 30, 54);
            pnlNav.Height = button4.Height;
            pnlNav.Top = button4.Top;
            //pnlNav.Left = button1.Left;
            button4.BackColor = Color.FromArgb(46, 51, 73);
        }

        private void button3_Leave(object sender, EventArgs e)
        {
            button3.BackColor = Color.FromArgb(24, 30, 54);
            pictureBox5.BackColor = Color.FromArgb(24, 30, 54);
        }

        private void button4_Leave(object sender, EventArgs e)
        {
            button4.BackColor = Color.FromArgb(24, 30, 54);
        }

        /*
        private void timer1_Tick(object sender, EventArgs e)
        {
            if (isCollapsed)
            {
                panel5.Height = panel5.MaximumSize.Height;
                if (panel5.Size == panel5.MaximumSize)
                {
                    timer1.Stop();
                    isCollapsed = false;
                }
            }
            else
            {
                panel5.Height = panel5.MinimumSize.Height;
                if (panel5.Size == panel5.MinimumSize)
                {
                    timer1.Stop();
                    isCollapsed = true;
                }
            }
        }
        */
        //private void drop_down(object sender, EventArgs e)
        //{
        //    if (isCollapsed)
        //    {
        //        panel5.Height = panel5.MaximumSize.Height;
        //        if (panel5.Size == panel5.MaximumSize)
        //        {
        //            isCollapsed = false;
        //        }
        //    }
        //    else
        //    {
        //        panel5.Height = panel5.MinimumSize.Height;
        //        if (panel5.Size == panel5.MinimumSize)
        //        {
        //            isCollapsed = true;
        //        }
        //    }
        //}

        private void pictureBox6_Click(object sender, EventArgs e)
        {

        }

        private void btnUjtermek_Click(object sender, EventArgs e)
        {
            lvOutput.Visible = true;
            FormSajatTermekBevitel frm = new FormSajatTermekBevitel() { Dock = DockStyle.Fill, TopLevel = false, TopMost = true };
            this.pnlFelvitel.Controls.Clear();
            this.pnlFelvitel.Controls.Add(frm);
            frm.Show();
            btnUjtermClicked = true;

            //panel láthatósága és a sajatTermekFelvitelForm megnyitása bele
            pnlFelvitel.Visible = true;

        }

        private void btnUjidegentermek_Click(object sender, EventArgs e)
        {
            lvOutput.Visible = true;
            pnlFelvitel.Visible = true;

            FormBeszerzettTermekBevitel frm = new FormBeszerzettTermekBevitel() { Dock = DockStyle.Fill, TopLevel = false, TopMost = true };
            this.pnlFelvitel.Controls.Clear();
            this.pnlFelvitel.Controls.Add(frm);
            frm.Show();
            btnUjidegenClicked = true;
        }

        private void btnUjidegentermek_Leave(object sender, EventArgs e)
        {


        }

        private void btnUjtermek_Leave(object sender, EventArgs e)
        {

        }

        private void lvOutput_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void pnlNav_Paint(object sender, PaintEventArgs e)
        {

        }

        private void pictureBox1_Click_1(object sender, EventArgs e)
        {

        }

        private void pictureBox6_Click_1(object sender, EventArgs e)
        {

        }

        private void pictureBox4_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox5_Click(object sender, EventArgs e)
        {

        }

        private void panel3_Paint(object sender, PaintEventArgs e)
        {

        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {

        }

        private void lblFelhasznalo_Click(object sender, EventArgs e)
        {

        }

        private void panel4_Paint(object sender, PaintEventArgs e)
        {

        }

        private void comboboxFelvitt_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (comboboxFelvitt.SelectedIndex)
            {

                //NULLREFERENCE EXCEPTION

                case 0:
                    lvLegutobbi.Clear();
                    for (int i = 0; i < beolvasottTermekekSzama; i++)
                    {
                        Sajat sajat = termekek.Pop();
                        //termekek.Push(sajat);
                        lvLegutobbi.Clear();
                        sajat.Courier(lvLegutobbi, sajat.LegutobbiListabaKiiratas(sajat));
                    }
                    break;
                case 1:
                    lvLegutobbi.Clear();
                    for (int i = 0; i < 5; i++)
                    {
                        Sajat sajat = termekek.Pop();
                        //termekek.Push(sajat);                        
                        sajat.Courier(lvLegutobbi, sajat.LegutobbiListabaKiiratas(sajat));
                    }
                    break;
                case 2:
                    lvLegutobbi.Clear();
                    for (int i = 0; i < 10; i++)
                    {
                        Sajat sajat = termekek.Pop();
                        //termekek.Push(sajat);
                        sajat.Courier(lvLegutobbi, sajat.LegutobbiListabaKiiratas(sajat));
                    }
                    break;
                case 3:
                    lvLegutobbi.Clear();
                    for (int i = 0; i < 20; i++)
                    {
                        Sajat sajat = termekek.Pop();
                        //termekek.Push(sajat);
                        sajat.Courier(lvLegutobbi, sajat.LegutobbiListabaKiiratas(sajat));
                    }
                    break;
            }
        }
        }
    } 
    

