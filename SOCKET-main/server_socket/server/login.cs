using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Odbc;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Channels;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ToolBar;

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



		//object _lockobject;
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
					log_write("[ C u b r i d ]_ C o n n e c t e d !");
					//TxtInvoker(TextBox, "[Cubrid] Connected!!");

					if (_serverComm.dbConn.State == ConnectionState.Closed)
						return;
				}
			}
			catch (OdbcException)
			{
				log_write("[Cubrid] Connecting.....\r\n");
			}
			catch (Exception)
			{
				log_write("[Cubrid] Connecting.....\r\n");
			}
			finally
			{
				_serverComm.dbConn.Close();
			}
		}
		signup signup = new signup();
		private void btn_new_Click(object sender, EventArgs e)
		{
			signup.ShowDialog();
			log_write("[Cubrid] 회원가입 \r\n");
		}
		Server serverSoc = new Server();
		private void btn_login_Click(object sender, EventArgs e)
		{
			//string id = textBox1.Text; //①,②
			//string pw = textBox2.Text; //①,②
			log_write("[Cubrid] 로그인 \r\n");
			string userID = textBox1.Text;//③
			string userPW = textBox2.Text;//③

			//OdbcCommand cmd = new OdbcCommand();

			//cmd.CommandType = CommandType.Text;

			//cmd.CommandText = "SELECT id,pw FROM sock_table WHERE id = '" + id + "'&& pw = '" + pw + "';";

			//string query = "SELECT * FROM sock_table WHERE id=? and pw=?"; //①

			//string sql_cub_sel = "SELECT * FROM sock_table WHERE id=? and pw=?"; //③

			//string sql_cub_sel = "SELECT * FROM sock_table WHERE id=? and pw=?"; //④
			//																	 //OdbcParameter idParam = new OdbcParameter("@id", userID);
			//																	 //idParam.Value = textBox1.Text;
			//																	 //OdbcParameter pwParam = new OdbcParameter("@pw", userPW);
			//																	 //pwParam.Value = textBox2.Text;
			//																	 //string sql_cub_ins = "INSERT INTO sock (id,pw) VALUES (@id,@pw);";
			//																	 //OdbcConnection conn = new OdbcConnection(_serverComm.dbConnectString);//①,②
			
			//string query = "SELECT * FROM Products Where ProductID = @ProductID and ProductNAME = @ProductNAME;";
			//string query2 = "SET @userID='test'";
			//string query3 = "SET @userPW='1234'";
			//string query4 = "EXECUTE id_ready USING @userID,@userPW";

			//EXECUTE id_ready USING 


			using (var connect = new OdbcConnection(_serverComm.dbConnectString))
			{

				try
				{
					connect.Open(); //연결
					//string query1 = "PREPARE id_ready FROM 'SELECT * FROM sock_table WHERE id=? and pw=?'";

					string query = "SELECT * FROM sock_table Where id=? and pw=?;";
					using (var command = new OdbcCommand(query, connect)) //①sql_cub_sel , ②query1 
					{

						//command.Parameters.Add(idParam);//④
						//command.Parameters.Add(pwParam);//④
						command.Parameters.AddWithValue("@id", userID); //③
						command.Parameters.AddWithValue("@pw", userPW); //③

						//command.Prepare();//④
						//Console.WriteLine(query1);
						//int ss = command.ExecuteNonQuery();
						command.ExecuteNonQuery();


						DataSet dataSet = new DataSet();//①,②
						Server serverSoc = new Server();//①,②

						OdbcDataAdapter adapter = new OdbcDataAdapter(); //①//odbcdataAdapter는 서버에서 데이터를 가져온뒤 연결 끊음


						try//①,②
						{
							command.Connection = connect;

							if (_serverComm.dbOpenState)  //ok
							{
								adapter.SelectCommand = command;

								connect.Close();
								adapter.Fill(dataSet); // adapter에게 채운다. (dataSet)을.

								adapter.DeleteCommand = command;

								if (dataSet.Tables.Count != 0)            //ok
								{
									if (dataSet.Tables[0].Rows.Count > 0)     //ok
									{
										MessageBox.Show("로그인 되었습니다.");
										log_write("[로그인 완료]");
										serverSoc.ShowDialog();
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

						connect.Close();
					}
				}
				catch (Exception)
				{

					throw;
				}

			}



			/*using (conn)//①,②
			{
				conn.Open();//①,②

				using (var cmd = new OdbcCommand(query, conn))//①,②
				{
					// Param Value 추가

					/////////cmd.Parameters.AddWithValue("@id", Convert.ToInt32(usrinput));//③

					/////////cmd.Parameters.AddWithValue("@pw", Convert.ToInt32(name));//③


					cmd.Parameters.AddWithValue("@id", Convert.ToString(id));//①,②

					cmd.Parameters.AddWithValue("@pw", Convert.ToString(pw));//①,②



					cmd.ExecuteReader();//①,②



					DataSet dataSet = new DataSet();//①,②
					Server serverSoc = new Server();//①,②

					OdbcDataAdapter adapter = new OdbcDataAdapter(); //①//odbcdataAdapter는 서버에서 데이터를 가져온뒤 연결 끊음


					try//①,②
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
			}*//*
			//MessageBox.Show(cmd.CommandText);*/
		}
/*
		private void TxtInvoker(object sender,string text)
		{
			TextBox txt = sender as TextBox;

			if (txt.InvokeRequired)
			{
				txt.BeginInvoke(new MethodInvoker(delegate { txt.AppendText(DateTime.Now.ToString("[yyyy-MM-dd HH:mm:ss:ff]") + txt); }));
			}
			else
			{
				txt.AppendText(DateTime.Now.ToString("[yyyy-MM-dd HH:mm:ss:ff]") + txt);
			}
		}	*/

		private void log_write(string text)
		{
			

			DateTime nowDate = DateTime.Now;
			string dateLogTxt = nowDate.ToString("[yyyy-MM-dd HH:mm:ss:ff]"+text);
			string txtEnd = ".txt";

			string Filepath = @"C:\Users\ansetech80\source\repos\SOCKET_DETAIL\SOCKET-main\server_socket\server\Log"+txtEnd;
			// filepath 텍스트 파일 위치.
			string Directoryroot = @"C:\Users\ansetech80\source\repos\SOCKET_DETAIL\SOCKET-main\server_socket\server";
			// directory 위치. 저장할 폴더 위치
			FileInfo txtfile = new FileInfo(Filepath);
			DirectoryInfo directory = new DirectoryInfo(Directoryroot);

			try
			{
				if (!directory.Exists)
				{
					directory.Create();
				}
				else
				{
					if (txtfile.Exists == false)
					{

						using (StreamWriter sw = new StreamWriter(Filepath))
						{
							sw.WriteLine(dateLogTxt);
						}
						File.AppendAllText(Filepath, DateTime.Now.ToString("[yyyy-MM-dd HH:mm:ss]\n"));
					}
					else
					{
						using (StreamWriter sw = File.AppendText(Filepath))
						{
							sw.WriteLine(dateLogTxt);
							sw.Close();
						}
						//var sw = new StreamWriter(Fpath, true);
						//sw.Write(DateTime.Now.ToString("[yyyy-MM-dd HH:mm:ss]\n"));
						//File.AppendAllText(Fpath, DateTime.Now.ToString("[yyyy-MM-dd HH:mm:ss]\n"));
						
					}


				}



			}
			catch (Exception)
			{

				throw;
			}
			/*string temp;

			DirectoryInfo directory= new DirectoryInfo(DirPath);
			FileInfo fileInfo= new FileInfo(FilePath);

			try
			{
				if (!directory.Exists) Directory.CreateDirectory(DirPath);
				if (fileInfo.Exists) {
					using (StreamWriter sw = new StreamWriter(FilePath))
					{
						temp = string.Format("[{0}]{1}", DateTime.Now, str);
						sw.WriteLine(temp);
						sw.Close();
					}
				}
				else
				{
					using (StreamWriter sw = File.AppendText(FilePath))
					{
						temp = string.Format("[{0}]{1}",DateTime.Now, str);
						sw.WriteLine(temp);
						sw.Close();
					}
				}
			}
			catch (Exception)
			{

				throw;
			}*/
		}

		

		private void btn_login_MouseHover(object sender, EventArgs e)
		{
			btn_login.BackColor = Color.LightGray;
			btn_login.Image = null;
		}

		private void btn_login_MouseLeave(object sender, EventArgs e)
		{
			
			btn_login.BackColor = Color.Transparent;
			btn_login.Image = Properties.Resources.cloud_small;
		
			
		}

		
	}
}

