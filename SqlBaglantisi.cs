using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proje
{
    internal class SqlBaglantisi
    {

        string baglanti_cmd = @"Data Source=LAPTOP-4UP8KD5I\SQLEXPRESS;Initial Catalog=InsaatOtomasyonDB;Integrated Security=True;";

        public SqlConnection Baglanti()
        {
            SqlConnection baglan = new SqlConnection(baglanti_cmd);
            baglan.Open();
            return baglan;
        }
    }
}
