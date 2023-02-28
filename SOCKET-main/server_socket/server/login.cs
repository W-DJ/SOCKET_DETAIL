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

		private void DB_Connect(object sender)
		{
			try
			{
				if (!con.dbOpenState)
				{
					con.dbConn = new OdbcConnection(con.dbConnectString);
					con.dbConn.Open();
					con.dbOpenState = true;

				}
			}
			catch (OdbcException)
			{
				con.dbConn.Close();
				con.dbOpenState = false;
			}
			catch (Exception)
			{
				con.dbConn.Close();
				con.dbOpenState = false;
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
			
			OdbcCommand cmd = new OdbcCommand();
			cmd.CommandType = CommandType.Text;
			cmd.CommandText = "SELECT id,pw FROM sock_table WHERE id = '" + id + "',pw = '"+pw+"';";

			OdbcConnection conn = new OdbcConnection(con.dbConnectString);

			//MessageBox.Show(cmd.CommandText);
			
			
			conn.Open();

			try
			{
				if (con.dbOpenState)
				{
					DataSet dataSet = new DataSet();
					cmd.Connection = con.dbConn;
					OdbcDataAdapter adapter = new OdbcDataAdapter(cmd);

					adapter.Fill(dataSet);
					adapter.DeleteCommand = cmd;

					//같은 id와 pw와 있는 지 돌려야지

					// id는 primary key니까 중복없을 것이고
					//pw는 3번이상 틀릴 경우 block 처리 하는 조건을 걸어야겠군.

					if (dataSet.Tables[0].Rows.Count >= 1)
					{
						MessageBox.Show(cmd.CommandText);
					}
					else
					{
						MessageBox.Show("데이터가 없습니다.");
					}



				}
			}
			catch (OdbcException)
			{
				conn.Close();
				return;
			}
		}
	
	}
}
