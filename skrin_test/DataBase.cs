using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace skrin_test
{
    class DataBase
    {
        
        public static SqlDataReader RunQuery(string query)
        {


            SqlConnection sqlConnection = new SqlConnection(@"Data Source=DESKTOP-J47LC4G\SQLEXPRESS;Initial Catalog=skrin;Integrated Security=True");

            SqlCommand command = new SqlCommand(query, sqlConnection);
            command.CommandTimeout = 60;
            sqlConnection.Open();
            SqlDataReader reader = command.ExecuteReader();
            return reader;
               
            
        }
        public static void XMLsave (string query, List<Object> a)
        {
            SqlConnection sqlConnection = new SqlConnection(@"Data Source=DESKTOP-J47LC4G\SQLEXPRESS;Initial Catalog=skrin;Integrated Security=True");

            SqlCommand command = new SqlCommand(query, sqlConnection);
            command.CommandTimeout = 60;
            sqlConnection.Open();
            
            command.CommandText = query;
            for (int i = 0; i < a.Count(); i++) 
            {
                command.Parameters.AddWithValue("@param" + i, a[i]);
                
            }
            
            command.ExecuteNonQuery();

        }
    }
}
