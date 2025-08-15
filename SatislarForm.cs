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
    public partial class SatislarForm : Form
    {
        public SatislarForm()
        {
            InitializeComponent();
        }

        SqlBaglantisi baglan = new SqlBaglantisi();

        private void SatislarForm_Load(object sender, EventArgs e)
        {
            MusterileriListele();
            comboBox2.Items.AddRange(new string[] { "Nakit", "Kredi Kartı", "EFT" });
            Listele();
            textBox1.ReadOnly = true;

        }

        public class ComboBoxItem
        {
            public string Text { get; set; }
            public object Value { get; set; }

            public override string ToString()
            {
                return Text;
            }
        }

        void MusterileriListele()
        {
            SqlConnection bgl = baglan.Baglanti();
            SqlCommand cmd = new SqlCommand("Select * From Musteriler", bgl);
            SqlDataReader dr = cmd.ExecuteReader();

            while(dr.Read())
            {
                comboBox1.Items.Add(new ComboBoxItem
                {
                    Text = dr["AdSoyad"].ToString(),
                    Value = dr["MusteriID"]
                });
            }
        }

        void Listele()
        {
            SqlConnection bgl = baglan.Baglanti();
            SqlDataAdapter da = new SqlDataAdapter("SELECT S.SatisID, S.Tarih, M.AdSoyad, S.OdemeTuru, S.ToplamTutar, S.Personel, S.Aciklama FROM Satislar S INNER JOIN Musteriler M ON S.MusteriID = M.MusteriID", bgl);
            DataTable dt = new DataTable();
            da.Fill(dt);
            dataGridView1.DataSource = dt;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (comboBox1.SelectedItem == null || comboBox2.SelectedItem == " ")
            {
                MessageBox.Show("Lütfen müşter ve ödeme türünü seçiniz.");
                return;
            }

            SqlConnection bgl = baglan.Baglanti();
            ComboBoxItem secili_musteri = (ComboBoxItem)comboBox1.SelectedItem;

            SqlCommand cmd = new SqlCommand("INSERT INTO Satislar (Tarih, MusteriID, OdemeTuru, ToplamTutar, Personel, Aciklama) OUTPUT INSERTED.SatisID VALUES (@tarih, @musteri, @odeme, @tutar, @personel, @aciklama)", bgl);
            cmd.Parameters.AddWithValue("@tarih", dateTimePicker1.Value);
            cmd.Parameters.AddWithValue("@musteri", secili_musteri.Value);
            cmd.Parameters.AddWithValue("@odeme", comboBox2.Text);
            cmd.Parameters.AddWithValue("@tutar", 0);
            cmd.Parameters.AddWithValue("@personel", textBox2.Text);
            cmd.Parameters.AddWithValue("@aciklama", textBox3.Text);

            int satisID = (int)cmd.ExecuteScalar();

            SatisDetaylariForm detayForm = new SatisDetaylariForm(satisID);
            detayForm.ShowDialog();

            Listele();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Listele();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            AnaMenu frm = new AnaMenu();
            frm.Show();
            this.Hide();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                DialogResult sonuc = MessageBox.Show("Seçili satışı silmek istediğinizden emin misiniz?", "Onay", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (sonuc == DialogResult.Yes)
                {
                    int satisID = Convert.ToInt32(dataGridView1.SelectedRows[0].Cells["SatisID"].Value);

                    SqlConnection bgl = baglan.Baglanti();
                    SqlCommand cmd = new SqlCommand("DELETE FROM SatisDetaylari WHERE SatisID = @id; DELETE FROM Satislar WHERE SatisID = @id", bgl);
                    cmd.Parameters.AddWithValue("@id", satisID);
                    cmd.ExecuteNonQuery();

                    SqlCommand kontrolKalanCmd = new SqlCommand("SELECT COUNT(*) FROM Satislar", bgl);
                    int kalanKayit = (int)kontrolKalanCmd.ExecuteScalar();

                    if (kalanKayit == 0)
                    {
                        SqlCommand reseedCmd = new SqlCommand("DBCC CHECKIDENT ('Satislar', RESEED, 0)", bgl);
                        reseedCmd.ExecuteNonQuery();
                    }

                    MessageBox.Show("Satış silindi.");
                    Listele();
                }
            }
            else
            {
                MessageBox.Show("Lütfen silmek için bir satış seçiniz.");
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            SatisDetaylariForm frm = new SatisDetaylariForm();
            frm.Show();
            this.Hide();
        }
    }
}
