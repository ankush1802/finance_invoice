using KF.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using KF.Utilities.Common;

namespace KF.Web
{
    public partial class UpdateClassification : System.Web.UI.Page
    {
        #region Properties
        public KF.Entity.MasterClassification StypeDetails
        {
            get;
            set;
        }
        public KF.Entity.MasterClassification TtypeDetails
        {
            get;
            set;
        }
        public KF.Entity.MasterClassification GtypeDetails
        {
            get;
            set;
        }
        public KF.Entity.MasterClassification ZtypeDetails
        {
            get;
            set;
        }

        public Int32 classificationId
        {
            get { return Convert.ToInt32(ViewState["classificationId"]); }
            set { ViewState["classificationId"] = value; }
        }

        public Boolean IsClassificationUsed
        {
            get { return Convert.ToBoolean(ViewState["IsClassificationUsed"]); }
            set { ViewState["IsClassificationUsed"] = value; }
        }

        public Int32 UserID
        {
            get { return Convert.ToInt32(ViewState["UserID"]); }
            set { ViewState["UserID"] = value; }
        }
        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (!string.IsNullOrEmpty(Request.QueryString["args"]))
                {
                    Dictionary<String, String> decryptedQueryString = new Dictionary<String, String>();
                    decryptedQueryString.ToDecryptedQueryString(Request.QueryString["args"].ToString());

                    if (decryptedQueryString.Where(s => s.Key == "classificationId").Any())
                    {
                        var statementData = decryptedQueryString.Where(s => s.Key == "classificationId").FirstOrDefault();
                        classificationId = Convert.ToInt32(statementData.Value);

                        //Get Classification Data
                        BindClassificationDetails();
                    }
                }
                else
                {
                    Response.Redirect("/Kippin/Finance/Accounting/MannualClassification");
                }
            }
        }

        #region A-Type Classification
        public void BindAtypeData()
        {
            using (var context = new KFentities())
            {
                StypeDetails = context.MasterClassifications.Where(d => d.ReportingTotal.Equals(ddlAtypeClassificationSubTotalList.SelectedItem.Text.ToString())).FirstOrDefault();
            }
            txtAtypeClassificationRangeAcct.Text = StypeDetails.ReportingTotalRange;
            txtAtypeClassificationNewAccount.Text = StypeDetails.NewClassificationVerb + "-";
            hdnAtypePrefix.Value = StypeDetails.NewClassificationVerb + "-";
        }
        public void BindAtypeSubTotalList()
        {
            using (var context = new KFentities())
            {
                var ObjList = context.MasterClassifications.Where(d => d.RepotingTotalType == "S" && d.UserId == null).ToList();
                ObjList.AddRange(context.MasterClassifications.Where(d => d.RepotingTotalType == "S" && d.UserId == UserID).ToList());
                ddlAtypeClassificationSubTotalList.DataSource = ObjList;
                ddlAtypeClassificationSubTotalList.DataBind();
                ddlAtypeClassificationSubTotalList.DataTextField = "ReportingTotal";
                ddlAtypeClassificationSubTotalList.DataValueField = "Id";
                ddlAtypeClassificationSubTotalList.DataBind();
                ddlAtypeClassificationSubTotalList.Items.Insert(0, "Please Select");
            }
        }
        protected void btnAtypeClassification_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(txtAtypeClassificationName.Text))
            {
                AtypeErrorDiv.Visible = true;
                AtypeErrorMessage.InnerText = "Classification Name is required.";
                return;
            }
            else if (String.IsNullOrEmpty(txtAtypeClassificationSubAccountDesc.Text))
            {
                AtypeErrorDiv.Visible = true;
                AtypeErrorMessage.InnerText = "Classification description is required.";
                return;
            }
            else
            {
                using (var db = new KFentities())
                {
                    var updateClassification = db.Classifications.Where(f => f.Id == classificationId).FirstOrDefault();
                    updateClassification.ClassificationType = txtAtypeClassificationName.Text.Trim();
                    updateClassification.Desc = txtAtypeClassificationSubAccountDesc.Text.Trim();
                    if (!IsClassificationUsed)
                    {
                      var masterClassificationDetail = db.MasterClassifications.Where(f => f.ReportingTotal.Equals(ddlAtypeClassificationSubTotalList.SelectedItem.Text)).FirstOrDefault();
                      String SubTotalDisplayNumber = masterClassificationDetail.SubTotalDisplayNumber;
                      Int32 SubTotalNumber = Convert.ToInt32(masterClassificationDetail.SubTotalDisplayNumber.Replace("-", string.Empty));
                      updateClassification.ChartAccountDisplayNumber = txtAtypeClassificationNewAccount.Text;
                      updateClassification.ChartAccountNumber = Convert.ToInt32(txtAtypeClassificationNewAccount.Text.Replace("-", string.Empty));
                      updateClassification.ReportingSubTotalDisplayNumber = SubTotalDisplayNumber;
                      updateClassification.ReportingSubTotalNumber = SubTotalNumber;
                    }
                    db.SaveChanges();
                }
                
                AtypeErrorDiv.Visible = false;
                AtypeSuccess.Visible = true;

            }
        }

        protected void ddlAtypeClassificationSubTotalList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ddlAtypeClassificationSubTotalList.SelectedIndex > 0)
            {
                BindAtypeData();
                txtAtypeClassificationValidationAccNumber.Text = String.Empty;
                btnAtypeClassification.Enabled = false;
            }
        }

        protected void txtAtypeClassificationValidationAccNumber_TextChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtAtypeClassificationValidationAccNumber.Text))
            {
                //Check if inputted number is valid range number and also check in database 
                var range = txtAtypeClassificationRangeAcct.Text.Split('-');
                int StartRange = 0;
                int EndRange = 0;
                int SecondStartRange = 0;
                int SecondEndRange = 0;
                if (range.Length == 2)
                {
                    StartRange = Convert.ToInt32(range[0]);
                    EndRange = Convert.ToInt32(range[1]);
                    if (!CheckNumberRange(Convert.ToInt32(txtAtypeClassificationValidationAccNumber.Text), StartRange, EndRange))
                    {
                        AtypeErrorDiv.Visible = true;
                        AtypeErrorMessage.InnerText = "Invalid number. Please try another number.";
                        btnAtypeClassification.Enabled = false;
                        return;
                    }
                }
                else
                {
                    StartRange = Convert.ToInt32(range[0].Replace("(", string.Empty));
                    SecondEndRange = Convert.ToInt32(range[2].ToString().Replace(")", string.Empty));
                    var newRange = range[1].Replace(") To (", "-").Split('-');
                    EndRange = Convert.ToInt32(newRange[0]);
                    SecondStartRange = Convert.ToInt32(newRange[1]);
                    if (!CheckNumberRange(Convert.ToInt32(txtAtypeClassificationValidationAccNumber.Text), StartRange, EndRange))
                    {
                        if (!CheckNumberRange(Convert.ToInt32(txtAtypeClassificationValidationAccNumber.Text), SecondStartRange, SecondEndRange))
                        {
                            AtypeErrorDiv.Visible = true;
                            AtypeErrorMessage.InnerText = "Invalid number. Please try another number.";
                            return;
                        }

                        //AtypeErrorDiv.Visible = true;
                        //AtypeErrorMessage.InnerText = "Invalid number. Please try another number.";
                        //return;
                    }
                }



                string NewChartAccNumber = hdnAtypePrefix.Value + txtAtypeClassificationValidationAccNumber.Text;

                //Check in database
                using (var context = new KFentities())
                {
                    if (context.Classifications.Where(d => d.ChartAccountDisplayNumber.Equals(NewChartAccNumber) && (d.UserId == UserID || d.UserId == null)).Any()
                        && NewChartAccNumber != context.Classifications.Where(g=>g.Id==classificationId).Select(f=>f.ChartAccountDisplayNumber).FirstOrDefault()
                        )
                    {
                        AtypeErrorDiv.Visible = true;
                        AtypeErrorMessage.InnerText = "Classification already existed. Please try another number.";
                        btnAtypeClassification.Enabled = false;
                    }
                    else
                    {
                        AtypeErrorDiv.Visible = false;
                        btnAtypeClassification.Enabled = true;
                    }
                }
                txtAtypeClassificationNewAccount.Text = NewChartAccNumber;

            }
            else
            {
                btnAtypeClassification.Enabled = false;
                return;
            }
        }
        #endregion

        #region Common Methods

        private void BindClassificationDetails()
        {
            using (var db = new KFentities())
            {
                var classificationDetails = db.Classifications.Where(f => f.Id == classificationId).FirstOrDefault();
                UserID = Convert.ToInt32(classificationDetails.UserId);

                #region G-Type
                if (classificationDetails.Type.Equals("G"))
                {
                    hdnGtypePrefix.Value = classificationDetails.ChartAccountDisplayNumber.Substring(0, 5);
                    var masterClassificationDetail = db.MasterClassifications
                      .Where(d => d.TotalDisplayNumber.Equals(classificationDetails.ReportingTotalDisplayNumber)).FirstOrDefault();
                    GtypeClassificationSection.Visible = true;
                    IsClassificationUsed = db.BankExpenses.Where(d => d.ClassificationId == classificationDetails.Id && d.IsDeleted == false).Any();
                    BindGtypeSubTotalList();
                    txtGtypeClassificationName.Text = classificationDetails.ClassificationType;
                    txtGtypeClassificationDesc.Text = classificationDetails.Desc;
                    txtGtypeChartAccountNumber.Text = classificationDetails.ChartAccountDisplayNumber.Substring(5, 4);
                    txtGtypeChartAccountDisplayNumber.Text = classificationDetails.ChartAccountDisplayNumber;
                    ddlGtypeClassificationSubTotalList.SelectedValue = Convert.ToString(masterClassificationDetail.Id);
                    txtGtypeRange.Text = masterClassificationDetail.ReportingTotalRange;
                    if (IsClassificationUsed)
                    {
                        txtGtypeChartAccountNumber.Enabled = false;
                        //txtGtypeClassificationRangeAcct.Enabled = false;
                        ddlGtypeClassificationSubTotalList.Enabled = false;
                    }
                    btnGtypeClassification.Enabled = true;
                }
                #endregion

                #region A-Type
                if (classificationDetails.Type.Equals("A"))
                {
                    hdnAtypePrefix.Value = classificationDetails.ChartAccountDisplayNumber.Substring(0, 5);
                    var masterClassificationDetail = db.MasterClassifications
                        .Where(d => d.SubTotalDisplayNumber.Equals(classificationDetails.ReportingSubTotalDisplayNumber)).FirstOrDefault();
                    AtypeClassificationSection.Visible = true;
                    IsClassificationUsed = db.BankExpenses.Where(d => d.ClassificationId == classificationDetails.Id && d.IsDeleted == false).Any();
                    BindAtypeSubTotalList();
                    txtAtypeClassificationName.Text = classificationDetails.ClassificationType;
                    txtAtypeClassificationSubAccountDesc.Text = classificationDetails.Desc;
                    txtAtypeClassificationValidationAccNumber.Text = classificationDetails.ChartAccountDisplayNumber.Substring(5, 4);
                    txtAtypeClassificationNewAccount.Text = classificationDetails.ChartAccountDisplayNumber;
                    ddlAtypeClassificationSubTotalList.SelectedValue = Convert.ToString(masterClassificationDetail.Id);
                    txtAtypeClassificationRangeAcct.Text = masterClassificationDetail.ReportingTotalRange;
                    if (IsClassificationUsed)
                    {
                        txtAtypeClassificationValidationAccNumber.Enabled = false;
                        txtAtypeClassificationRangeAcct.Enabled = false;
                        ddlAtypeClassificationSubTotalList.Enabled = false;
                    }
                    btnAtypeClassification.Enabled = true;

                }
                #endregion
               
                if (classificationDetails.Type.Equals("S"))
                {
                    hdnStypePrefix.Value = classificationDetails.ChartAccountDisplayNumber.Substring(4, 5);
                    hdnSTypeMasterClassificationSubTotalDisplayNumber.Value = classificationDetails.ChartAccountDisplayNumber;
                    var masterClassificationDetail = db.MasterClassifications
                      .Where(d => d.TotalDisplayNumber.Equals(classificationDetails.ReportingTotalDisplayNumber)).FirstOrDefault();
                    StypeClassificationSection.Visible = true;
                    IsClassificationUsed = db.BankExpenses.Where(d => d.Classification.ReportingSubTotalDisplayNumber == classificationDetails.ChartAccountDisplayNumber && d.UserId==UserID && d.IsDeleted == false).Any();
                    BindStypeSubTotalList();
                    txtStypeClassificationName.Text = classificationDetails.ClassificationType;
                    txtStypeClassificationDesc.Text = classificationDetails.Desc;
                    txtStypeChartAccountNumber.Text = classificationDetails.ChartAccountDisplayNumber.Substring(0, 4);
                    txtStypeChartAccountRange.Text = classificationDetails.ChartAccountDisplayNumber;
                    ddlStypeClassificationSubTotalList.SelectedValue = Convert.ToString(masterClassificationDetail.Id);
                    txtStypeRange.Text = masterClassificationDetail.ReportingTotalRange;
                    if (IsClassificationUsed)
                    {
                        txtStypeChartAccountNumber.Enabled = false;
                        ddlStypeClassificationSubTotalList.Enabled = false;
                    }
                    btnStypeClassification.Enabled = true;
                }
                
            }
        }

        public bool CheckNumberRange(int chartAccNo, int startRange, int EndRange)
        {
            if ((chartAccNo >= startRange) && (chartAccNo <= EndRange))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        #endregion

        #region G-Type
        #region G-Type Methods
        public void BindGtypeData()
        {
            using (var context = new KFentities())
            {
                GtypeDetails = context.MasterClassifications.Where(d => d.ReportingTotal.Equals(ddlGtypeClassificationSubTotalList.SelectedItem.Text.ToString())).FirstOrDefault();
            }

            txtGtypeRange.Text = GtypeDetails.ReportingTotalRange;
            txtGtypeChartAccountDisplayNumber.Text = GtypeDetails.NewClassificationVerb + "-";
            hdnGtypePrefix.Value = GtypeDetails.NewClassificationVerb + "-";
        }
        public void BindGtypeSubTotalList()
        {
            using (var context = new KFentities())
            {
                var ObjList = context.MasterClassifications.Where(d => d.RepotingTotalType == "G").ToList();
                ddlGtypeClassificationSubTotalList.DataSource = ObjList;
                ddlGtypeClassificationSubTotalList.DataBind();
                ddlGtypeClassificationSubTotalList.DataTextField = "ReportingTotal";
                ddlGtypeClassificationSubTotalList.DataValueField = "Id";
                ddlGtypeClassificationSubTotalList.DataBind();
                ddlGtypeClassificationSubTotalList.Items.Insert(0, "Please Select");
            }
        }

        #endregion

        protected void txtGtypeChartAccountNumber_TextChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtGtypeChartAccountNumber.Text))
            {
                //Check if inputted number is valid range number and also check in database 
                var range = txtGtypeRange.Text.Split('-');
                int StartRange = 0;
                int EndRange = 0;
                StartRange = Convert.ToInt32(range[0]);
                EndRange = Convert.ToInt32(range[1]);
                if (!CheckNumberRange(Convert.ToInt32(txtGtypeChartAccountNumber.Text), StartRange, EndRange))
                {
                    GtypeErrorDiv.Visible = true;
                    GtypeErrorMessage.InnerText = "Invalid number. Please try another number.";
                    btnGtypeClassification.Enabled = false;
                    return;
                }
                string NewChartAccNumber = hdnGtypePrefix.Value + txtGtypeChartAccountNumber.Text;

                //Check in database
                using (var context = new KFentities())
                {
                    if (context.Classifications.Where(d => d.ChartAccountDisplayNumber.Equals(NewChartAccNumber) && (d.UserId == UserID || d.UserId == null)).Any()
                        && NewChartAccNumber != context.Classifications.Where(g => g.Id == classificationId).Select(f => f.ChartAccountDisplayNumber).FirstOrDefault()
                        )
                    {
                        GtypeErrorDiv.Visible = true;
                        GtypeErrorMessage.InnerText = "Classification already existed. Please try another number.";
                        btnGtypeClassification.Enabled = false;
                    }
                    else
                    {
                        GtypeErrorDiv.Visible = false;
                        btnGtypeClassification.Enabled = true;
                    }
                }
                txtGtypeChartAccountDisplayNumber.Text = NewChartAccNumber;
            }

            else
            {
                btnGtypeClassification.Enabled = false;
                return;
            }
        }

        protected void ddlGtypeClassificationSubTotalList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ddlGtypeClassificationSubTotalList.SelectedIndex > 0)
            {
                BindGtypeData();
                txtGtypeChartAccountNumber.Text = string.Empty;
                btnGtypeClassification.Enabled = false;
            }
        }

        protected void btnGtypeClassification_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(txtGtypeClassificationName.Text))
            {
                GtypeErrorDiv.Visible = true;
                GtypeErrorMessage.InnerText = "Classification Name is required.";
                return;
            }
            else if (String.IsNullOrEmpty(txtGtypeClassificationDesc.Text))
            {
                GtypeErrorDiv.Visible = true;
                GtypeErrorMessage.InnerText = "Classification description is required.";
                return;
            }
            else
            {
                using (var db = new KFentities())
                {
                    var updateClassification = db.Classifications.Where(f => f.Id == classificationId).FirstOrDefault();
                    updateClassification.ClassificationType = txtGtypeClassificationName.Text.Trim();
                    updateClassification.Desc = txtGtypeClassificationDesc.Text.Trim();
                    if (!IsClassificationUsed)
                    {
                        var masterClassificationDetail = db.MasterClassifications.Where(f => f.ReportingTotal.Equals(ddlGtypeClassificationSubTotalList.SelectedItem.Text)).FirstOrDefault();
                        String TotalDisplayNumber = masterClassificationDetail.TotalDisplayNumber;
                        Int32 TotalNumber = Convert.ToInt32(masterClassificationDetail.TotalDisplayNumber.Replace("-", string.Empty));
                        updateClassification.ChartAccountDisplayNumber = txtGtypeChartAccountDisplayNumber.Text;
                        updateClassification.ChartAccountNumber = Convert.ToInt32(txtGtypeChartAccountDisplayNumber.Text.Replace("-", string.Empty));
                        updateClassification.ReportingTotalDisplayNumber = TotalDisplayNumber;
                        updateClassification.ReportingTotalNumber = TotalNumber;
                    }
                    db.SaveChanges();
                }

                GtypeErrorDiv.Visible = false;
                GtypeSuccess.Visible = true;
            }
        }
        #endregion

        #region S-Type
        protected void btnStypeClassification_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(txtStypeClassificationName.Text))
            {
                StypeErrorDiv.Visible = true;
                StypeErrorMessage.InnerText = "Classification Name is required.";
                return;
            }
            else if (String.IsNullOrEmpty(txtStypeClassificationDesc.Text))
            {
                StypeErrorDiv.Visible = true;
                StypeErrorMessage.InnerText = "Classification description is required.";
                return;
            }
            else
            {
                using (var db = new KFentities())
                {
                    var updateMasterClassification = db.MasterClassifications.Where(d => d.SubTotalDisplayNumber == hdnSTypeMasterClassificationSubTotalDisplayNumber.Value).FirstOrDefault();
                    String MasterClassificationName = String.Empty;

                    var updateClassification = db.Classifications.Where(f => f.Id == classificationId).FirstOrDefault();
                    updateClassification.ClassificationType = txtStypeClassificationName.Text.Trim();
                    updateClassification.Desc = txtStypeClassificationDesc.Text.Trim();
                    updateMasterClassification.ReportingTotal = txtStypeClassificationName.Text.Trim() + "-" + txtStypeChartAccountRange.Text;
                    if (!IsClassificationUsed)
                    {
                       

                        var masterClassificationDetail = db.MasterClassifications.Where(f => f.ReportingTotal.Equals(ddlStypeClassificationSubTotalList.SelectedItem.Text)).FirstOrDefault();
                        String TotalDisplayNumber = masterClassificationDetail.TotalDisplayNumber;
                        Int32 TotalNumber = Convert.ToInt32(masterClassificationDetail.TotalDisplayNumber.Replace("-", string.Empty));
                        updateClassification.ChartAccountDisplayNumber = txtStypeChartAccountRange.Text;
                        updateClassification.ChartAccountNumber = Convert.ToInt32(txtStypeChartAccountRange.Text.Replace("-", string.Empty));
                        updateClassification.ReportingTotalDisplayNumber = TotalDisplayNumber;
                        updateClassification.ReportingTotalNumber = TotalNumber;

                         //Master Update
                        updateMasterClassification.NewClassificationVerb = txtStypeChartAccountNumber.Text;
                        updateMasterClassification.SubTotalDisplayNumber = txtStypeChartAccountRange.Text;
                        updateMasterClassification.TotalDisplayNumber = TotalDisplayNumber;
                    }
                    hdnSTypeMasterClassificationSubTotalDisplayNumber.Value = txtStypeChartAccountRange.Text;
                    db.SaveChanges();
                }
                //save the classification
                StypeErrorDiv.Visible = false;
                StypeSuccess.Visible = true;
            }
        }

        protected void ddlStypeClassificationSubTotalList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ddlStypeClassificationSubTotalList.SelectedIndex > 0)
            {
                BindStypeData();
                btnStypeClassification.Enabled = false;
                txtStypeChartAccountNumber.Text = string.Empty;
            }
        }

        protected void txtStypeChartAccountNumber_TextChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtStypeChartAccountNumber.Text))
            {
                //Check if inputted number is valid range number and also check in database 
                var range = txtStypeRange.Text.Split('-');
                int StartRange = 0;
                int EndRange = 0;
                StartRange = Convert.ToInt32(range[0]);
                EndRange = Convert.ToInt32(range[1]);
                if (!CheckNumberRange(Convert.ToInt32(txtStypeChartAccountNumber.Text), StartRange, EndRange))
                {
                    StypeErrorDiv.Visible = true;
                    StypeErrorMessage.InnerText = "Invalid number. Please try another number.";
                    btnStypeClassification.Enabled = false;
                    return;
                }
                string NewChartAccNumber = txtStypeChartAccountNumber.Text + hdnStypePrefix.Value;

                //Check in database
                using (var context = new KFentities())
                {
                    if (context.Classifications.Where(d => d.ChartAccountDisplayNumber.Equals(NewChartAccNumber) && (d.UserId == UserID || d.UserId == null)).Any()
                        && NewChartAccNumber != context.Classifications.Where(g => g.Id == classificationId).Select(f => f.ChartAccountDisplayNumber).FirstOrDefault()
                        )
                    {
                        StypeErrorDiv.Visible = true;
                        StypeErrorMessage.InnerText = "Classification already existed. Please try another number.";
                        btnStypeClassification.Enabled = false;
                    }
                    else
                    {
                        StypeErrorDiv.Visible = false;
                        btnStypeClassification.Enabled = true;
                    }
                }
                txtStypeChartAccountRange.Text = NewChartAccNumber;
            }

            else
            {
                btnStypeClassification.Enabled = false;
                return;
            }
        }

        #region S-Type Methods
        public void BindStypeData()
        {
            using (var context = new KFentities())
            {
                TtypeDetails = context.MasterClassifications.Where(d => d.ReportingTotal.Equals(ddlStypeClassificationSubTotalList.SelectedItem.Text.ToString())).FirstOrDefault();
            }
            txtStypeChartAccountNumber.Text = string.Empty;
            btnStypeClassification.Enabled = false;
            txtStypeRange.Text = TtypeDetails.ReportingTotalRange;
            txtStypeChartAccountRange.Text = "-" + TtypeDetails.NewClassificationVerb;
            hdnStypePrefix.Value = "-" + TtypeDetails.NewClassificationVerb;
        }
        public void BindStypeSubTotalList()
        {
            using (var context = new KFentities())
            {
                var ObjList = context.MasterClassifications.Where(d => d.RepotingTotalType == "T").ToList();
                ddlStypeClassificationSubTotalList.DataSource = ObjList;
                ddlStypeClassificationSubTotalList.DataBind();
                ddlStypeClassificationSubTotalList.DataTextField = "ReportingTotal";
                ddlStypeClassificationSubTotalList.DataValueField = "Id";
                ddlStypeClassificationSubTotalList.DataBind();
                ddlStypeClassificationSubTotalList.Items.Insert(0, "Please Select");
            }
        }
        #endregion
        #endregion

    }
}