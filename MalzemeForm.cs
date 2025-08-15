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
    public partial class MalzemeForm : Form
    {
        public MalzemeForm()
        {
            InitializeComponent();
        }

        SqlBaglantisi baglan = new SqlBaglantisi();
        int secilen_id = 1;

        private void MalzemeForm_Load(object sender, EventArgs e)
        {

            comboBox1.Items.AddRange(new String[] { "Adet", "Kg", "Torba", "Metre" });
            this.dataGridView1.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView1_CellClick);

            comboBox2.Items.AddRange(new String[]{"İnşaat Malzemeleri","Elektrik Malzemeleri","Sıhhi Tesisat","Boyalar ve Kaplamalar","İzolasyon Malzemeleri","Mobilya ve Aksesuarlar","Zemin Kaplama","Diğer"});
            this.dataGridView1.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView1_CellClick);

            Listele();

        }

        void Listele()
        {
            SqlConnection bgl = baglan.Baglanti();
            SqlDataAdapter da = new SqlDataAdapter("Select * From Malzemeler", bgl);
            DataTable dt = new DataTable();
            da.Fill(dt);
            dataGridView1.DataSource = dt;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            SqlConnection bgl = baglan.Baglanti();
            SqlCommand cmd = new SqlCommand("INSERT INTO Malzemeler (MalzemeAdi, Birim, BirimFiyat, StokMiktari, Aciklama,Kategori) VALUES (@adi, @birim, @fiyat, @stok, @aciklama, @kategori)", bgl);
            cmd.Parameters.AddWithValue("@adi", textBox1.Text);
            cmd.Parameters.AddWithValue("@birim", comboBox1.Text);
            cmd.Parameters.AddWithValue("@fiyat", Convert.ToDecimal(textBox2.Text));
            cmd.Parameters.AddWithValue("@stok", Convert.ToDecimal(textBox3.Text));
            cmd.Parameters.AddWithValue("@aciklama", textBox4.Text);
            cmd.Parameters.AddWithValue("@kategori", comboBox2.Text);
            cmd.ExecuteNonQuery();
            Listele();
            MessageBox.Show("Malzeme Başarıyla Eklendi");
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (secilen_id != -1)
            {
                SqlConnection bgl = baglan.Baglanti();
                SqlCommand cmd = new SqlCommand("UPDATE Malzemeler SET MalzemeAdi=@adi, Birim=@birim, BirimFiyat=@fiyat, StokMiktari=@stok, Aciklama=@aciklama, Kategori=@kategori WHERE MalzemeID=@id", bgl);
                cmd.Parameters.AddWithValue("@adi", textBox1.Text);
                cmd.Parameters.AddWithValue("@birim", comboBox1.Text);
                cmd.Parameters.AddWithValue("@fiyat", Convert.ToDecimal(textBox2.Text));
                cmd.Parameters.AddWithValue("@stok", Convert.ToDecimal(textBox3.Text));
                cmd.Parameters.AddWithValue("@aciklama", textBox4.Text);
                cmd.Parameters.AddWithValue("@kategori", comboBox2.Text);
                cmd.Parameters.AddWithValue("@id", secilen_id);
                cmd.ExecuteNonQuery();
                Listele();
                MessageBox.Show("Malzeme güncellendi.");
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {

            if (dataGridView1.SelectedRows.Count == 0)
            {
                MessageBox.Show("Lütfen silmek için bir malzeme seçin.");
                return;
            }

            int malzeme_id = Convert.ToInt32(dataGridView1.SelectedRows[0].Cells["MalzemeID"].Value);

            SqlConnection bgl = baglan.Baglanti();
            SqlCommand kontrolCmd = new SqlCommand("SELECT COUNT(*) FROM SatisDetaylari WHERE MalzemeID = @id", bgl);
            kontrolCmd.Parameters.AddWithValue("@id", malzeme_id);
            int kullanilmaSayisi = (int)kontrolCmd.ExecuteScalar();

            if (kullanilmaSayisi > 0)
            {
                MessageBox.Show("Bu malzeme satışlarda kullanıldığı için silinemez.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DialogResult onay = MessageBox.Show("Bu malzemeyi silmek istediğinize emin misiniz?", "Onay", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (onay == DialogResult.No)
            {
                return;
            }

            SqlCommand cmd = new SqlCommand("DELETE FROM Malzemeler WHERE MalzemeID=@id", bgl);
            cmd.Parameters.AddWithValue("@id", malzeme_id);
            cmd.ExecuteNonQuery();

            SqlCommand kontrolKalanCmd = new SqlCommand("SELECT COUNT(*) FROM Malzemeler", bgl);
            int kalanKayit = (int)kontrolKalanCmd.ExecuteScalar();

            if (kalanKayit == 0)
            {
                SqlCommand reseedCmd = new SqlCommand("DBCC CHECKIDENT ('Malzemeler', RESEED, 0)", bgl);
                reseedCmd.ExecuteNonQuery();
            }

            Listele();
            MessageBox.Show("Malzeme başarıyla silindi.");
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Listele();
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dataGridView1.Rows[e.RowIndex];

                secilen_id = Convert.ToInt32(row.Cells["MalzemeID"].Value);

                textBox1.Text = row.Cells["MalzemeAdi"].Value.ToString();
                comboBox1.Text = row.Cells["Birim"].Value.ToString();
                textBox2.Text = row.Cells["BirimFiyat"].Value.ToString();
                textBox3.Text = row.Cells["StokMiktari"].Value.ToString();
                textBox4.Text = row.Cells["Aciklama"].Value.ToString();
                comboBox2.Text = row.Cells["Kategori"].Value.ToString();
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
