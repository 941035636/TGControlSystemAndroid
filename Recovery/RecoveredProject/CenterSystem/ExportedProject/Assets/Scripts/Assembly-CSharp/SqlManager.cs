using System;
using System.Data.SqlClient;
using UnityEngine;

public class SqlManager : MonoBehaviour
{
	private static SqlManager Instance;

	private readonly string SqlAddress = "";

	private readonly string sqlurl = Environment.GetEnvironmentVariable("TG_CONTROL_SQL_CONNECTION") ?? string.Empty;

	private readonly string sqlurlEditor = Environment.GetEnvironmentVariable("TG_CONTROL_SQL_EDITOR_CONNECTION") ?? string.Empty;

	public static SqlManager GetInstance()
	{
		if (Instance == null)
		{
			Instance = new SqlManager();
		}
		return Instance;
	}

	private void Start()
	{
	}

	public string Get(string acc)
	{
		try
		{
			string result = string.Empty;
			SqlConnection sqlConnection = new SqlConnection(sqlurl);
			sqlConnection.Open();
			SqlCommand sqlCommand = new SqlCommand("select * from userinfo where account=@account", sqlConnection);
			sqlCommand.Parameters.AddWithValue("account", acc);
			SqlDataReader sqlDataReader = sqlCommand.ExecuteReader();
			if (sqlDataReader.HasRows)
			{
				while (sqlDataReader.Read())
				{
					result = sqlDataReader["deadlinedate"].ToString();
				}
			}
			sqlDataReader.Close();
			return result;
		}
		catch (Exception)
		{
			return "false";
		}
	}

	public bool IsExituser(string account)
	{
		SqlConnection sqlConnection = new SqlConnection(sqlurl);
		sqlConnection.Open();
		SqlCommand sqlCommand = new SqlCommand("select *from userinfo where account=@account", sqlConnection);
		sqlCommand.Parameters.AddWithValue("account", account);
		return sqlCommand.ExecuteReader().HasRows;
	}

	public string Add(string account, string password, string registerdate, int prescription, int authority, string deadlinedate, int longtime, string productnum, string activecode)
	{
		try
		{
			if (IsExituser(account))
			{
				return "已经存在该用户名，请换一个";
			}
			SqlConnection sqlConnection = new SqlConnection(sqlurl);
			sqlConnection.Open();
			new SqlCommand("INSERT INTO userinfo(account,password,registerdate,prescription,authority,deadlinedate,longtime,productnum,activecode) VALUES('" + account + "','" + password + "','" + registerdate + "','" + prescription + "','" + authority + "','" + deadlinedate + "','" + longtime + "','" + productnum + "','" + activecode + "')", sqlConnection).ExecuteNonQuery();
			sqlConnection.Close();
		}
		catch (Exception ex)
		{
			Debug.LogError(ex.ToString());
			return ex.ToString();
		}
		return "用户添加成功";
	}
}
