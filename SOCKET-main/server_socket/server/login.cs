using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Odbc;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace server
{
	public partial class login : Form
	{

		public struct cubridConn // cubrid 연결
		{
			public XmlDocument xmlConfig; // xml 설정 가져오기
			public string dbConnectString;
			public bool dbOpenState;
			public OdbcConnection dbConn;//큐브리드 연결.

		}

		public cubridConn _serverComm = new cubridConn();
		public login()
		{
			InitializeComponent();
			Thread_CubridConnect();

		}

		public Server socNaverBlog;
		private void login_Load(object sender, EventArgs e)
		{
			try
			{
				//xml 주소 
				_serverComm.xmlConfig = new XmlDocument();
				_serverComm.xmlConfig.Load(Application.StartupPath + "/login.xml"); // xml파일을 로드(시작점은 login.xml 파일로 부터)

				// database
				XmlNode dbConnectString = _serverComm.xmlConfig.SelectSingleNode("/DB_Connect");

				//xml노드 dbcon 은 cubrid. xml파일로드한 것. 
				_serverComm.dbConnectString = dbConnectString.InnerText;

				//MessageBox.Show(_serverComm.dbConnectString + "안녕하세요");
			}
			catch (Exception)
			{


				MessageBox.Show("실행 파일 에러. 관리자에게 문의하세요.");
			}
		}

		private void Thread_CubridConnect()
		{
			System.Threading.Timer timerDB = new System.Threading.Timer(DB_Connect);
			//DB 컨넥트를 만들어주자.
			timerDB.Change(0, 2000); //OK
		}



		object _lockobject;
		private void DB_Connect(object sender)
		{
			try
			{
				if (!_serverComm.dbOpenState)
				{
					_serverComm.dbConn = new OdbcConnection(_serverComm.dbConnectString);
					//MessageBox.Show(_serverComm.dbConnectString);
					_serverComm.dbConn.Open();
					_serverComm.dbOpenState = true;
					if (_serverComm.dbConn.State == ConnectionState.Closed)
						return;
				}
			}
			catch (OdbcException)
			{
			}
			catch (Exception ex)
			{

			}
			finally
			{
				_serverComm.dbConn.Close();
			}
		}
		private void btn_new_Click(object sender, EventArgs e)
		{
			signup signup = new signup();
			signup.ShowDialog();
		}

		private void btn_login_Click(object sender, EventArgs e)
		{
			string id = textBox1.Text;
			string pw = textBox2.Text;

			////////string usrinput = "ID";
			////////string name = "NAME";

			//OdbcCommand cmd = new OdbcCommand();

			//cmd.CommandType = CommandType.Text;

			//cmd.CommandText = "SELECT id,pw FROM sock_table WHERE id = '" + id + "'&& pw = '" + pw + "';";

			string query = "SELECT * FROM sock_table WHERE id=? and pw=?";

			OdbcConnection conn = new OdbcConnection(_serverComm.dbConnectString);

			using (conn)
			{
				conn.Open();

				using (var cmd = new OdbcCommand(query, conn))
				{
					// Param Value 추가

					/////////cmd.Parameters.AddWithValue("@id", Convert.ToInt32(usrinput));

					/////////cmd.Parameters.AddWithValue("@pw", Convert.ToInt32(name));

					
					cmd.Parameters.AddWithValue("@id", Convert.ToString(id));
			
					cmd.Parameters.AddWithValue("@pw", Convert.ToString(pw));

			

					cmd.ExecuteReader();

				

					DataSet dataSet = new DataSet();
					Server serverSoc = new Server();

					OdbcDataAdapter adapter = new OdbcDataAdapter(); //odbcdataAdapter는 서버에서 데이터를 가져온뒤 연결 끊음


					try
					{
						cmd.Connection = conn;

						if (_serverComm.dbOpenState)  //ok
						{
							adapter.SelectCommand = cmd;
							
					conn.Close();
							adapter.Fill(dataSet); // adapter에게 채운다. (dataSet)을.

							adapter.DeleteCommand = cmd;

							if (dataSet.Tables.Count != 0)            //ok
							{
								if (dataSet.Tables[0].Rows.Count > 0)     //ok
								{
									MessageBox.Show("로그인 되었습니다.");
									serverSoc.ShowDialog();



									while (true)  //ok
									{
										if (true)
										{
											//cmd.CommandText = "SELECT id,pw FROM sock_table WHERE id = ?&& pw = ?;";
										}
										break;
									}
								}
								else
								{
									MessageBox.Show("아이디,비밀번호를 확인하세요");
									return;
								}
							}
							//같은 id와 pw와 있는 지 돌려야지

							// id는 primary key니까 중복없을 것이고
							//pw는 3번이상 틀릴 경우 block 처리 하는 조건을 걸어야겠군.

							if (dataSet.Tables.Count == 0)
							{
								MessageBox.Show("데이터 없음");
							}

						}
						else
						{
							MessageBox.Show("연결을 확인하세요");
						}
					}
					catch (OdbcException)
					{
						MessageBox.Show("DB연결을 확인하세요");
					}

					conn.Close();
				}
			}
			//MessageBox.Show(cmd.CommandText);
		}
	}
}

