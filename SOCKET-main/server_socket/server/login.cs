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

		public cubridConn con = new cubridConn();
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
				con.xmlConfig = new XmlDocument();
				con.xmlConfig.Load(Application.StartupPath + "/login.xml"); // xml파일을 로드(시작점은 login.xml 파일로 부터)

				// database
				XmlNode dbConnectString = con.xmlConfig.SelectSingleNode("/DB_Connect");

				//xml노드 dbcon 은 cubrid. xml파일로드한 것. 
				con.dbConnectString = dbConnectString.InnerText;
				//MessageBox.Show(con.dbConnectString);
			}
			catch (Exception)
			{

				MessageBox.Show("실행 파일 에러. 관리자에게 문의하세요.");
			}
		}

		private void Thread_CubridConnect()
		{
			System.Threading.Timer thTimer_DB = new System.Threading.Timer(DB_Connect);
			thTimer_DB.Change(0, 2000); //OK
		}



		object _lockobject;
		private void DB_Connect(object sender)
		{
			try
			{
				if (!con.dbOpenState)
				{
					con.dbConn = new OdbcConnection(con.dbConnectString);
					con.dbConn.Open();
					con.dbOpenState = true;
					if (con.dbConn.State == ConnectionState.Closed)
						return;
				}
			}
			catch (OdbcException)
			{
				MessageBox.Show("연결 실패");
			}
			catch (Exception ex)
			{
				
				MessageBox.Show("연결 실패~~~~~");
				MessageBox.Show(ex.Message);
				
			}
			finally
			{
				con.dbConn.Close();
			}
		}
		private void btn_new_Click(object sender, EventArgs e)
		{
			signup signup = new signup();
			signup.ShowDialog();
		}

		private void btn_login_Click(object sender, EventArgs e)
		{
			// 외부 입력 값 

			string userinput ="test";

			string name ="1234";



			//파라미터 바인딩을 위해 @ 을 사용한다. 외부입력 값에 의해 쿼리 구조 변경을 할 수 없다.

			string query = "SELECT id,pw FROM sock_table Where id=@ProductID and pw=@ProductNAME;";
			try
			{
				using (var conn = con.dbConn)
				{
					using (var cmd = new OdbcCommand(query, conn))

					{
						
						// Param Value 추가

						cmd.Parameters.AddWithValue("@ProductID", userinput);

						cmd.Parameters.AddWithValue("@ProductNAME", Convert.ToInt32(name));

						conn.Open();

						cmd.ExecuteReader();

					}
					conn.Close();
				}
			}
			catch (OdbcException ex)
			{
				MessageBox.Show(ex.Message);
			}


		}

		/*
					string id = textBox1.Text;
					string pw = textBox2.Text;

					OdbcCommand cmd = new OdbcCommand();
					cmd.CommandType = CommandType.Text;
					cmd.CommandText = "SELECT id,pw FROM sock_table WHERE id = '" + id + "'&& pw = '" + pw + "';";



					OdbcConnection conn = new OdbcConnection(con.dbConnectString);

					//MessageBox.Show(cmd.CommandText);
							DataSet dataSet = new DataSet();

		*/

		/*try
		{
			cmd.Connection = conn;

			if (con.dbOpenState)  //ok
			{
				OdbcDataAdapter adapter = new OdbcDataAdapter(cmd);
				adapter.Fill(dataSet);
				adapter.DeleteCommand = cmd;

				if (dataSet.Tables.Count != 0)            //ok
				{
					if (dataSet.Tables[0].Rows.Count > 0)     //ok
					{
						MessageBox.Show("로그인 되었습니다.");

						while (true)  //ok
						{

							if (true)
							{
						cmd.CommandText = "SELECT id,pw FROM sock_table WHERE id = '" + id + "'&& pw = '" + pw + "';";



							}break;

						}
					} else
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

				else
				{
					MessageBox.Show("로그인.");
				}



			} else
			{
				MessageBox.Show("연결을 확인하세요");
			}
		}
		catch (OdbcException)
		{
			MessageBox.Show("DB연결을 확인하세요");
		}*/
	}

}

