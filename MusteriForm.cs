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
    public partial class MusteriForm : Form
    {
        public MusteriForm()
        {
            InitializeComponent();
        }

        SqlBaglantisi baglan = new SqlBaglantisi();
        int secilen_id = 1;
        private void MusteriForm_Load(object sender, EventArgs e)
        {
            Listele();
        }

        void Listele()
        {
            SqlConnection bgl = baglan.Baglanti();
            SqlDataAdapter da = new SqlDataAdapter("SELECT * FROM Musteriler", bgl);
            DataTable dt = new DataTable();
            da.Fill(dt);
            dataGridView1.DataSource = dt;
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dataGridView1.Rows[e.RowIndex];
                secilen_id = Convert.ToInt32(row.Cells["MusteriID"].Value);
                textBox1.Text = row.Cells["AdSoyad"].Value.ToString();
                textBox2.Text = row.Cells["Telefon"].Value.ToString();
                textBox3.Text = row.Cells["Eposta"].Value.ToString();
                textBox4.Text = row.Cells["Adres"].Value.ToString();
                textBox5.Text = row.Cells["VergiNo"].Value.ToString();
                textBox6.Text = row.Cells["FirmaAdi"].Value.ToString();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            SqlConnection bgl = baglan.Baglanti();
            SqlCommand cmd = new SqlCommand("INSERT INTO Musteriler (AdSoyad, Telefon, Eposta, Adres, VergiNo, FirmaAdi) VALUES (@ad, @tel, @mail, @adres, @vergi, @firma)", bgl);
            cmd.Parameters.AddWithValue("@ad", textBox1.Text);
            cmd.Parameters.AddWithValue("@tel", textBox2.Text);
            cmd.Parameters.AddWithValue("@mail", textBox3.Text);
            cmd.Parameters.AddWithValue("@adres", textBox4.Text);
            cmd.Parameters.AddWithValue("@vergi", textBox5.Text);
            cmd.Parameters.AddWithValue("@firma", textBox6.Text);
            cmd.ExecuteNonQuery();
            Listele();
            MessageBox.Show("Müşteri eklendi.");
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (secilen_id != -1)
            {
                SqlConnection bgl = baglan.Baglanti();
                SqlCommand cmd = new SqlCommand("UPDATE Musteriler SET AdSoyad=@ad, Telefon=@tel, Eposta=@mail, Adres=@adres, VergiNo=@vergi, FirmaAdi=@firma WHERE MusteriID=@id", bgl);
                cmd.Parameters.AddWithValue("@ad", textBox1.Text);
                cmd.Parameters.AddWithValue("@tel", textBox2.Text.ToString());
                cmd.Parameters.AddWithValue("@mail", textBox3.Text);
                cmd.Parameters.AddWithValue("@adres", textBox4.Text);
                cmd.Parameters.AddWithValue("@vergi", textBox5.Text);
                cmd.Parameters.AddWithValue("@firma", textBox6.Text);
                cmd.Parameters.AddWithValue("@id", secilen_id);
                cmd.ExecuteNonQuery();
                Listele();
                MessageBox.Show("Müşteri güncellendi.");
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (secilen_id != -1)
            {
                SqlConnection bgl = baglan.Baglanti();
                SqlCommand cmd = new SqlCommand("DELETE FROM Musteriler WHERE MusteriID=@id", bgl);
                cmd.Parameters.AddWithValue("@id", secilen_id);
                cmd.ExecuteNonQuery();

                SqlCommand kontrolCmd = new SqlCommand("SELECT COUNT(*) FROM Musteriler", bgl);
                int kalanKayit = (int)kontrolCmd.ExecuteScalar();
                if (kalanKayit == 0)
                {
                    SqlCommand reseedCmd = new SqlCommand("DBCC CHECKIDENT ('Musteriler', RESEED, 0)", bgl);
                    reseedCmd.ExecuteNonQuery();
                }

                Listele();
                MessageBox.Show("Müşteri silindi.");
            }
        }
        private void button5_Click(object sender, EventArgs e)
        {
            AnaMenu frm = new AnaMenu();
            frm.Show();
            this.Hide();
        }
    }
}
