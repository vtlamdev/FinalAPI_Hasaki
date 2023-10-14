using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Web;
using FinalAPI_Hasaki.Controllers;
namespace FinalAPI_Hasaki.Database
{
    public class Database
    {
        public static DataTable ReadTable(string StoredProcedureName, Dictionary<string, object> para = null)
        {
            try
            {
                // Result variable
                DataTable resultTable = new DataTable();

                // Create connection
                string SQLConnectionString = ConfigurationManager.ConnectionStrings["dbhasakiConnectionString"].ConnectionString;
                SqlConnection connection = new SqlConnection(SQLConnectionString);

                connection.Open();

                // Create and Assign properties for command
                SqlCommand sqlCmd = connection.CreateCommand();
                sqlCmd.Connection = connection;
                sqlCmd.CommandText = StoredProcedureName;
                sqlCmd.CommandType = CommandType.StoredProcedure;

                // Check parameters in Stored Procedure
                if (para != null)
                {
                    foreach (KeyValuePair<string, object> data in para)
                    {
                        if (data.Value == null)
                        {
                            sqlCmd.Parameters.AddWithValue("@" + data.Key, DBNull.Value);
                        }
                        else
                        {
                            sqlCmd.Parameters.AddWithValue("@" + data.Key, data.Value);
                        }
                    }
                }

                // Execute sqlCommand and Assign to result variable
                SqlDataAdapter sqlDA = new SqlDataAdapter();
                sqlDA.SelectCommand = sqlCmd;
                sqlDA.Fill(resultTable);

                connection.Close();

                return resultTable;
            }
            catch
            {
                return null;
            }
        }
        public static object Exec_Command(string StoredProcedureName, Dictionary<string, object> dic_param = null)
        {
            string SQLconnectionString = ConfigurationManager.ConnectionStrings["dbhasakiConnectionString"].ConnectionString;
            object result = null;
            using (SqlConnection conn = new SqlConnection(SQLconnectionString))
            {
                conn.Open();
                SqlCommand cmd = conn.CreateCommand();

                // Start a local transaction.

                cmd.Connection = conn;
                cmd.CommandText = StoredProcedureName;
                cmd.CommandType = CommandType.StoredProcedure;

                if (dic_param != null)
                {
                    foreach (KeyValuePair<string, object> data in dic_param)
                    {
                        if (data.Value == null)
                        {
                            cmd.Parameters.AddWithValue("@" + data.Key, DBNull.Value);
                        }
                        else
                        {
                            cmd.Parameters.AddWithValue("@" + data.Key, data.Value);
                        }
                    }
                }
                cmd.Parameters.Add("@currentid", SqlDbType.Int).Direction = ParameterDirection.Output;
                try
                {
                    cmd.ExecuteNonQuery();
                    result = cmd.Parameters["@currentid"].Value;
                    // Attempt to commit the transaction.

                }
                catch (Exception ex)
                {
                    result = null;
                }

            }
            return result;
        }
        public static NguoiDung DangKy_TaiKhoan(NguoiDung nd)
        {
            Dictionary<string, object> param = new Dictionary<string, object>();  
            param.Add("sodt", nd.SODIENTHOAI);
            param.Add("matkhauhash", nd.MATKHAUHASH);
            param.Add("matkhausalt", nd.MATKHAUSALT);
            param.Add("email", nd.EMAIL);
            int kq = int.Parse(Exec_Command("Proc_DangKy", param).ToString());
            if (kq > -1)
                nd.MAKH = kq;
            return nd;
        }
        public static NguoiDung DoiMatKhau(NguoiDung nd)
        {
            Dictionary<string, object> param = new Dictionary<string, object>();
            param.Add("makh", nd.MAKH);
            param.Add("matkhauhash", nd.MATKHAUHASH);
            param.Add("matkhausalt", nd.MATKHAUSALT);
            int kq = int.Parse(Exec_Command("Proc_DoiMatKhau", param).ToString());
            if (kq > -1)
                nd.MAKH = kq;
            return nd;
        }
        public static Forgot_Password CheckEmail(Forgot_Password fg)
        {
            Dictionary<string, object> param = new Dictionary<string, object>();
            param.Add("email", fg.gmail);         
            int kq = int.Parse(Exec_Command("Proc_Check_email", param).ToString());
            if (kq > -1)
                fg.makh = kq;
            return fg;
        }
        public static NguoiDung apiDangNhap(string SODIENTHOAI)
        {
            Dictionary<string, object> param = new Dictionary<string, object>();
            param.Add("sodt", SODIENTHOAI);
            DataTable tb = ReadTable("Proc_DangNhap", param);
            NguoiDung kq = new NguoiDung();
            if (tb.Rows.Count > 0)
            {
                kq.SODIENTHOAI = tb.Rows[0]["SODIENTHOAI"].ToString();
                kq.MATKHAUHASH = tb.Rows[0]["MATKHAUHASH"].ToString();
                kq.MATKHAUSALT = tb.Rows[0]["MATKHAUSALT"].ToString();
                kq.MAKH = int.Parse(tb.Rows[0]["MAKH"].ToString());
            }
            else
                kq.MAKH = 0;
            return kq;
        }
        public static void ThemGioHang(int MAKH, int MASP, int soluong)
        {
            Dictionary<string, object> param = new Dictionary<string, object>();
            param.Add("makh", MAKH);
            param.Add("masp", MASP);
            param.Add("soluong", soluong);
            ReadTable("Proc_ThemGioHang", param);
        }
    }
}