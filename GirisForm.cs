using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Proje
{
    public partial class GirisForm : Form
    {
        public GirisForm()
        {
            InitializeComponent();
        }

        SqlBaglantisi baglan = new SqlBaglantisi();


        private void button1_Click(object sender, EventArgs e)
        {
            SqlConnection bgl = baglan.Baglanti();

            string kullanici_adi = textBox1.Text;
            string sifre = textBox2.Text;

            string sql = "Select COUNT(*) From Kullanicilar Where KullaniciAdi = @KullaniciAdi AND Sifre = @Sifre";
            SqlCommand cmd = new SqlCommand(sql, bgl);
            cmd.Parameters.AddWithValue("KullaniciAdi", kullanici_adi);
            cmd.Parameters.AddWithValue("Sifre", sifre);

            try
            {
                if (bgl.State == ConnectionState.Closed)
                    bgl.Open();

                object result = cmd.ExecuteScalar();

                int sonuc = 0;
                if (result != null && int.TryParse(result.ToString(), out sonuc))
                {
                    if (sonuc > 0)
                    {
                        MessageBox.Show("Giriş Başarılı");
                        this.Hide();
                        AnaMenu frm = new AnaMenu();
                        frm.Show();
                    }
                    else
                    {
                        MessageBox.Show("Kullanıcı Adı Veya Şifre Hatalı!");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hata oluştu: " + ex.Message);
            }
            finally
            {
                if (bgl.State == ConnectionState.Open)
                    bgl.Close();
            }
        }
    }
}
