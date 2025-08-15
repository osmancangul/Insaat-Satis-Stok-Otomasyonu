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
    public partial class StokHareketleriForm : Form
    {
        public StokHareketleriForm()
        {
            InitializeComponent();
        }

        SqlBaglantisi baglan = new SqlBaglantisi();

        public class ComboBoxItem
        {
            public string Text { get; set; }
            public object Value { get; set; }

            public ComboBoxItem(string text, object value)
            {
                Text = text;
                Value = value;
            }

            public override string ToString()
            {
                return Text;
            }
        }

        private void StokHareket_Load(object sender, EventArgs e)
        {
            SqlConnection bgl = baglan.Baglanti();
            SqlCommand cmd = new SqlCommand("SELECT MalzemeID, MalzemeAdi FROM Malzemeler", bgl);
            SqlDataReader dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                comboBox1.Items.Add(new ComboBoxItem(dr["MalzemeAdi"].ToString(), dr["MalzemeID"]));
            }
            dr.Close();

            comboBox2.Items.AddRange(new string[] { "Giriş", "Çıkış" });

            Listele();
        }

        void Listele()
        {
            SqlConnection bgl = baglan.Baglanti();
            SqlDataAdapter da = new SqlDataAdapter(@"
        SELECT SH.HareketID, M.MalzemeAdi, SH.Tarih, SH.HareketTuru, SH.Miktar, SH.Aciklama, SH.Personel, SH.ReferansNo
        FROM StokHareketleri SH
        JOIN Malzemeler M ON SH.MalzemeID = M.MalzemeID", bgl);
            DataTable dt = new DataTable();
            da.Fill(dt);
            dataGridView1.DataSource = dt;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (comboBox1.SelectedItem == null || comboBox2.SelectedItem == null || textBox1.Text == "")
            {
                MessageBox.Show("Lütfen tüm gerekli alanları doldurun.");
                return;
            }

            ComboBoxItem seciliMalzeme = (ComboBoxItem)comboBox1.SelectedItem;
            decimal miktar = Convert.ToDecimal(textBox1.Text);
            string hareketTuru = comboBox2.SelectedItem.ToString();

            SqlConnection bgl = baglan.Baglanti();
            SqlTransaction trans = bgl.BeginTransaction();

            try
            {
                SqlCommand cmd = new SqlCommand(@"
            INSERT INTO StokHareketleri (MalzemeID, Tarih, HareketTuru, Miktar, Aciklama,Personel,ReferansNo)
            VALUES (@malzemeID, @tarih, @tur, @miktar, @aciklama, @personel, @referans)", bgl, trans);
                cmd.Parameters.AddWithValue("@malzemeID", seciliMalzeme.Value);
                cmd.Parameters.AddWithValue("@tarih", dateTimePicker1.Value.Date);
                cmd.Parameters.AddWithValue("@tur", hareketTuru);
                cmd.Parameters.AddWithValue("@miktar", miktar);
                cmd.Parameters.AddWithValue("@aciklama", textBox2.Text);
                cmd.Parameters.AddWithValue("@personel", textBox3.Text);
                cmd.Parameters.AddWithValue("@referans", textBox4.Text);
                cmd.ExecuteNonQuery();

                string stokSQL = hareketTuru == "Giriş"
                    ? "UPDATE Malzemeler SET StokMiktari = StokMiktari + @miktar WHERE MalzemeID = @id"
                    : "UPDATE Malzemeler SET StokMiktari = StokMiktari - @miktar WHERE MalzemeID = @id";

                SqlCommand stokCmd = new SqlCommand(stokSQL, bgl, trans);
                stokCmd.Parameters.AddWithValue("@miktar", miktar);
                stokCmd.Parameters.AddWithValue("@id", seciliMalzeme.Value);
                stokCmd.ExecuteNonQuery();

                trans.Commit();
                Listele();
                MessageBox.Show("Stok hareketi eklendi ve stok güncellendi.");
            }
            catch (Exception ex)
            {
                trans.Rollback();
                MessageBox.Show("Hata: " + ex.Message);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (dataGridView1.CurrentRow != null)
            {
                int hareketID = Convert.ToInt32(dataGridView1.CurrentRow.Cells["HareketID"].Value);

                SqlConnection bgl = baglan.Baglanti();
                SqlTransaction trans = bgl.BeginTransaction();

                try
                {
                    SqlCommand getirCmd = new SqlCommand("SELECT MalzemeID, Miktar, HareketTuru FROM StokHareketleri WHERE HareketID=@id", bgl, trans);
                    getirCmd.Parameters.AddWithValue("@id", hareketID);
                    SqlDataReader dr = getirCmd.ExecuteReader();

                    if (dr.Read())
                    {
                        int malzemeID = Convert.ToInt32(dr["MalzemeID"]);
                        decimal miktar = Convert.ToDecimal(dr["Miktar"]);
                        string hareketTuru = dr["HareketTuru"].ToString();
                        dr.Close();

                        string stokSQL = (hareketTuru == "Giriş")
                            ? "UPDATE Malzemeler SET StokMiktari = StokMiktari - @miktar WHERE MalzemeID = @id"
                            : "UPDATE Malzemeler SET StokMiktari = StokMiktari + @miktar WHERE MalzemeID = @id";

                        SqlCommand stokCmd = new SqlCommand(stokSQL, bgl, trans);
                        stokCmd.Parameters.AddWithValue("@miktar", miktar);
                        stokCmd.Parameters.AddWithValue("@id", malzemeID);
                        stokCmd.ExecuteNonQuery();

                        SqlCommand silCmd = new SqlCommand("DELETE FROM StokHareketleri WHERE HareketID=@id", bgl, trans);
                        silCmd.Parameters.AddWithValue("@id", hareketID);
                        silCmd.ExecuteNonQuery();


                        SqlCommand kontrolKalanCmd = new SqlCommand("SELECT COUNT(*) FROM StokHareketleri", bgl,trans);
                        int kalanKayit = (int)kontrolKalanCmd.ExecuteScalar();

                        if (kalanKayit == 0)
                        {
                            SqlCommand reseedCmd = new SqlCommand("DBCC CHECKIDENT ('StokHareketleri', RESEED, 0)", bgl,trans);
                            reseedCmd.ExecuteNonQuery();
                        }

                        trans.Commit();
                        Listele();
                        MessageBox.Show("Stok hareketi silindi ve stok düzeltildi.");
                    }
                    else
                    {
                        dr.Close();
                        trans.Rollback();
                        MessageBox.Show("Kayıt bulunamadı.");
                    }
                }
                catch (Exception ex)
                {
                    trans.Rollback();
                    MessageBox.Show("Hata: " + ex.Message);
                }
            }
            else
            {
                MessageBox.Show("Lütfen silinecek bir kayıt seçin.");
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            AnaMenu frm = new AnaMenu();
            frm.Show();
            this.Hide();
        }
    }
}
