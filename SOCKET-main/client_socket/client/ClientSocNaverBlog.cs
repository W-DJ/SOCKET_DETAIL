using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Runtime.Remoting.Contexts;
using System.Data.Odbc;
using System.Xml;
using System.Xml.Linq;
using MySql.Data.MySqlClient;

namespace client
{
	public partial class Client : Form
	{
		//private static extern int GetPrivateProfileString(String section, String key, String def, StringBuilder refVal, int size, String filePath);
		private Socket dataSocket; //연결 소켓
		private bool isOpen; // 연결 확인
		private byte[] buffer; // 메모리

		

		public struct mariaConn
		{
			public XmlDocument xmlConfig;
			public string mariaConnectionString;
			public bool mariaOpenState;
			public OdbcConnection _mariaConn;

		}

		public mariaConn _mariaComm = new mariaConn();




		public Client()
		{
			InitializeComponent();
			//MariaDB_Threading();
		}


		private void Client_Load(object sender, EventArgs e)
		{
			dataSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP); //왜 IP로 하지

			try
			{
				//xml 주소 
				_mariaComm.xmlConfig = new XmlDocument();
				_mariaComm.xmlConfig.Load(Application.StartupPath + "/data.xml"); // xml파일을 로드(시작점은 data.xml 파일로 부터)

				// database
				XmlNode mariaConnectionString = _mariaComm.xmlConfig.SelectSingleNode("/DB_Connect");

				//xml노드 dbcon 은 cubrid. xml파일로드한 것. 
				_mariaComm.mariaConnectionString = mariaConnectionString.InnerText;

				//MessageBox.Show(_mariaComm.mariaConnectionString + "안녕하세요");
			}
			catch (Exception)
			{


				throw;
			}
		}



		private void button1_Click_1(object sender, EventArgs e)
		{
			try
			{
				if (isOpen && dataSocket.Connected)
				{
					MessageBox.Show("소켓이 이미 열려 있습니다.");
					return;
				}
				isOpen = true;

				dataSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);
				dataSocket.Connect("10.10.10.60", int.Parse("5000"));
				buffer = new byte[1024];
				dataSocket.BeginReceive(buffer, 0, buffer.Length, 0, OnReceive, null);
				MessageBox.Show("소켓이 열렸습니다.");
			}
			catch (Exception ex)
			{
				MessageBox.Show("소켓을 열 수 없습니다.");
				MessageBox.Show(ex.Message);
			}


		}

		private void btnClose_Click(object sender, EventArgs e)
		{
			if (!isOpen)
			{
				MessageBox.Show("소켓이 열려있지 않습니다.");
				return;   //돌려보내기
			}
			string msg = "Close"; // 보내는 msg
			dataSocket.Send(Encoding.Default.GetBytes(msg)); //데이터 소켓은 보낸다 (인코딩해서 기본적인 ms 바이트를 .
			//MessageBox.Show("보낸 메시지 : " + msg);

			dataSocket.Close();
			MessageBox.Show("소켓을 닫았습니다.");
			dataSocket = null;
		}

		private void btnSend_Click(object sender, EventArgs e)
		{
			if (!isOpen)
			{
				MessageBox.Show("소켓을 열어주세요.");
				return;

			} else
			{


			string msg = textBox3.Text;
			dataSocket.Send(Encoding.Default.GetBytes(msg));
			//MessageBox.Show("보낸 메시지 : " + msg);
			listChat.Items.Add(" 회원 : " + msg);

				DBTest();

				textBox3.Clear();

				
			
			}

		}

		private void DBTest()
		{
			string conn_string = "Server=10.10.10.60;Port=3306;Database=test;Uid=root;Pwd=ansetech";
			MySqlConnection conn= new MySqlConnection(conn_string);
			MySqlCommand cmd = conn.CreateCommand();

			string NAM = "client";
			string CHAT = textBox3.Text;


			//string sql_makedb = "INSERT INTO sock (name,chat) VALUES"+"('"+NAM+"','"+CHAT+"');";  //①
			//string sql_makedb = "INSERT INTO sock (name,chat) VALUES (?,?);"; //②
			string sql_makedb = "INSERT INTO sock (name,chat) VALUES (@name,@chat);"; //③

			//MessageBox.Show(sql_makedb);
			cmd.CommandText = sql_makedb;
		

			try
			{
				conn.Open();

				cmd.Parameters.AddWithValue("@name", NAM); //②,③
				cmd.Parameters.AddWithValue("@chat", CHAT); //②,③

				cmd.ExecuteNonQuery();

				conn.Close();
			}
			catch (Exception)
			{

				throw;
			}
			//MySqlDataReader reader = cmd.ExecuteReader();


			/*while (reader.Read())  // reader 계속 읽는 동안에
			{
				MessageBox.Show(reader["id"].ToString()); // 메시지박스로 읽은 id 컬럼을 출력한다.
			}*/
			
			//MySqlCommand cmd = new MySqlCommand(sql_makedb, conn);

			
			
		}
		private void OnReceive(IAsyncResult ar)//받기 IA 싱크 결과값을 ar로 받는다.
		{
			int received = dataSocket.EndReceive(ar); //받으면 1인가.

			if (dataSocket==null) {
				MessageBox.Show("서버 소켓이 종료 되었습니다.");
			}
			string data = Encoding.Default.GetString(buffer); // 버퍼 메모리를 스트링으로 변환하여 data에 저장

			dataSocket.BeginReceive(buffer, 0, buffer.Length, 0, OnReceive, null);

			listChat.Invoke(new Action(delegate ()
			{
				listChat.Items.Add(" 관리자 :" + data);
			}));


		}


		private void textBox3_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == System.Windows.Forms.Keys.Enter)
			{
				btnSend_Click(sender, e);
			}
		}

		/*private void MariaDB_Threading()
		{


			System.Threading.Timer Maria_Connect = new System.Threading.Timer(DB_Connect);
			Maria_Connect.Change(0, 5000); // 쓰레드 돌리는 타임


		}

		private void DB_Connect(object sender)
		{
			try
			{
				if (_mariaComm.mariaOpenState)
				{
					_mariaComm._mariaConn = new OdbcConnection(_mariaComm.mariaConnectionString);
					MessageBox.Show(_mariaComm.mariaConnectionString);
					_mariaComm._mariaConn.Open();
					_mariaComm.mariaOpenState = true;
					MessageBox.Show("Maria DB _ Connected");
				}
				else
				{
					//이미 준비 상태 
				}
			}
			catch (OdbcException)
			{
				throw;
				_mariaComm.mariaOpenState = false;
				if (_mariaComm._mariaConn != null)
				{
					_mariaComm._mariaConn.Dispose();

				}
				MessageBox.Show("MariaDb _ OdbcEx ");
			catch (Exception)
			{
				throw;
				_mariaComm.mariaOpenState = false;
				if (_mariaComm._mariaConn != null)
				{
					_mariaComm._mariaConn.Dispose();
				}
				MessageBox.Show("MariaDb _ Ex ");
			}
		}*/
	}
}

/*OdbcConnection conn = new OdbcConnection(_mariaComm.mariaConnectionString);
			DataSet ds = new DataSet();
			string cmdText = "SELECT * FROM sock_table WHERE id=?;";
			
			OdbcCommand cmd = new OdbcCommand(cmdText, conn);
				//MessageBox.Show(cmdText);
				
			cmd.CommandType = CommandType.Text;

			try
			{
				cmd.Connection = conn;


				if (_mariaComm.mariaOpenState)
				{
					OdbcDataAdapter adapter = new OdbcDataAdapter(cmd);
					adapter.Fill(ds);
					adapter.DeleteCommand= cmd;
					MessageBox.Show("welcome");
					
				}
			}
			catch (Exception)
			{

				throw;
			}*/