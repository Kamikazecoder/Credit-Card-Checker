using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;
using System.Media;
using System.Diagnostics;
/*
 * Created By Kamikaze.coder;
 * https://github.com/Kamikazecoder
 */
namespace ChkKMKZ
{
    public partial class Form1 : Form
    {
        private int _nApproved;
        private int _nReproved;
        public Form1()
        {
            _nApproved = 0;
            _nReproved = 0;
            InitializeComponent();
        }

        private void btnImport_Click(object sender, EventArgs e)
        {
            try
            {
                OpenFileDialog open = new OpenFileDialog();
                if (open.ShowDialog() == DialogResult.OK)
                {
                    list.Items.AddRange(System.IO.File.ReadAllLines(open.FileName));
                    list.SelectedIndex = 0;
                    lblList.Text = list.Items.Count.ToString();
                    lblStatus.Text = "Loaded";
                }

               
               
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }


        }

        private void btnStart_Click(object sender, EventArgs e)
        {
          
            Starts.Start();
            
        }

        private void Starts_Tick(object sender, EventArgs e)
        {
            try
            {
                if (list.Items.Count >= 1)
                {
                    lblStatus.Text = "Working";
                    list.SelectedIndex = 0;
                    flag();
                    fillData();
                    Starts.Stop();
                    Verify.Start();
                }

                else
                {
                    Starts.Stop();
                    Returns.Start();
                    lblStatus.Text = "Stopped";
                    MessageBox.Show("Sem dados em lista", "Acabou");
                }
            }

            catch (Exception ex)
            {
                Starts.Stop();
                Returns.Start();
                MessageBox.Show(ex.Message);
            }


        }

        private void fillData()
        {
            string item = list.SelectedItem.ToString();
            string[] split = item.Split(Convert.ToChar(txtSplit.Text));
            lblCard.Text = split[0];
            web.Document.GetElementById("firstname").SetAttribute("value", name());
            web.Document.GetElementById("lastname").SetAttribute("value", lastName());
            web.Document.GetElementById("addr1").SetAttribute("value", alphanumeric(8) + " " + number(2));
            web.Document.GetElementById("city").SetAttribute("value", "New York");
            web.Document.GetElementById("state_cd").SetAttribute("value", "NY");
            web.Document.GetElementById("zip").SetAttribute("value", "10001");
            web.Document.GetElementById("email").SetAttribute("value", alphanumeric(11) + email());
            web.Document.GetElementById("phone").SetAttribute("value", "555702" + number(4));
            web.Document.GetElementById("cc_number").SetAttribute("value", split[0]);
            web.Document.GetElementById("cc_expir_month").SetAttribute("value", split[1]);
            web.Document.GetElementById("cc_expir_year").SetAttribute("value", split[2]);
            web.Document.GetElementById("amt_other_text").SetAttribute("value", "1");
            web.Document.GetElementById("amt_other").InvokeMember("click");
            web.Document.GetElementById("processbutton").InvokeMember("click");

        }

        private string alphanumeric(int number)
        {
            var chars = "abcdefghijklmnopqrstuaaaaaaaerreesaasegvftynnpoiuytresqaxdrtghhjvwxyz0123456789";
            var stringChars = new char[number];
            var random = new Random();

            for (int i = 0; i < stringChars.Length; i++)
            {
                stringChars[i] = chars[random.Next(chars.Length)];
            }

            var finalString = new String(stringChars);

            return finalString.ToString();
        }

        private string number(int numbers)
        {
            var chars = "0123456789";
            var stringChars = new char[numbers];
            var random = new Random();

            for (int i = 0; i < stringChars.Length; i++)
            {
                stringChars[i] = chars[random.Next(chars.Length)];
            }

            var finalString = new String(stringChars);

            return finalString.ToString();
        }

        private string name()
        {
            Random rnd = new Random();
            string[] names = { "Paulo", "Joao", "Eduardo", "Rafael", "Junior", "Pedro" ,"Adriana","Carlos","Daniel","Silvo"};
            return names[rnd.Next(names.Length)];
        }

        private string lastName()
        {
            Random rnd = new Random();
            string[] lastNames = { "Silva", "Lemes", "Santos", "Sousa", "Moura", "Francisco" };
            return lastNames[rnd.Next(lastNames.Length)];
        }

        private string email()
        {
            Random rnd = new Random();
            string[] emails = { "@gmail.com", "@hotmail.com", "@live.com", "@mail.ru","@bol.com","@yahoo.com" };
            return emails[rnd.Next(emails.Length)];
        }

        private void flag()
        {
            string lista = list.SelectedItem.ToString();
            string[] split = lista.Split(Convert.ToChar(txtSplit.Text));
            
            if (split[0].StartsWith("4"))
            {
                web.Document.GetElementById("cc_vs").InvokeMember("click");
                lblFlag.ForeColor = Color.Blue;
                lblFlag.Text = "Visa";
            }

            else if (split[0].StartsWith("5"))
            {
                web.Document.GetElementById("cc_mc").InvokeMember("click");
                lblFlag.ForeColor = Color.Orange;
                lblFlag.Text = "Mastercard";
            }

            else if (split[0].StartsWith("37"))
            {
                web.Document.GetElementById("cc_ax").InvokeMember("click");
                lblFlag.ForeColor = Color.Cyan;
                lblFlag.Text = "Amex";
            }

            else
            {
                web.Document.GetElementById("cc_ds").InvokeMember("click");
                lblFlag.ForeColor = Color.Green;
                lblFlag.Text = "Discovery";
            }
        }

        private void Verify_Tick(object sender, EventArgs e)
        {
            try
            {
                if (web.Document.Body.InnerText.Contains("Thanks"))
                {
                    SystemSounds.Asterisk.Play();
                    
                    approved.Items.Add(list.SelectedItem + "|Live|By Kamikaze|1$|");
                    _nApproved++;
                    lblLive.Text = _nApproved.ToString();
                    list.Items.RemoveAt(0);
                    lblList.Text = list.Items.Count.ToString();
                    Verify.Stop();
                    Returns.Start();
                    Starts.Start();
                }

                else if (web.Document.Body.InnerText.Contains("Error Processing Contribution"))
                {
                    _nReproved++;
                    lblDie.Text = _nReproved.ToString();
                    list.Items.RemoveAt(0);
                    lblList.Text = list.Items.Count.ToString();
                    Verify.Stop();
                    Returns.Start();
                    Starts.Start();
                }

                else
                {
                    _nReproved++;
                    lblDie.Text = _nReproved.ToString();
                    list.Items.RemoveAt(0);
                    lblList.Text = list.Items.Count.ToString();
                    Verify.Stop();
                    Returns.Start();
                    Starts.Start();
                }


            }

            catch (Exception ex)
            {
                Starts.Stop();
                Verify.Stop();
                Returns.Start();
                MessageBox.Show(ex.Message);
            }
        }

       

        private void Returns_Tick(object sender, EventArgs e)
        {
            try
            {
                web.Navigate("https://site.com/page/contribute/donate");
                Returns.Stop();
            }
            catch (Exception ex)
            {
                Returns.Stop();
                MessageBox.Show(ex.Message);
            }
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            Starts.Stop();
            Verify.Stop();
            Returns.Start();
            lblStatus.Text = "Stopped";
            MessageBox.Show("successfully stopped!", "stopped");
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFile = new SaveFileDialog();
            saveFile.Filter = "Text File(*.txt)|*.txt";

            if (saveFile.ShowDialog() == DialogResult.OK)
            {
                StreamWriter stWr = new StreamWriter(saveFile.FileName);
                foreach (string linha in approved.Items)
                {
                    stWr.Write(linha + "\r\n");
                }
                stWr.Close();
                MessageBox.Show("Saved", "Successful");
            }
        }

        

        private void btnClear_Click(object sender, EventArgs e)
        {
            lblStatus.Text = "Stopped";
            lblCard.Text = "0000000000000000";
            lblFlag.ForeColor = Color.DimGray;
            lblFlag.Text = "None";
            _nApproved = 0;
            _nReproved = 0;
            list.Items.Clear();
            approved.Items.Clear();
            lblDie.Text = "0";
            lblList.Text = "0";
            lblLive.Text = "0";
            Process.Start("rundll32.exe", "InetCpl.cpl,ClearMyTracksByProcess 1");
            Process.Start("rundll32.exe", "InetCpl.cpl,ClearMyTracksByProcess 2");
            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (button1.Text == "Show Browser")
            {
                this.Width = 774;
                this.Height = 437;
                button1.Text = "Hide Browser";
            }

            else if (button1.Text == "Hide Browser")
            {
                this.Width = 480;
                this.Height = 437;
                button1.Text = "Show Browser";

            }
        }

      

       
       
        

       

       

     

            
       

       

       

        

        

       
    }
}
