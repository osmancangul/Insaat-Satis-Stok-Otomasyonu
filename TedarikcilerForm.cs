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
    public partial class TedarikcilerForm : Form
    {
        public TedarikcilerForm()
        {
            InitializeComponent();
        }

        SqlBaglantisi baglan = new SqlBaglantisi();
        int secilen_id = -1;

        private void Tedarikçiler_Load(object sender, EventArgs e)
        {
            Listele();
        }

        void Listele()
        {
            SqlConnection bgl = baglan.Baglanti();
            SqlDataAdapter da = new SqlDataAdapter("SELECT * FROM Tedarikciler", bgl);
            DataTable dt = new DataTable();
            da.Fill(dt);
            dataGridView1.DataSource = dt;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            SqlConnection bgl = baglan.Baglanti();
            SqlCommand cmd = new SqlCommand(@"INSERT INTO Tedarikciler (FirmaAdi, YetkiliAdi, Telefon, Eposta, Adres,VergiNo) VALUES (@firma, @yetkili, @tel, @mail, @adres, @vergi)", bgl);

            cmd.Parameters.AddWithValue("@firma", textBox1.Text);
            cmd.Parameters.AddWithValue("@yetkili", textBox2.Text);
            cmd.Parameters.AddWithValue("@tel", textBox3.Text);
            cmd.Parameters.AddWithValue("@mail", textBox4.Text);
            cmd.Parameters.AddWithValue("@adres", textBox5.Text);
            cmd.Parameters.AddWithValue("@vergi", textBox6.Text);

            cmd.ExecuteNonQuery();
            Listele();
            MessageBox.Show("Tedarikçi eklendi.");
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dataGridView1.Rows[e.RowIndex];
                secilen_id = Convert.ToInt32(row.Cells["TedarikciID"].Value);
                textBox1.Text = row.Cells["FirmaAdi"].Value.ToString();
                textBox2.Text = row.Cells["YetkiliAdi"].Value.ToString();
                textBox3.Text = row.Cells["Telefon"].Value.ToString();
                textBox4.Text = row.Cells["Eposta"].Value.ToString();
                textBox5.Text = row.Cells["Adres"].Value.ToString();
                textBox6.Text = row.Cells["VergiNo"].Value.ToString();
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (secilen_id != -1)
            {
                SqlConnection bgl = baglan.Baglanti();
                SqlCommand cmd = new SqlCommand(@"UPDATE Tedarikciler SET FirmaAdi=@firma, YetkiliAdi=@yetkili, Telefon=@tel, Eposta=@mail, Adres=@adres, VergiNo=@vergi WHERE TedarikciID=@id", bgl);

                cmd.Parameters.AddWithValue("@firma", textBox1.Text);
                cmd.Parameters.AddWithValue("@yetkili", textBox2.Text);
                cmd.Parameters.AddWithValue("@tel", textBox3.Text);
                cmd.Parameters.AddWithValue("@mail", textBox4.Text);
                cmd.Parameters.AddWithValue("@adres", textBox5.Text);
                cmd.Parameters.AddWithValue("@vergi", textBox6.Text);
                cmd.Parameters.AddWithValue("@id", secilen_id);

                cmd.ExecuteNonQuery();
                Listele();
                MessageBox.Show("Tedarikçi güncellendi.");
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            SqlConnection bgl = baglan.Baglanti();

            if (secilen_id != -1)
            {
                SqlCommand cmd = new SqlCommand("DELETE FROM Tedarikciler WHERE TedarikciID=@id", bgl);
                cmd.Parameters.AddWithValue("@id", secilen_id);
                cmd.ExecuteNonQuery();
                Listele();
                MessageBox.Show("Tedarikçi silindi.");
            }


            SqlCommand kontrolKalanCmd = new SqlCommand("SELECT COUNT(*) FROM Tedarikciler", bgl);
            int kalanKayit = (int)kontrolKalanCmd.ExecuteScalar();

            if (kalanKayit == 0)
            {
                SqlCommand reseedCmd = new SqlCommand("DBCC CHECKIDENT ('Tedarikciler', RESEED, 0)", bgl);
                reseedCmd.ExecuteNonQuery();
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            AnaMenu frm = new AnaMenu();
            frm.Show();
            this.Hide();
        }
    }
}
