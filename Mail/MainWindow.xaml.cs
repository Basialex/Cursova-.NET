using Chilkat;
using Limilabs.Client.IMAP;
using Limilabs.Mail;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Net.Mail;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using MessageBox = System.Windows.MessageBox;

namespace Mail
{

    public partial class MainWindow : System.Windows.Window
    {
        private Limilabs.Client.IMAP.Imap imap;
        private IMail imail;
        private DataTable table;
        private long idmail;
        bool textbox7_has_focus = false;



        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            imap = new Limilabs.Client.IMAP.Imap();
            string m = textbox1.Text;
            int l = m.Length;
            int ll = passwordbox.Password.Length;
            if (l > 8)
            {
                if (m.Substring(m.Length - 9) == "gmail.com")
                {

                    imap.ConnectSSL("imap.gmail.com", 993);


                    try
                    {

                        table = new DataTable();
                        table.Columns.Add("IDMail", typeof(string));
                        table.Columns.Add("Subject", typeof(string));
                        table.Columns.Add("Date", typeof(string));
                        table.Columns.Add("From", typeof(string));

                        imap.Login(textbox1.Text, passwordbox.Password);
                        MessageBox.Show("Post open");
                    }
                    catch (Exception s)
                    {
                        MessageBox.Show("Post dont open, try again\n" + s.Message);
                    }
                }

                if (m.Substring(m.Length - 7) == "mail.ru")
                {
                    imap.ConnectSSL("imap.mail.ru", 993);
                    try
                    {
                        table = new DataTable();
                        table.Columns.Add("IDMail", typeof(string));
                        table.Columns.Add("Subject", typeof(string));
                        table.Columns.Add("Date", typeof(string));
                        table.Columns.Add("From", typeof(string));
                        imap.Login(textbox1.Text, passwordbox.Password);
                        MessageBox.Show("Post open");
                    }
                    catch
                    {
                        MessageBox.Show("Post dont open, try again");
                    }
                }
                if (m.Substring(m.Length - 9) == "yandex.ru")
                {
                    imap.ConnectSSL("imap.yandex.ru", 993);
                    try
                    {
                        table = new DataTable();
                        table.Columns.Add("IDMail", typeof(string));
                        table.Columns.Add("Subject", typeof(string));
                        table.Columns.Add("Date", typeof(string));
                        table.Columns.Add("From", typeof(string));
                        imap.Login(textbox1.Text, passwordbox.Password);
                        MessageBox.Show("Post open");
                    }
                    catch
                    {
                        MessageBox.Show("Post dont open, try again");
                    }
                }
            }
            else
            {
                MessageBox.Show("Incorrect login or password");
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            listbox.DataContext = null;

            imap.SelectInbox();
            List<long> vids = imap.SearchFlag(Flag.All);
            int i = vids.Count;
            int j = 0;
            foreach (long vid in vids)
            {
                if (j < i)
                {
                    try
                    {
                        byte[] eml = imap.GetMessageByUID(vid);
                        imail = new MailBuilder().CreateFromEml(eml);
                        TimeSpan t = dp_1.SelectedDate.Value - imail.Date.Value;
                        TimeSpan t1 = dp_2.SelectedDate.Value - imail.Date.Value;
                        if (t.Days <= 0 && t1.Days >= 0)
                        {
                            DataRow row = table.NewRow();
                            row["IDMAil"] = vid.ToString();
                            row["Subject"] = imail.Subject;
                            row["Date"] = imail.Date.Value.ToString("dd/MM/yyyy");
                            row["From"] = imail.From.ToString();
                            table.Rows.Add(row);
                            table.AcceptChanges();
                        }
                        else
                        {
                            i--;
                            j++;
                        }
                    }
                    catch
                    {
                        j++;
                    }
                }
                else break;
            }

            listbox.DataContext = table;
        }
        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            try
            {
                imap.DeleteMessageByUID(idmail);
                foreach (DataRow row in table.Rows)
                {
                    if (idmail == long.Parse(row["IDMail"].ToString()))
                    {
                        table.Rows.Remove(row);
                        table.AcceptChanges();
                        break;
                    }
                }
                table.Rows.Clear();
                listbox.DataContext = table;
                MessageBox.Show("Deleted");
            }
            catch
            {
                MessageBox.Show("Do not deleted");
            }
        }


        private void listbox_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {

            string id = listbox.SelectedItems.Count.ToString();
            byte[] eml = imap.GetMessageByUID(long.Parse(id));
            imail = new MailBuilder().CreateFromEml(eml);
            textbox3.Text = imail.Text;
            idmail = long.Parse(id);

        }

        private void button5_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            try
            {
                if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    textbox5.Text = File.ReadAllText(openFileDialog.FileName);
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void Button_Click_4(object sender, RoutedEventArgs e)
        {
            int lengthg1 = textbox1.Text.Length;
            int lengthg2 = textbox4.Text.Length;
            string m = textbox4.Text;

            if (lengthg1 > 9)
            {
                if (m.Substring(m.Length - 7) == "mail.ru")
                {
                    try
                    {
                        SmtpClient client = new SmtpClient("smpt.mail.ru", 587);
                        MailMessage message = new MailMessage();
                        message.From = new MailAddress(textbox1.Text);
                        message.To.Add(textbox4.Text);
                        message.Subject = textbox6.Text;
                        message.Body = textbox7.Text;
                        client.Timeout = 10000;
                        // client.UseDefaultCredentials = false;
                        client.EnableSsl = false;////true
                        if (textbox5.Text != "")
                        {
                            message.Attachments.Add(new Attachment(textbox5.Text));
                        }
                        client.Credentials = new System.Net.NetworkCredential(textbox1.Text, passwordbox.Password);
                        client.Send(message);
                        message = null;
                        MessageBox.Show("Message send");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Message not sent because of:" + ex.ToString());
                    }
                }

                if (m.Substring(m.Length - 9) == "gmail.com")
                {
                    Thread threadSendMails;
                    threadSendMails = new Thread(delegate ()
                    {
                        try
                        {
                            SmtpClient client = new SmtpClient("smpt.gmail.com", 587);
                            MailMessage message = new MailMessage();
                            message.From = new MailAddress(textbox1.Text);
                            message.To.Add(textbox4.Text);
                            message.Subject = textbox6.Text;
                            message.Body = textbox7.Text;
                            client.Timeout = 10000;
                            // client.UseDefaultCredentials = false;
                            client.EnableSsl = true;  //false
                            if (textbox5.Text != "")
                            {
                                message.Attachments.Add(new Attachment(textbox5.Text));
                            }
                            client.Credentials = new System.Net.NetworkCredential(textbox1.Text, passwordbox.Password);
                            client.Send(message);
                            message = null;
                            MessageBox.Show("Message send");
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Message not sent because of:" + ex.ToString());
                        }
                    });
                    threadSendMails.IsBackground = true;
                    threadSendMails.Start();
                }
                if (m.Substring(m.Length - 9) == "yandex.ru")
                {
                    try
                    {
                        SmtpClient client = new SmtpClient("smpt.yandex.ru", 587);
                        MailMessage message = new MailMessage();
                        message.From = new MailAddress(textbox1.Text);
                        message.To.Add(textbox4.Text);
                        message.Subject = textbox6.Text;
                        message.Body = textbox7.Text;
                        client.Timeout = 10000;
                        // client.UseDefaultCredentials = false;
                        client.EnableSsl = false;////true
                        if (textbox5.Text != "")
                        {
                            message.Attachments.Add(new Attachment(textbox5.Text));
                        }
                        client.Credentials = new System.Net.NetworkCredential(textbox1.Text, passwordbox.Password);
                        client.Send(message);
                        message = null;
                        MessageBox.Show("Message send");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Message not sent because of:" + ex.ToString());
                    }
                }
            }
            else
            {
                MessageBox.Show("Incorrect login or password");
            }
        }

        private void MessageTB_GotFocus(object sender, RoutedEventArgs e)
        {
            if (!textbox7_has_focus)
            {
                textbox7.Text = string.Empty;
                textbox7_has_focus = true;
            }
        }
    }
}
