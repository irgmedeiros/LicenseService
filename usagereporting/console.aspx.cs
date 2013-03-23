using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Web.UI.DataVisualization.Charting;
using System.Data.SqlClient;
using LogicNP.CryptoLicensing;
using System.Data.Common;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

namespace LicService
{
    enum ChartDisplayType
    {
        FeatureUsageByFeature,
        FeatureUsageByDay,
        AppRunsWithVersion,
        AppRunsWithoutVersion,
        AppRunsByDay,
        SysProfileOS,
        SysProfileCLR,
    }

    class QueryParameter
    {
        internal QueryParameter(string name, object value)
        {
            this.name = name;
            this.value = value;
        }

        internal string name;
        internal object value;
    }

    public partial class UsageReportingConsole : System.Web.UI.Page
    {
        ChartDisplayType ChartType
        {
            get
            {
                try
                {
                    return (ChartDisplayType)Session["ChartType"];
                }
                catch { }

                return ChartDisplayType.FeatureUsageByFeature;
            }
            set
            {
                try
                {
                    Session["ChartType"] = value;
                }
                catch { }
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            WebUserControl11.BtnApply.Click += new EventHandler(BtnApply_Click);
            WebUserControl11.BtnClearFilters.Click += new EventHandler(BtnClearFilters_Click);
            WebUserControl11.DtFrom.SelectionChanged += new EventHandler(DtFrom_SelectionChanged);
            WebUserControl11.DtTo.SelectionChanged += new EventHandler(DtFrom_SelectionChanged);
            WebUserControl11.DtFrom.VisibleMonthChanged += new MonthChangedEventHandler(DtFrom_VisibleMonthChanged);
            WebUserControl11.DtTo.VisibleMonthChanged += new MonthChangedEventHandler(DtFrom_VisibleMonthChanged);
            WebUserControl11.CmbSettingFiles.SelectedIndexChanged += new EventHandler(CmbSettingFiles_SelectedIndexChanged);

            if (!IsPostBack)
            {
                InitSettingFilesDropDown();
                InitChart();
            }
            else
            {
                string id = Request.Form["__EVENTTARGET"];
                if (id.EndsWith("dtFrom") || id.EndsWith("dtTo"))
                {
                    InitChart();
                }
            }
        }

        void CmbSettingFiles_SelectedIndexChanged(object sender, EventArgs e)
        {
            InitChart();
        }

         void InitSettingFilesDropDown()
        {
            WebUserControl11.CmbSettingFiles.Items.Clear();
            foreach (string file in Directory.GetFiles(install.basePath, "*.xml"))
            {
                WebUserControl11.CmbSettingFiles.Items.Add(Path.GetFileName(file));
            }
            WebUserControl11.CmbSettingFiles.SelectedIndex = 0;
        }


        void DtFrom_VisibleMonthChanged(object sender, MonthChangedEventArgs e)
        {
            InitChart();
        }

        void DtFrom_SelectionChanged(object sender, EventArgs e)
        {
            InitChart();
        }

        void BtnClearFilters_Click(object sender, EventArgs e)
        {
            WebUserControl11.ClearFilters();
            InitChart();
        }

        void BtnApply_Click(object sender, EventArgs e)
        {
            InitChart();
        }

        protected void lnkFeatureUsageByFeature_Click(object sender, EventArgs e)
        {
            ChartType = ChartDisplayType.FeatureUsageByFeature;
            InitChart();
        }

        protected void lnkFeatureUsageByDay_Click(object sender, EventArgs e)
        {
            ChartType = ChartDisplayType.FeatureUsageByDay;
            InitChart();
        }

        protected void lnkSysProfileOS_Click(object sender, EventArgs e)
        {
            ChartType = ChartDisplayType.SysProfileOS;
            InitChart();
        }

        protected void lnkAppRunsWithVersion_Click(object sender, EventArgs e)
        {
            ChartType = ChartDisplayType.AppRunsWithVersion;
            InitChart();
        }

        protected void lnkAppRunsWithoutVersion_Click(object sender, EventArgs e)
        {
            ChartType = ChartDisplayType.AppRunsWithoutVersion;
            InitChart();
        }

        protected void lnkAppRunsByDay_Click(object sender, EventArgs e)
        {
            ChartType = ChartDisplayType.AppRunsByDay;
            InitChart();
        }

        protected void lnkSysProfileCLR_Click(object sender, EventArgs e)
        {
            ChartType = ChartDisplayType.SysProfileCLR;
            InitChart();
        }


        LicService.LicenseServiceClass svc = new LicService.LicenseServiceClass();
        DBWorker worker;
        DbConnection GetDBConnection()
        {
            string settingsFilePath = Path.Combine(install.basePath, WebUserControl11.CmbSettingFiles.SelectedValue);

            svc.Initialise(settingsFilePath);
           
            worker = svc.GetDBWorker(svc.ConnectionString);
            DbConnection conn = worker.GetConnection(svc.ConnectionString);
            return conn;
        }

        void InitChart()
        {
            using (DbConnection conn = GetDBConnection())
            {
                conn.Open();
                ChartDisplayType type = this.ChartType;
                if (type == ChartDisplayType.FeatureUsageByFeature)
                    InitChart_FeatureUsageByFeature(conn, worker);
                else if (type == ChartDisplayType.FeatureUsageByDay)
                    InitChart_FeatureUsageByDay(conn, worker);
                else if (type == ChartDisplayType.SysProfileOS)
                    InitChart_SysProfileOS(conn,worker);
                else if (type == ChartDisplayType.AppRunsWithVersion)
                    InitChart_AppRunsWithVersion(conn, worker);
                else if (type == ChartDisplayType.AppRunsWithoutVersion)
                    InitChart_AppRunsWithoutVersion(conn, worker);
                else if (type == ChartDisplayType.AppRunsByDay)
                    InitChart_AppRunsByDay(conn, worker);
                else if (type == ChartDisplayType.SysProfileCLR)
                    InitChart_SysProfileCLR(conn, worker);
            }
        }

        void InitChart_FeatureUsageByFeature(DbConnection conn, DBWorker worker)
        {

            DbCommand cmd = conn.CreateCommand();

            InitQueryForFeatureTable("select f.Name, count(f.Name) as NumSessions, avg(" + worker.GetDateDiffExpression("f.StartTime", "f.EndTime", "s") + ")  as Dur from " + svc.UsageReportFeatureTableName + " as f ", cmd, " group by f.Name order by count(f.Name) desc;");
            DataTable dt = LoadDataTableFromCommand(cmd);

            AddDurationColumnFromSeconds(dt, "Duration", 2);

            Chart1.DataSource = dt;
            Chart1.Series["Series1"].XValueMember = "Name";
            Chart1.Series["Series1"].YValueMembers = "NumSessions";
            Chart1.Series["Series1"].Label = "#VALY\n(#PERCENT)";
            SetAxisProperties(Chart1.ChartAreas[0].AxisX, "Feature Usage Count");

            ChartArea ca = Chart1.ChartAreas.Add("duration");

            Series s = Chart1.Series.Add("duration");
            s.ChartArea = "duration";
            s.XValueMember = "name";
            s.YValueMembers = "Duration";
            s.YValueType = ChartValueType.DateTime;
            s.IsValueShownAsLabel = true;
            s.LabelFormat = "HH:mm:ss";

            ca.AxisY.Minimum = 0;
            ca.AxisY.LabelStyle.Format = "HH:mm:ss";
            ca.AxisY.LabelStyle.Enabled = true;

            SetAxisProperties(ca.AxisX, "Avg Duration");

            AlignChartAreaTo(ca, Chart1.ChartAreas[0].Name);

            Chart1.DataBind();

        }

        void InitChart_FeatureUsageByDay(DbConnection conn,DBWorker worker )
        {
            DbCommand cmd = conn.CreateCommand();

            string dateFormatStr = worker.GetConvertDateToStringExpression("f.StartTime", worker.GetDateFormatString_yyyy_mm_dd());
            InitQueryForFeatureTable("select " + dateFormatStr + " as RunDate, count(f.Name) as NumSessions, avg(" + worker.GetDateDiffExpression("f.StartTime", "f.EndTime", "s") + ") as Dur from " + svc.UsageReportFeatureTableName + " as f ", cmd, " group by " + dateFormatStr + " order by " + dateFormatStr + ";");

            DataTable dt = LoadDataTableFromCommand(cmd);

            AddDurationColumnFromSeconds(dt, "Duration", 2);

            Chart1.DataSource = dt;
            Chart1.Series["Series1"].XValueMember = "RunDate";
            Chart1.Series["Series1"].YValueMembers = "NumSessions";
            Chart1.Series["Series1"].Label = "#VALY\n(#PERCENT)";
            Chart1.Series["Series1"].ChartType = SeriesChartType.Line;

            SetAxisProperties(Chart1.ChartAreas[0].AxisX, "Feature Usage Count By Day");

            ChartArea ca = Chart1.ChartAreas.Add("duration");

            Series s = Chart1.Series.Add("duration");
            s.ChartArea = "duration";
            s.XValueMember = "RunDate";
            s.YValueMembers = "Duration";
            s.YValueType = ChartValueType.DateTime;
            s.IsValueShownAsLabel = true;
            s.LabelFormat = "HH:mm";


            ca.AxisY.Minimum = 0;
            ca.AxisY.LabelStyle.Format = "HH:mm";
            ca.AxisY.LabelStyle.Enabled = true;

            SetAxisProperties(ca.AxisX, "Avg Duration");

            AlignChartAreaTo(ca, Chart1.ChartAreas[0].Name);

            Chart1.DataBind();
        }

        void InitChart_AppRunsByDay(DbConnection conn, DBWorker worker)
        {
            DbCommand cmd = conn.CreateCommand();

            string dateFormatStr = worker.GetConvertDateToStringExpression("StartTime", worker.GetDateFormatString_yyyy_mm_dd());

            InitQueryForUsageReportTable("select " + dateFormatStr + " as RunDate, count(s.ID) as NumSessions, avg(" + worker.GetDateDiffExpression("StartTime", "EndTime", "s") + ") as Dur from " + svc.UsageReportTableName + " as s ", cmd, " group by " + dateFormatStr);

            DataTable dt = LoadDataTableFromCommand(cmd);

            AddDurationColumnFromSeconds(dt, "Duration", 2);

            Chart1.DataSource = dt;
            Chart1.Series["Series1"].XValueMember = "RunDate";
            Chart1.Series["Series1"].YValueMembers = "NumSessions";
            Chart1.Series["Series1"].Label = "#VALY\n(#PERCENT)";
            Chart1.Series["Series1"].ChartType = SeriesChartType.Line;

            SetAxisProperties(Chart1.ChartAreas[0].AxisX, "Number Of Runs");

            ChartArea ca = Chart1.ChartAreas.Add("duration");

            Series s = Chart1.Series.Add("duration");
            s.ChartArea = "duration";
            s.XValueMember = "RunDate";
            s.YValueMembers = "Duration";
            s.YValueType = ChartValueType.DateTime;
            s.IsValueShownAsLabel = true;
            s.LabelFormat = "HH:mm";


            ca.AxisY.Minimum = 0;
            ca.AxisY.LabelStyle.Format = "HH:mm";
            ca.AxisY.LabelStyle.Enabled = true;

            SetAxisProperties(ca.AxisX, "Avg Duration");

            AlignChartAreaTo(ca, Chart1.ChartAreas[0].Name);

            Chart1.DataBind();

        }

        void InitChart_SysProfileCLR(DbConnection conn, DBWorker worker)
        {
            DbCommand cmd = conn.CreateCommand();

            string floatToStrExpr = worker.GetConvertFloatToStringExpression("s.CLRVersion",5,1);
            InitQueryForUsageReportTable("select '.Net ' + " + floatToStrExpr + " as CLR, count(s.ID) as NumSessions, avg(" + worker.GetDateDiffExpression("s.StartTime", "s.EndTime", "s") + ")  as Dur  from " + svc.UsageReportTableName + " as s ", cmd, " group by s.CLRVersion order by count(s.ID) desc;");

            DataTable dt = LoadDataTableFromCommand(cmd);

            AddDurationColumnFromSeconds(dt, "Duration", 2);

            Chart1.DataSource = dt;
            Chart1.Series["Series1"].XValueMember = "CLR";
            Chart1.Series["Series1"].YValueMembers = "NumSessions";
            Chart1.Series["Series1"].Label = "#VALY\n(#PERCENT)";
            SetAxisProperties(Chart1.ChartAreas[0].AxisX, "Runs Per .Net Runtime");

            ChartArea ca = Chart1.ChartAreas.Add("duration");

            Series s = Chart1.Series.Add("duration");
            s.ChartArea = "duration";
            s.XValueMember = "CLR";
            s.YValueMembers = "Duration";
            s.YValueType = ChartValueType.DateTime; 
            s.IsValueShownAsLabel = true;
            s.LabelFormat = "HH:mm";


            ca.AxisY.Minimum = 0;
            ca.AxisY.LabelStyle.Format = "HH:mm";
            ca.AxisY.LabelStyle.Enabled = true;

            SetAxisProperties(ca.AxisX, "Avg Duration");

            AlignChartAreaTo(ca, Chart1.ChartAreas[0].Name);

            Chart1.DataBind();

        }

        void InitChart_SysProfileOS(DbConnection conn,DBWorker worker)
        {
            DbCommand cmd = conn.CreateCommand();

            InitQueryForUsageReportTable("select s.OS, count(s.ID) as NumSessions, avg(" + worker.GetDateDiffExpression("s.StartTime", "s.EndTime", "s") + ")  as Dur  from " + svc.UsageReportTableName + " as s ", cmd, " group by s.OS order by count(s.ID) DESC;");

            DataTable dt = LoadDataTableFromCommand(cmd);

            AddDurationColumnFromSeconds(dt, "Duration", 2);

            Chart1.DataSource = dt;
            Chart1.Series["Series1"].XValueMember = "OS";
            Chart1.Series["Series1"].YValueMembers = "NumSessions";
            Chart1.Series["Series1"].Label = "#VALY\n(#PERCENT)";
            SetAxisProperties(Chart1.ChartAreas[0].AxisX, "Runs per OS");

            ChartArea ca = Chart1.ChartAreas.Add("duration");

            Series s = Chart1.Series.Add("duration");
            s.ChartArea = "duration";
            s.XValueMember = "OS";
            s.YValueMembers = "Duration";
            s.YValueType = ChartValueType.DateTime;
            s.IsValueShownAsLabel = true;
            s.LabelFormat = "HH:mm";


            ca.AxisY.Minimum = 0;
            ca.AxisY.LabelStyle.Format = "HH:mm";
            ca.AxisY.LabelStyle.Enabled = true;

            SetAxisProperties(ca.AxisX, "Avg Duration");

            AlignChartAreaTo(ca, Chart1.ChartAreas[0].Name);

            Chart1.DataBind();

        }

        void InitChart_AppRunsWithVersion(DbConnection conn, DBWorker worker)
        {
            DbCommand cmd = conn.CreateCommand();

            string floatToStrExpr = worker.GetConvertFloatToStringExpression("s.AppVersion", -1, 1);

            InitQueryForUsageReportTable("select s.AppName+' v'+" + floatToStrExpr + " as App, count(s.ID) as NumSessions, avg(" + worker.GetDateDiffExpression("s.StartTime", "s.EndTime", "s") + ") as Dur from " + svc.UsageReportTableName + " as s ", cmd, "group by s.AppName,s.AppVersion order by count(s.ID) DESC");

            DataTable dt =  LoadDataTableFromCommand(cmd);

            AddDurationColumnFromSeconds(dt, "Duration",2);

            Chart1.DataSource = dt;
            Chart1.Series["Series1"].XValueMember = "App";
            Chart1.Series["Series1"].YValueMembers = "NumSessions";
            Chart1.Series["Series1"].Label = "#VALY\n(#PERCENT)";
            SetAxisProperties(Chart1.ChartAreas[0].AxisX, "Number Of Runs");

            ChartArea ca = Chart1.ChartAreas.Add("duration");

            Series s = Chart1.Series.Add("duration");
            s.ChartArea = "duration";
            s.XValueMember = "App";
            s.YValueMembers = "Duration";
            s.YValueType = ChartValueType.DateTime;
            s.IsValueShownAsLabel = true;
            s.LabelFormat = "HH:mm";


            ca.AxisY.Minimum = 0;
            ca.AxisY.LabelStyle.Format = "HH:mm";
            ca.AxisY.LabelStyle.Enabled = true;

            SetAxisProperties(ca.AxisX,"Avg Duration");

            AlignChartAreaTo(ca,Chart1.ChartAreas[0].Name);

            Chart1.DataBind();
            
        }

        void InitChart_AppRunsWithoutVersion(DbConnection conn, DBWorker worker)
        {
            DbCommand cmd = conn.CreateCommand();

            InitQueryForUsageReportTable("select s.AppName, count(s.ID) as NumSessions, avg(" + worker.GetDateDiffExpression("s.StartTime", "s.EndTime", "s") + ") as Dur from " + svc.UsageReportTableName + " as s ", cmd, "group by s.AppName order by count(s.ID) DESC");

            DataTable dt = LoadDataTableFromCommand(cmd);

            AddDurationColumnFromSeconds(dt, "Duration", 2);

            Chart1.DataSource = dt;
            Chart1.Series["Series1"].XValueMember = "AppName";
            Chart1.Series["Series1"].YValueMembers = "NumSessions";
            Chart1.Series["Series1"].Label = "#VALY\n(#PERCENT)";
            SetAxisProperties(Chart1.ChartAreas[0].AxisX, "Number Of Runs");

            ChartArea ca = Chart1.ChartAreas.Add("duration");

            Series s = Chart1.Series.Add("duration");
            s.ChartArea = "duration";
            s.XValueMember = "AppName";
            s.YValueMembers = "Duration";
            s.YValueType = ChartValueType.DateTime;
            s.IsValueShownAsLabel = true;
            s.LabelFormat = "HH:mm";

            ca.AxisY.Minimum = 0;
            ca.AxisY.LabelStyle.Format = "HH:mm";
            ca.AxisY.LabelStyle.Enabled = true;

            SetAxisProperties(ca.AxisX, "Avg Duration");

            AlignChartAreaTo(ca, Chart1.ChartAreas[0].Name);

            Chart1.DataBind();

        }

        void AddDurationColumnFromSeconds(DataTable dt, string newColumnName, int secondsColumnIndex)
        {
            DataColumn dc = dt.Columns.Add(newColumnName, typeof(DateTime));
            foreach (DataRow dr in dt.Rows)
            {
                int seconds = Convert.ToInt32(dr.ItemArray[secondsColumnIndex]);
                dr[dc] = new DateTime(TimeSpan.FromSeconds(seconds).Ticks);
            }
        }

        DataTable LoadDataTableFromCommand(DbCommand cmd)
        {
            DbDataReader rdr = cmd.ExecuteReader();

            DataTable dt = new DataTable();
            dt.Load(rdr);

            rdr.Close();
            return dt;

        }

        void SetAxisProperties(Axis a, string title)
        {
            a.MajorGrid.Enabled = false;
            a.Title = title;
            a.TitleFont = new Font("Arial", 12, FontStyle.Bold);
            a.Interval = 1;
            a.IsLabelAutoFit = true;
            a.LabelAutoFitStyle = LabelAutoFitStyles.DecreaseFont | LabelAutoFitStyles.IncreaseFont | LabelAutoFitStyles.LabelsAngleStep30
                | LabelAutoFitStyles.LabelsAngleStep45 | LabelAutoFitStyles.LabelsAngleStep90 | LabelAutoFitStyles.StaggeredLabels | LabelAutoFitStyles.WordWrap;
        }

        void AlignChartAreaTo(ChartArea ca, string toArea)
        {
            ca.AlignmentStyle = AreaAlignmentStyles.All;
            ca.AlignmentOrientation = AreaAlignmentOrientations.Vertical;
            ca.AlignWithChartArea = toArea;
        }

        void InitQueryForUsageReportTable(string prefix, DbCommand cmd, string suffix)
        {
            List<QueryParameter> queryParams = new List<QueryParameter>();

            string query = prefix;

            if (WebUserControl11.HasConstraints)
            {
                query += " where ";
                if (WebUserControl11.HasUserNameConstraint)
                {
                    query += " s.UserID = ? and";
                    queryParams.Add(new QueryParameter("userid", WebUserControl11.TxtUserName.Text));
                }

                if (WebUserControl11.HasOSNameConstraint)
                {
                    query += " s.OS = ? and";
                    queryParams.Add(new QueryParameter("os", WebUserControl11.TxtOSName.Text));
                }

                if (WebUserControl11.HasMemoryConstraint)
                {
                    query += " s.SystemMemory " + WebUserControl11.CmbMmeoryOperator.SelectedValue + " ? and";
                    queryParams.Add(new QueryParameter("memory", WebUserControl11.TxtMemoryValue.Text));
                }

                if (WebUserControl11.HasAppNameConstraint)
                {
                    query += " s.AppName = ? and";
                    queryParams.Add(new QueryParameter("appname", WebUserControl11.TxtAppName.Text));
                }

                if (WebUserControl11.HasAppVersionConstraint)
                {
                    query += " s.AppVersion " + WebUserControl11.CmbAppVersionOperator.SelectedValue + " ? and";
                    queryParams.Add(new QueryParameter("appversion", float.Parse(WebUserControl11.TxtAppVersion.Text)));
                }

                if (WebUserControl11.HasDateFromConstraint)
                {
                    query += " s.StartTime >= ? and";
                    queryParams.Add(new QueryParameter("starttime", WebUserControl11.DtFrom.SelectedDate));
                }

                if (WebUserControl11.HasDateToConstraint)
                {
                    query += " s.EndTime < ? and";
                    queryParams.Add(new QueryParameter("endtime", WebUserControl11.DtTo.SelectedDate.AddDays(1)));
                }

                if (WebUserControl11.HasLicenseIDConstraint)
                {
                    query += " s.AssociatedLicenseID = ? and";
                    queryParams.Add(new QueryParameter("licenseid", WebUserControl11.TxtLicenseID.Text));
                }

                if (WebUserControl11.HasRuntimeConstraint)
                {
                    query += " s.CLRVersion " + WebUserControl11.CmbRuntime.SelectedValue + " ? and";
                    queryParams.Add(new QueryParameter("clrversion", float.Parse(WebUserControl11.TxtRuntime.Text)));
                }

                if (WebUserControl11.HasFeatureTableConstraints)
                {
                    query += " s.ID in (select f.ReportID from " + svc.UsageReportFeatureTableName + " as f where f.ReportID=s.ID and ";
                    if (WebUserControl11.HasFeatureNameConstraint)
                    {
                        query += "f.Name = ? and";
                        queryParams.Add(new QueryParameter("featurename", WebUserControl11.TxtFeatureName.Text));
                    }

                    if (query.EndsWith("and"))
                        query = query.Remove(query.Length - 3);

                    query += ")";
                }

                if (query.EndsWith("and"))
                    query = query.Remove(query.Length - 3);
            }

            query += suffix;

            cmd.CommandText = query;

            foreach (QueryParameter qp in queryParams)
            {
                worker.AddInputParameter(cmd, qp.name, qp.value);
            }

        }

        void InitQueryForFeatureTable(string prefix, DbCommand cmd, string suffix)
        {
            List<QueryParameter> queryParams = new List<QueryParameter>();

            string query = prefix;

            if (WebUserControl11.HasConstraints)
            {
                query += " where ";

                if (WebUserControl11.HasFeatureNameConstraint)
                {
                    query += "f.Name = ? and";
                    queryParams.Add(new QueryParameter("featurename", WebUserControl11.TxtFeatureName.Text));
                }

                if (WebUserControl11.HasUsageReportTableConstraints)
                {
                    query += " f.ReportID in (select s.ID from " + svc.UsageReportTableName + " as s where f.ReportID=s.ID and";

                    if (WebUserControl11.HasUserNameConstraint)
                    {
                        query += " s.UserID = ? and";
                        queryParams.Add(new QueryParameter("userid", WebUserControl11.TxtUserName.Text));
                    }

                    if (WebUserControl11.HasOSNameConstraint)
                    {
                        query += " s.OS = ? and";
                        queryParams.Add(new QueryParameter("os", WebUserControl11.TxtOSName.Text));
                    }

                    if (WebUserControl11.HasMemoryConstraint)
                    {
                        query += " s.SystemMemory " + WebUserControl11.CmbMmeoryOperator.SelectedValue + " ? and";
                        queryParams.Add(new QueryParameter("memory", WebUserControl11.TxtMemoryValue.Text));
                    }

                    if (WebUserControl11.HasAppNameConstraint)
                    {
                        query += " s.AppName = ? and";
                        queryParams.Add(new QueryParameter("appname", WebUserControl11.TxtAppName.Text));
                    }

                    if (WebUserControl11.HasAppVersionConstraint)
                    {
                        query += " s.AppVersion " + WebUserControl11.CmbAppVersionOperator.SelectedValue + " ? and";
                        queryParams.Add(new QueryParameter("appversion", float.Parse(WebUserControl11.TxtAppVersion.Text)));
                    }

                    if (WebUserControl11.HasDateFromConstraint)
                    {
                        query += " s.StartTime >= ? and";
                        queryParams.Add(new QueryParameter("starttime", WebUserControl11.DtFrom.SelectedDate));
                    }

                    if (WebUserControl11.HasDateToConstraint)
                    {
                        query += " s.EndTime < ? and";
                        queryParams.Add(new QueryParameter("endtime", WebUserControl11.DtTo.SelectedDate.AddDays(1)));
                    }

                    if (WebUserControl11.HasLicenseIDConstraint)
                    {
                        query += " s.AssociatedLicenseID = ? and";
                        queryParams.Add(new QueryParameter("licenseid", WebUserControl11.TxtLicenseID.Text));
                    }

                    if (WebUserControl11.HasRuntimeConstraint)
                    {
                        query += " s.CLRVersion " + WebUserControl11.CmbRuntime.SelectedValue + " ? and";
                        queryParams.Add(new QueryParameter("clrversion", float.Parse(WebUserControl11.TxtRuntime.Text)));
                    }

                    if (query.EndsWith("and"))
                        query = query.Remove(query.Length - 3);

                    query += ")";
                }

                if (query.EndsWith("and"))
                    query = query.Remove(query.Length - 3);
            }

            query += suffix;

            cmd.CommandText = query;

            foreach (QueryParameter qp in queryParams)
            {
                worker.AddInputParameter(cmd, qp.name, qp.value);
            }
        }

    }
}