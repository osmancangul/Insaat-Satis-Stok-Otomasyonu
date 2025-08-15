using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Proje
{
    public partial class SatisDetaylariForm : Form
    {
        public SatisDetaylariForm()
        {
            InitializeComponent();
        }

        SqlBaglantisi baglan = new SqlBaglantisi();
        int satisID;

        public SatisDetaylariForm(int _satisID)
        {
            InitializeComponent();
            satisID = _satisID;
        }

        private void SatisDetaylari_Load(object sender, EventArgs e)
        {
            MalzemeListele();
            TedarikcileriListele();
            Listele();
            textBox3.ReadOnly = true;
        }

        void MalzemeListele()
        {
            SqlConnection bgl = baglan.Baglanti();
            SqlCommand cmd = new SqlCommand("Select * From Malzemeler", bgl);
            SqlDataReader dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                comboBox1.Items.Add(new ComboBoxItem
                {
                    Text = dr["MalzemeAdi"].ToString(),
                    Value = dr["MalzemeID"]
                });
            }
        }

        void TedarikcileriListele()
        {
            SqlConnection bgl = baglan.Baglanti();
            SqlCommand cmd = new SqlCommand("Select * From Tedarikciler", bgl);
            SqlDataReader dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                comboBox2.Items.Add(new ComboBoxItem
                {
                    Text = dr["YetkiliAdi"].ToString(),
                    Value = dr["TedarikciID"]
                });
            }
        }

        void Listele()
        {
            SqlConnection bgl = baglan.Baglanti();
            SqlDataAdapter da = new SqlDataAdapter("SELECT * From SatisDetaylari ",bgl);
            da.SelectCommand.Parameters.AddWithValue("@satisID", satisID);

            DataTable dt = new DataTable();
            da.Fill(dt);
            dataGridView1.DataSource = dt;
            ToplamTutarGuncelle();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox1.SelectedItem != null)
            {
                ComboBoxItem secili = (ComboBoxItem)comboBox1.SelectedItem;
                SqlConnection bgl = baglan.Baglanti();
                SqlCommand cmd = new SqlCommand("SELECT BirimFiyat FROM Malzemeler WHERE MalzemeID=@id", bgl);
                cmd.Parameters.AddWithValue("@id", secili.Value);
                SqlDataReader dr = cmd.ExecuteReader();
                if (dr.Read())
                {
                    textBox2.Text = dr["BirimFiyat"].ToString();
                }

                dr.Close();

                if (decimal.TryParse(textBox1.Text, out decimal miktar) && decimal.TryParse(textBox2.Text, out decimal fiyat))
                {
                    textBox3.Text = (miktar * fiyat).ToString("0.00");
                }

            }
        }



        void ToplamTutarGuncelle()
        {
            SqlConnection bgl = baglan.Baglanti();
            SqlCommand cmd = new SqlCommand("SELECT SUM(Tutar) FROM SatisDetaylari WHERE SatisID = @satisID", bgl);
            cmd.Parameters.AddWithValue("@satisID", satisID);
            object sonuc = cmd.ExecuteScalar();
            decimal toplam = sonuc != DBNull.Value ? Convert.ToDecimal(sonuc) : 0;

            SqlCommand guncelle = new SqlCommand("UPDATE Satislar SET ToplamTutar = @toplam WHERE SatisID = @id", bgl);
            guncelle.Parameters.AddWithValue("@toplam", toplam);
            guncelle.Parameters.AddWithValue("@id", satisID);
            guncelle.ExecuteNonQuery();
        }

        public class ComboBoxItem
        {
            public string Text { get; set; }
            public object Value { get; set; }

            public override string ToString() => Text;
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            try
            {
                decimal miktar = Convert.ToDecimal(textBox1.Text);
                decimal fiyat = Convert.ToDecimal(textBox2.Text);
                textBox3.Text = (miktar * fiyat).ToString("0.00");
            }
            catch
            {
                textBox3.Text = "0";
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (comboBox1.SelectedItem == null || textBox3.Text == "")
            {
                MessageBox.Show("Lütfen tüm alanları doldurun.");
                return;
            }

            ComboBoxItem secili = (ComboBoxItem)comboBox1.SelectedItem;

            SqlConnection bgl = baglan.Baglanti();
            SqlTransaction trans = bgl.BeginTransaction();

            try
            {

                decimal miktar = Convert.ToDecimal(textBox1.Text);
                decimal fiyat = Convert.ToDecimal(textBox2.Text);
                decimal tutar = miktar * fiyat;


                SqlCommand cmd = new SqlCommand("INSERT INTO SatisDetaylari (SatisID, MalzemeID, Miktar, BirimFiyat,TedarikciAdi,AlisTarihi) " + "VALUES (@satisID, @malzemeID, @miktar, @fiyat, @tedarikci, @alis)", bgl,trans);
                cmd.Parameters.AddWithValue("@satisID", satisID);
                cmd.Parameters.AddWithValue("@malzemeID", secili.Value);
                cmd.Parameters.AddWithValue("@miktar", Convert.ToDecimal(textBox1.Text));
                cmd.Parameters.AddWithValue("@fiyat", Convert.ToDecimal(textBox2.Text));
                cmd.Parameters.AddWithValue("@tedarikci", comboBox2.Text);
                cmd.Parameters.AddWithValue("@alis", dateTimePicker1.Value.Date);
                cmd.ExecuteNonQuery();


                /*SqlCommand stokAzalt = new SqlCommand(@"UPDATE Malzemeler SET StokMiktari = StokMiktari - @miktar WHERE MalzemeID = @id", bgl, trans);
                stokAzalt.Parameters.AddWithValue("@miktar", miktar);
                stokAzalt.Parameters.AddWithValue("@id", secili.Value);
                stokAzalt.ExecuteNonQuery();*/

                trans.Commit();

                Listele();
                MessageBox.Show("Satış detayı eklendi ve stok güncellendi.");
            }
            catch (Exception ex)
            {

                trans.Rollback();
                MessageBox.Show("Hata: " + ex.Message);
                throw;
            }


        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                int detayID = Convert.ToInt32(dataGridView1.SelectedRows[0].Cells["DetayID"].Value);

                SqlConnection bgl = baglan.Baglanti();
                SqlTransaction trans = bgl.BeginTransaction();

                try
                {
                    SqlCommand getDetay = new SqlCommand("SELECT MalzemeID, Miktar FROM SatisDetaylari WHERE DetayID = @id", bgl, trans);
                    getDetay.Parameters.AddWithValue("@id", detayID);
                    SqlDataReader dr = getDetay.ExecuteReader();
                    int malzemeID = 0;
                    decimal miktar = 0;
                    if (dr.Read())
                    {
                        malzemeID = Convert.ToInt32(dr["MalzemeID"]);
                        miktar = Convert.ToDecimal(dr["Miktar"]);
                    }
                    dr.Close();

                    SqlCommand cmd = new SqlCommand("DELETE FROM SatisDetaylari WHERE DetayID = @id", bgl, trans);
                    cmd.Parameters.AddWithValue("@id", detayID);
                    cmd.ExecuteNonQuery();

                    SqlCommand stokArtir = new SqlCommand("UPDATE Malzemeler SET StokMiktari = StokMiktari + @miktar WHERE MalzemeID = @id", bgl, trans);
                    stokArtir.Parameters.AddWithValue("@miktar", miktar);
                    stokArtir.Parameters.AddWithValue("@id", malzemeID);
                    stokArtir.ExecuteNonQuery();


                    SqlCommand kontrolKalanCmd = new SqlCommand("SELECT COUNT(*) FROM SatisDetaylari", bgl,trans);
                    int kalanKayit = (int)kontrolKalanCmd.ExecuteScalar();

                    if (kalanKayit == 0)
                    {
                        SqlCommand reseedCmd = new SqlCommand("DBCC CHECKIDENT ('SatisDetaylari', RESEED, 0)", bgl,trans);
                        reseedCmd.ExecuteNonQuery();
                    }

                    trans.Commit();
                    Listele();
                    MessageBox.Show("Detay silindi ve stok geri eklendi.");
                }
                catch (Exception ex)
                {
                    trans.Rollback();
                    MessageBox.Show("Hata: " + ex.Message);
                }
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Listele();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            AnaMenu frm = new AnaMenu();
            frm.Show();
            this.Hide();
        }
    }
}
