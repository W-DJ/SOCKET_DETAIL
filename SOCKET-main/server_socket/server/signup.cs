using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Odbc;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using System.Net;
using System.IO;
using System.Threading;

namespace server
{
	public partial class signup : Form
	{
		public struct cubridConn     //cubrid 연결
		{


			public XmlDocument xmlConfig; //xml설정 가져오기

			public string dbConnectString;
			public bool dbOpenState;
			public OdbcConnection dbConnection; // 큐브리드랑 연결할 거
		}
		public cubridConn Comm = new cubridConn(); //연결 집합


		public signup()
		{
			InitializeComponent();
			ThreadCubridConnect();
		}

		private void signup_Load(object sender, EventArgs e)
		{
			try
			{
				Comm.xmlConfig = new XmlDocument();
				Comm.xmlConfig.Load(Application.StartupPath + "/login.xml"); //xml파일 주소

				//db
				XmlNode dbConnectString = Comm.xmlConfig.SelectSingleNode("/DB_Connect");

				//xml노트 dbConString은 xml 안의 내용
				Comm.dbConnectString = dbConnectString.InnerText;
			}
			catch (Exception)
			{
				MessageBox.Show("실행파일 에러, 관리자에게 문의바람");
			}
		}

		private void ThreadCubridConnect()
		{
			System.Threading.Timer timerDB = new System.Threading.Timer(DB_Connect);
			//DB 컨넥트를 만들어주자.
			timerDB.Change(0, 2000); //OK
		}

		private void DB_Connect(object sender)
		{
			try
			{
				if (!Comm.dbOpenState)
				{
					Comm.dbConnection = new OdbcConnection(Comm.dbConnectString);
					Comm.dbConnection.Open();
					Comm.dbOpenState = true;
				}
			}
			catch (OdbcException)
			{
				Comm.dbConnection.Close();
				Comm.dbOpenState = false;
			}

			catch (Exception)
			{

				Comm.dbConnection.Close();
				Comm.dbOpenState = false;
			}
		}
		//DB 연결 준비 끝
		private void signbtn_Click(object sender, EventArgs e)
		{
			OdbcConnection conn = new OdbcConnection(Comm.dbConnectString);
			DataSet ds = new DataSet();
			OdbcCommand cmd = new OdbcCommand(); // Odbc커맨드
			cmd.CommandType = CommandType.Text; //커맨드 종류는 문자

			string name = textBox1.Text;
			string id = textBox2.Text;
			string pw = textBox3.Text;

			cmd.CommandText = "INSERT INTO sock_table (name, id, pw) VALUES " + "('" + name + "','" + id + "','" + pw + "');";
			string welcome = id + "님 가입을 축하드립니다.";

			//MessageBox.Show(cmd.CommandText);


			try
			{
				cmd.Connection = conn;  // Odbc커맨드 객체 cmd.연결 = conn(xml값)

				if (Comm.dbOpenState)
				{
					OdbcDataAdapter adapter = new OdbcDataAdapter(cmd);
					adapter.Fill(ds);
					adapter.DeleteCommand = cmd;
					MessageBox.Show(welcome);
					
					
				}


			}
			catch(Exception)
			{
				throw;
			}

			this.Close();
				
	
		}


	}
}
