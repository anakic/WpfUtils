using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Data.Sql;
using System.Data;
using System.Text.RegularExpressions;
using System.Data.SqlClient;
using System.ComponentModel;
using System.Reflection;
using System.Linq.Expressions;
using Thingie.WPF.Controls.PropertiesEditor.CustomEditing;

namespace Thingie.WPF.Controls
{
	/// <summary>
	/// Interaction logic for DbConnectionEditor.xaml
	/// </summary>
	public partial class DbConnectionEditor : UserControl, ICustomEditor
    {
        DbConnectionData _connData;
        DbConnectionEditorVM _vm;

        public DbConnectionEditor()
        {
            InitializeComponent();
        }

        #region IPropertyEditor Members

        public object Value
        {
            get
            {
                return _connData.ToString();
            }
            set
            {
                _connData = new DbConnectionData();
                _connData.Parse((string)value);
                _vm = new DbConnectionEditorVM(_connData);
                this.DataContext = _vm;
            }
        }

        #endregion

        string _lastServerConnectionTry;
        private void cbDataBases_GotFocus(object sender, RoutedEventArgs e)
        {
            if (_connData.ToString() != _lastServerConnectionTry)
            {
                _lastServerConnectionTry = _connData.ToString();
                _vm.FindDatabasesCommand.Execute(null);
            }
        }

        bool _serverInstancesLoaded = false;
        private void cbServerInstances_GotFocus(object sender, RoutedEventArgs e)
        {
            if (!_serverInstancesLoaded)
            {
                _vm.FindServerInstancesCommand.Execute(null);
                _serverInstancesLoaded = true;
            }
        }

        private void btnTest_Click(object sender, RoutedEventArgs e)
        {
            if (_connData.Test())
                MessageBox.Show("Connection successful!", "DB connection", MessageBoxButton.OK, MessageBoxImage.Information);
            else
                MessageBox.Show("Connection faild!", "DB connection", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    public class DbConnectionData
    {
        public string ServerInstance { get; set; }
        public bool IntegratedSecurity { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }

        public string DataBase { get; set; }

        public DbConnectionData()
        {
            IntegratedSecurity = true;
        }

        public void Parse(string connectionString)
        {
            string[] parts = connectionString.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string part in parts)
            {
                Match m = Regex.Match(part, @"(?'left'[^=]*)=(?'right'.*)");
                if (m.Success)
                {
                    string left = m.Groups["left"].Value;
                    string right = m.Groups["right"].Value;

                    switch (left.ToLower())
                    {
                        case "server":
                        case "data source":
                            ServerInstance = right;
                            break;
                        case "database":
                        case "initial catalog":
                            DataBase = right;
                            break;
                        case "integrated security":
                            if (right.ToLower() == "sspi")
                                IntegratedSecurity = true;
                            else
                                IntegratedSecurity = false;
                            break;
                        case "trusted_connection":
                            bool trusted;
                            Boolean.TryParse(right, out trusted);
                            IntegratedSecurity = trusted;
                            break;
                        case "user id":
                            UserName = right;
                            break;
                        case "password":
                            Password = right;
                            break;
                    }
                }
            }
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("Server={0};", this.ServerInstance);
            sb.AppendFormat("DataBase={0};", this.DataBase);
            if (this.IntegratedSecurity)
            {
                sb.Append("Trusted_Connection=True;");
                sb.Append("MultipleActiveResultSets=True;");
            }
            else
            {
                sb.AppendFormat("User ID={0};", this.UserName);
                sb.AppendFormat("Password={0};", this.Password);
                sb.Append("Trusted_Connection=False;");
                sb.Append("MultipleActiveResultSets=True;");
            }
            return sb.ToString();
        }

        public bool Test()
        {
            SqlConnection connection = new SqlConnection(this.ToString());
            try
            {
                connection.Open();
                connection.Close();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public IEnumerable<string> GetDatabases()
        {
            SqlConnection connection = new SqlConnection(this.ToString());
            try
            {
                connection.Open();
                List<string> databases = new List<string>();
                foreach (DataRow row in connection.GetSchema("Databases").Rows)
                {
                    databases.Add((string)row[0]);
                }
                return databases;
            }
            catch
            {
                return null;
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                    connection.Close();
            }
        }

        public IEnumerable<string> GetInstances()
        {
            DataTable sourcesDT = SqlDataSourceEnumerator.Instance.GetDataSources();
            List<string> instanceList = new List<string>();
            foreach (DataRow row in sourcesDT.Rows)
            {
                string instanceFullName = (string)row["ServerName"];
                if (row["InstanceName"] != DBNull.Value)
                    instanceFullName += @"\" + (string)row["InstanceName"];
                instanceList.Add(instanceFullName);
            }
            return instanceList;
        }
    }

    public class DbConnectionEditorVM : INotifyPropertyChanged
    {
        DbConnectionData _connData;

        public string SelectedInstance
        {
            get { return _connData.ServerInstance; }
            set
            {
                _connData.ServerInstance = value;
                DataBases = null;
                OnPropertyChanged(() => this.SelectedInstance);
            }
        }

        public bool IntegratedSecurity
        {
            get { return _connData.IntegratedSecurity; }
            set
            {
                _connData.IntegratedSecurity = value;
                OnPropertyChanged(() => this.IntegratedSecurity);
            }
        }

        public string UserName
        {
            get { return _connData.UserName; }
            set
            {
                _connData.UserName = value;
                OnPropertyChanged(() => this.UserName);
            }
        }

        public string Password
        {
            get { return _connData.Password; }
            set
            {
                _connData.Password = value;
                OnPropertyChanged(() => this.Password);
            }
        }

        public string DataBase
        {
            get { return _connData.DataBase; }
            set
            {
                _connData.DataBase = value;
                OnPropertyChanged(() => this.DataBase);
            }
        }

        IEnumerable<string> _serverInstances;
        public IEnumerable<string> ServerInstances
        {
            get { return _serverInstances; }
            set
            {
                _serverInstances = value;
                OnPropertyChanged(() => this.ServerInstances);
            }
        }

        IEnumerable<string> _dataBases;
        public IEnumerable<string> DataBases
        {
            get { return _dataBases; }
            set
            {
                _dataBases = value;
                OnPropertyChanged(() => this.DataBases);
            }
        }

        public DbConnectionEditorVM(DbConnectionData connData)
        {
            _connData = connData;
            _dataBases = new string[] { _connData.DataBase };
            _serverInstances = new string[] { _connData.ServerInstance };
        }

        RelayCommand _findDatabasesCommand;
        public RelayCommand FindDatabasesCommand
        {
            get
            {
                if (_findDatabasesCommand == null)
                {
                    _findDatabasesCommand = new RelayCommand(p =>
                    {
                        DataBases = _connData.GetDatabases();
                    });
                }
                return _findDatabasesCommand;
            }
        }

        RelayCommand _findServersCommand;
        public RelayCommand FindServerInstancesCommand
        {
            get
            {
                if (_findServersCommand == null)
                {
                    _findServersCommand = new RelayCommand(p =>
                    {
                        ServerInstances = _connData.GetInstances();
                    });
                }
                return _findServersCommand;
            }
        }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged<T>(Expression<Func<T>> propFunc)
        {
            string propName = ((propFunc.Body as MemberExpression).Member as PropertyInfo).Name;

			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
		}

        #endregion
    }
}
