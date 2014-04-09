using System.Web.UI.WebControls;
using System.Data.SqlClient;
using System;
using System.Configuration;
using System.Drawing;
using System.Data;
using System.Collections.Generic;

public partial class MyCalender : System.Web.UI.Page
{
    private string sConnectionString;
    private List<DateTime> oDatelistfromdb;
    private List<bool?> oIsactivelistfromdb;
    private bool? bIsactiveindb;
    private bool oDbhasrows = false;
    private EventArgs oEvent = null;
    private bool bSelectionchanged = false;
    private bool bVisiblemonthchanged = false;
    private object postbacksender = null;
    private DateTime odefaultdate = new DateTime(0001, 01, 01);
    private int iterate = 0;

    public MyCalender()
    {
        this.sConnectionString = ConfigurationManager.ConnectionStrings["CalenderConnection"].ConnectionString;
        
    }

    protected string InsertStatement(DateTime oDate)
    {
        string sSql = "INSERT into t_calender(calenderdate, isactive)" + "VALUES('" + oDate.ToString("yyyy-MM-dd") + " '," + "'" + true + "')";
        return sSql;
    }

    protected string UpdateStatement(DateTime oDate, string isactive)
    {
        string sSql = "UPDATE t_calender SET isactive =" + isactive + " WHERE calenderdate = '" + oDate.ToString("yyyy-MM-dd") + "'";
        return sSql;
    }

    protected bool IsRecordExists(DateTime oDate)
    {
        string sSelect = "select CONVERT(date, calenderdate, 102), isactive from t_calender where calenderdate= '" + oDate.ToString("yyyy-MM-dd") + "'";

        using (SqlConnection oCon = new SqlConnection())
        {
            oCon.ConnectionString = ConfigurationManager.ConnectionStrings["CalenderConnection"].ConnectionString;
            oCon.Open();
            SqlCommand cmd = new SqlCommand(sSelect, oCon);
            SqlDataReader dr = cmd.ExecuteReader();

            while (dr.Read())
            {
                if (dr[1] is DBNull)
                    bIsactiveindb = null;
                else
                    bIsactiveindb = (bool)dr[1];
                return true;
            }
        }
        return false;
    }

    protected bool Loadcollectionlist()
    {
        this.oDatelistfromdb = new List<DateTime>();
        this.oIsactivelistfromdb = new List<bool?>();

        using (SqlConnection oCon = new SqlConnection(this.sConnectionString))
        {
            using (SqlCommand cmd = new SqlCommand("select CONVERT(date, calenderdate, 102), isactive from t_calender order by calenderdate asc", oCon))
            {
                SqlDataAdapter oDataAdapter = new SqlDataAdapter(cmd);
                DataTable oDataTable = new DataTable();
                oCon.Open();
                oDataAdapter.Fill(oDataTable);
                if (oDataTable != null && oDataTable.Rows.Count > 0)
                {
                    foreach (DataRow oRow in oDataTable.Rows)
                    {
                        this.oDatelistfromdb.Add((DateTime)oRow[0]);

                        if (oRow[1] is DBNull)
                            this.oIsactivelistfromdb.Add(null);
                        else
                            this.oIsactivelistfromdb.Add((bool)oRow[1]);
                    }
                }
                return oDbhasrows = true;
            }
        }
    }

    protected void Ispostback()
    {
        if (!IsPostBack && iterate == 1 && Calendar1.SelectedDate == odefaultdate)
            this.Loadcollectionlist();

        else if (bSelectionchanged == false && this.bVisiblemonthchanged == false && IsPostBack && ViewState["PostID"].ToString() == Session["PostID"].ToString())
            Calendar1_SelectionChanged(postbacksender, oEvent);
    }

    protected void Calendar1_DayRender(object sender, DayRenderEventArgs e)
    {
        if (e.Day.Date > DateTime.Now)
            e.Day.IsSelectable = false;

        iterate++;

        this.Ispostback();

        if (oDatelistfromdb == null)
            Loadcollectionlist();
        
        int iIndexofcollection = oDatelistfromdb.IndexOf(e.Day.Date);

        if (e.Day.IsOtherMonth)
        {
            e.Cell.Controls.Clear();
            e.Cell.BackColor = Color.White;
            e.Cell.ForeColor = Color.White;
        }

        else if (oDbhasrows && iIndexofcollection >= 0)
        {
            if (e.Day.Date == oDatelistfromdb[iIndexofcollection] && oIsactivelistfromdb[iIndexofcollection] == true)
            {
                e.Cell.BackColor = Color.FromArgb(156, 219, 168);
                e.Cell.ForeColor = Color.White;
                if (iIndexofcollection < oDatelistfromdb.Count - 1)
                {
                    iIndexofcollection++;
                }
            }
            else if (e.Day.Date == oDatelistfromdb[iIndexofcollection] && oIsactivelistfromdb[iIndexofcollection] == false)
            {
                e.Cell.BackColor = Color.FromArgb(245, 151, 151);
                e.Cell.ForeColor = Color.White;
                if (iIndexofcollection < oDatelistfromdb.Count - 1)
                {
                    iIndexofcollection++;
                }
            }
            else if (e.Day.Date == oDatelistfromdb[iIndexofcollection] && oIsactivelistfromdb[iIndexofcollection] == null)
            {
                e.Cell.BackColor = Color.White;
                e.Cell.ForeColor = Color.Black;
                if (iIndexofcollection < oDatelistfromdb.Count - 1)
                {
                    iIndexofcollection++;
                }
            }
        }
    }

    protected void Calendar1_SelectionChanged(object sender, EventArgs e)
    {
        if (Calendar1.SelectedDate != this.odefaultdate)
        {
            using (SqlConnection oCon = new SqlConnection())
            {
                oCon.ConnectionString = this.sConnectionString;
                oCon.Open();
                using (SqlCommand oCommand = new SqlCommand(InsertStatement(Calendar1.SelectedDate), oCon))
                {
                    bool bRecordexists = IsRecordExists(Calendar1.SelectedDate);
                    // New record
                    if (!bRecordexists)
                    {
                        oCommand.ExecuteNonQuery();
                    }
                    //Existing record so need to update.
                    else if (bRecordexists)
                    {
                        if (bIsactiveindb == true)
                        {
                            oCommand.CommandText = UpdateStatement(Calendar1.SelectedDate, "0");
                            oCommand.ExecuteNonQuery();
                        }
                        else if (bIsactiveindb == false)
                        {
                            oCommand.CommandText = UpdateStatement(Calendar1.SelectedDate, "null");
                            oCommand.ExecuteNonQuery();
                        }
                        else if (bIsactiveindb == null)
                        {
                            oCommand.CommandText = UpdateStatement(Calendar1.SelectedDate, "1");
                            oCommand.ExecuteNonQuery();
                        }
                    }
                    bSelectionchanged = true;
                    this.Loadcollectionlist();
                }
            }
        }
    }

    protected void Calendar1_VisibleMonthChanged(object sender, MonthChangedEventArgs e)
    {
        this.Loadcollectionlist();
        this.bVisiblemonthchanged = true;
    }

    protected void Calendar1_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            Session["PostID"] = "1";
            ViewState["PostID"] = Session["PostID"].ToString();
        }
        else
        { 
            // Triggers when postback
            if (ViewState["PostID"].ToString() == Session["PostID"].ToString())
            {
                Session["PostID"] = (Convert.ToInt16(Session["PostID"]) + 1).ToString();
                ViewState["PostID"] = Session["PostID"].ToString();
            }
            else
            {
                // Trigger when Refresh button is clicked
                ViewState["PostID"] = Session["PostID"].ToString();
            }
        }
    }
}