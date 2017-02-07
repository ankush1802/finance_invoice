using KF.Entity;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace KF.Web
{
    public partial class AddClassification : System.Web.UI.Page
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
                if (!string.IsNullOrEmpty(Request.QueryString["userid"]))
                {
                    UserID = Convert.ToInt32(Request.QueryString["userid"]);
                }
                else
                {
                    Response.Redirect("/Kippin/Finance/Accounting/StatementList");
                }
                //Get UserId in query string
                //userid
                BindAtypeSubTotalList();
                BindStypeSubTotalList();
                BindGtypeSubTotalList();
              //  BindTtypeSubTotalList();
            }
        }



        #region A-Type Classification

        protected void ddlAtypeClassificationSubTotalList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ddlAtypeClassificationSubTotalList.SelectedIndex > 0)
            {
                BindAtypeData();
                txtAtypeClassificationValidationAccNumber.Enabled = true;
                txtAtypeClassificationValidationAccNumber.Text = string.Empty;
            }
            else
            {
                txtAtypeClassificationValidationAccNumber.Enabled = false;
                btnAtypeClassification.Enabled = false;
                txtAtypeClassificationNewAccount.Text = string.Empty;
                txtAtypeClassificationRangeAcct.Text = string.Empty;
                txtAtypeClassificationValidationAccNumber.Text = string.Empty;
            }
            HideAlertMessages();
        }

        protected void txtreqAtypeClassificationValidationAccNumber_TextChanged(object sender, EventArgs e)
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
                    if (context.Classifications.Where(d => d.ChartAccountDisplayNumber.Equals(NewChartAccNumber) && (d.UserId== UserID || d.UserId == null)).Any())
                    {
                        AtypeErrorDiv.Visible = true;
                        AtypeErrorMessage.InnerText = "Classification already existed. Please try another number.";
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
                //save the classification
                AtypeErrorDiv.Visible = false;
                SaveClassification("A");
                ResetControl("A");
                AtypeSuccess.Visible = true;
            }
        }

        #region Methods
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
                var ObjList = context.MasterClassifications.Where(d => d.RepotingTotalType == "S" && d.UserId== null).ToList();
                ObjList.AddRange(context.MasterClassifications.Where(d => d.RepotingTotalType == "S" && d.UserId == UserID).ToList());
                ddlAtypeClassificationSubTotalList.DataSource = ObjList;
                ddlAtypeClassificationSubTotalList.DataBind();
                ddlAtypeClassificationSubTotalList.DataTextField = "ReportingTotal";
                ddlAtypeClassificationSubTotalList.DataValueField = "Id";
                ddlAtypeClassificationSubTotalList.DataBind();
                ddlAtypeClassificationSubTotalList.Items.Insert(0, "Please Select");
            }
        }
        #endregion

        #endregion

        #region S-Type Classification

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
                    return;
                }
                string NewChartAccNumber = txtStypeChartAccountNumber.Text + hdnStypePrefix.Value;

                //Check in database
                using (var context = new KFentities())
                {
                    if (context.Classifications.Where(d => d.ChartAccountDisplayNumber.Equals(NewChartAccNumber) && (d.UserId== UserID || d.UserId == null)).Any())
                    {
                        StypeErrorDiv.Visible = true;
                        StypeErrorMessage.InnerText = "Classification already existed. Please try another number.";
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

        protected void ddlStypeClassificationSubTotalList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ddlStypeClassificationSubTotalList.SelectedIndex > 0)
            {
                BindStypeData();
                txtStypeChartAccountNumber.Enabled = true;
                txtGtypeChartAccountNumber.Text = string.Empty;
            }
            else
            {
                txtStypeChartAccountNumber.Enabled = false;
                btnStypeClassification.Enabled = false;
                txtStypeChartAccountRange.Text = string.Empty;
                txtStypeRange.Text = string.Empty;
                txtStypeChartAccountNumber.Text = string.Empty;
            }
            HideAlertMessages();
        }

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
                //save the classification
                StypeErrorDiv.Visible = false;
                SaveClassification("S");
                ResetControl("S");
                StypeSuccess.Visible = true;
            }
        }

        #region S-Type Methods
        public void BindStypeData()
        {
            using (var context = new KFentities())
            {
                TtypeDetails = context.MasterClassifications.Where(d => d.ReportingTotal.Equals(ddlStypeClassificationSubTotalList.SelectedItem.Text.ToString())).FirstOrDefault();
            }

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

        #region G-Type Classification
        protected void ddlGtypeClassificationSubTotalList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ddlGtypeClassificationSubTotalList.SelectedIndex > 0)
            {
                BindGtypeData();
                txtGtypeChartAccountNumber.Enabled = true;
                txtStypeChartAccountNumber.Text = string.Empty;
            }
            else
            {
                txtGtypeChartAccountNumber.Enabled = false;
                btnGtypeClassification.Enabled = false;
                txtGtypeRange.Text = string.Empty;
                txtGtypeChartAccountDisplayNumber.Text = string.Empty;
                txtGtypeChartAccountNumber.Text = string.Empty;
            }
            HideAlertMessages();
        }

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
                    return;
                }
                string NewChartAccNumber = hdnGtypePrefix.Value + txtGtypeChartAccountNumber.Text;

                //Check in database
                using (var context = new KFentities())
                {
                    if (context.Classifications.Where(d => d.ChartAccountDisplayNumber.Equals(NewChartAccNumber) && (d.UserId == UserID || d.UserId == null)).Any())
                    {
                        GtypeErrorDiv.Visible = true;
                        GtypeErrorMessage.InnerText = "Classification already existed. Please try another number.";
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
                //save the classification
                GtypeErrorDiv.Visible = false;
                SaveClassification("G");
                ResetControl("G");
                GtypeSuccess.Visible = true;
            }
        }
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
        #endregion

        #region T-Type Classification
        //protected void btnTtypeClassification_Click(object sender, EventArgs e)
        //{
        //    if (String.IsNullOrEmpty(txtTtypeClassificationName.Text))
        //    {
        //        TtypeErrorDiv.Visible = true;
        //        TtypeErrorMessage.InnerText = "Classification Name is required.";
        //        return;
        //    }
        //    else if (String.IsNullOrEmpty(txtGtypeClassificationDesc.Text))
        //    {
        //        TtypeErrorDiv.Visible = true;
        //        TtypeErrorMessage.InnerText = "Classification description is required.";
        //        return;
        //    }
        //    else
        //    {
        //        //save the classification
        //        GtypeErrorDiv.Visible = false;
        //        SaveClassification("T");
        //        ResetControl("T");
        //        TtypeSuccess.Visible = true;
        //    }
        //}


        //protected void ddlTtypeClassificationSubTotalList_SelectedIndexChanged(object sender, EventArgs e)
        //{
        //    if (ddlTtypeClassificationSubTotalList.SelectedIndex > 0)
        //    {
        //        BindTtypeData();
        //        txtTtypeClassificationName.Focus();
        //    }
        //}

        //protected void txtTtypeChartAccountNumber_TextChanged(object sender, EventArgs e)
        //{
        //    if (!string.IsNullOrEmpty(txtTtypeChartAccountNumber.Text))
        //    {
        //        //Check if inputted number is valid range number and also check in database 
        //        var range = txtTtypeRange.Text.Split('-');
        //        int StartRange = 0;
        //        int EndRange = 0;
        //        StartRange = Convert.ToInt32(range[0]);
        //        EndRange = Convert.ToInt32(range[1]);
        //        if (!CheckNumberRange(Convert.ToInt32(txtTtypeChartAccountNumber.Text), StartRange, EndRange))
        //        {
        //            TtypeErrorDiv.Visible = true;
        //            TtypeErrorMessage.InnerText = "Invalid number. Please try another number.";
        //            return;
        //        }
        //        string NewChartAccNumber = txtTtypeChartAccountNumber.Text + hdnTtypePrefix.Value;

        //        //Check in database
        //        using (var context = new KFentities())
        //        {
        //            if (context.Classifications.Where(d => d.ChartAccountDisplayNumber.Equals(NewChartAccNumber)).Any())
        //            {
        //                TtypeErrorDiv.Visible = true;
        //                TtypeErrorMessage.InnerText = "Classification already existed. Please try another number.";
        //            }
        //            else
        //            {
        //                TtypeErrorDiv.Visible = false;
        //                btnTtypeClassification.Enabled = true;
        //            }
        //        }
        //        txtTtypeChartAccountDisplayNumber.Text = NewChartAccNumber;
        //    }

        //    else
        //    {
        //        btnTtypeClassification.Enabled = false;
        //        return;
        //    }
        //}

        #region T-Type Methods
        //public void BindTtypeData()
        //{
        //    using (var context = new KFentities())
        //    {
        //        TtypeDetails = context.MasterClassifications.Where(d => d.ReportingTotal.Equals(ddlTtypeClassificationSubTotalList.SelectedItem.Text.ToString())).FirstOrDefault();
        //    }

        //    txtTtypeRange.Text = TtypeDetails.ReportingTotalRange;
        //    txtTtypeChartAccountDisplayNumber.Text = "-" + TtypeDetails.NewClassificationVerb;
        //    hdnTtypePrefix.Value = "-" + TtypeDetails.NewClassificationVerb;
        //}
        //public void BindTtypeSubTotalList()
        //{
        //    using (var context = new KFentities())
        //    {
        //        var ObjList = context.MasterClassifications.Where(d => d.RepotingTotalType == "T").ToList();
        //        ddlTtypeClassificationSubTotalList.DataSource = ObjList;
        //        ddlTtypeClassificationSubTotalList.DataBind();
        //        ddlTtypeClassificationSubTotalList.DataTextField = "ReportingTotal";
        //        ddlTtypeClassificationSubTotalList.DataValueField = "Id";
        //        ddlTtypeClassificationSubTotalList.DataBind();
        //        ddlTtypeClassificationSubTotalList.Items.Insert(0, "Please Select");
        //    }
        //}
        #endregion
        #endregion

        #region Common Methods

        private void RebindControl()
        {
            BindAtypeSubTotalList();
            BindStypeSubTotalList();
            BindGtypeSubTotalList();
        }

        public void HideAlertMessages()
        {
            AtypeErrorDiv.Visible = false;
            StypeErrorDiv.Visible = false;
            GtypeErrorDiv.Visible = false;
            AtypeSuccess.Visible = false;
            StypeSuccess.Visible = false;
            GtypeSuccess.Visible = false;
        }

        /// <summary>
        /// Classification Range Checkm
        /// </summary>
        /// <param name="chartAccNo"></param>
        /// <param name="startRange"></param>
        /// <param name="EndRange"></param>
        /// <returns></returns>
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

        #region Save Classification
        public Boolean SaveClassification(string ClassificationType)
        {
            using (var context = new KFentities())
            {
                var masterClassificationDetail = new MasterClassification();
                String ChartAccountDisplayNumber = string.Empty;
                if (ClassificationType == "A")
                {
                    ChartAccountDisplayNumber = txtAtypeClassificationNewAccount.Text;
                    masterClassificationDetail = context.MasterClassifications.Where(f => f.ReportingTotal.Equals(ddlAtypeClassificationSubTotalList.SelectedItem.Text)).FirstOrDefault();
                }
                if (ClassificationType == "S")
                {
                    ChartAccountDisplayNumber = txtStypeChartAccountRange.Text;
                    masterClassificationDetail = context.MasterClassifications.Where(f => f.ReportingTotal.Equals(ddlStypeClassificationSubTotalList.SelectedItem.Text)).FirstOrDefault();
                }
                if (ClassificationType == "G")
                {
                    ChartAccountDisplayNumber = txtGtypeChartAccountDisplayNumber.Text;
                    masterClassificationDetail = context.MasterClassifications.Where(f => f.ReportingTotal.Equals(ddlGtypeClassificationSubTotalList.SelectedItem.Text)).FirstOrDefault();
                }
                if (ClassificationType == "T")
                {
                    //ChartAccountDisplayNumber = txtTtypeChartAccountDisplayNumber.Text;
                   // masterClassificationDetail = context.MasterClassifications.Where(f => f.ReportingTotal.Equals(ddlTtypeClassificationSubTotalList.SelectedItem.Text)).FirstOrDefault();
                }

                Int32 ChartAccountNumber = Convert.ToInt32(ChartAccountDisplayNumber.Replace("-", string.Empty));
                Int32 CategoryId = Convert.ToInt32(masterClassificationDetail.CategoryId);
                String SubTotalDisplayNumber = string.Empty;
                Int32 SubTotalNumber = 0;
                String TotalDisplayNumber = string.Empty;
                Int32 TotalNumber = 0;
                String ClassificationName = string.Empty;
                String ClassificationDesc = string.Empty;
                if (ClassificationType == "A")
                {
                    SubTotalDisplayNumber = masterClassificationDetail.SubTotalDisplayNumber;
                    SubTotalNumber = Convert.ToInt32(masterClassificationDetail.SubTotalDisplayNumber.Replace("-", string.Empty));
                    ClassificationName = txtAtypeClassificationName.Text.Trim();
                    ClassificationDesc = txtAtypeClassificationSubAccountDesc.Text.Trim();
                }
                //if (ClassificationType == "T")
                //{
                //    //Create a automatic S type classification 
                //    TotalDisplayNumber = masterClassificationDetail.TotalDisplayNumber;
                //    TotalNumber = Convert.ToInt32(TotalDisplayNumber.Replace("-", string.Empty));
               
                //}
                if (ClassificationType == "S" || ClassificationType == "G")
                {
                    TotalDisplayNumber = masterClassificationDetail.TotalDisplayNumber;
                    TotalNumber = Convert.ToInt32(TotalDisplayNumber.Replace("-", string.Empty));
                    if (ClassificationType == "S")
                    {
                        ClassificationName = txtStypeClassificationName.Text.Trim();
                        ClassificationDesc = txtStypeClassificationDesc.Text.Trim();
                        SubTotalDisplayNumber = txtStypeChartAccountRange.Text;
                        SubTotalNumber = Convert.ToInt32(SubTotalDisplayNumber.Replace("-", string.Empty));
                    }
                    if (ClassificationType == "G")
                    {
                        ClassificationName = txtGtypeClassificationName.Text.Trim();
                        ClassificationDesc = txtGtypeClassificationDesc.Text.Trim();
                    }
                   
                }
                Classification dbInsert = new Classification();
                dbInsert.CategoryId = CategoryId;
                dbInsert.ChartAccountDisplayNumber = ChartAccountDisplayNumber;
                dbInsert.ChartAccountNumber = ChartAccountNumber;
                dbInsert.ClassificationType = ClassificationName;
                dbInsert.CreatedDate = DateTime.Now;
                dbInsert.Desc = ClassificationDesc;
                dbInsert.IsDeleted = false;
                dbInsert.Name = ClassificationDesc;
                dbInsert.ReportingSubTotalDisplayNumber = SubTotalDisplayNumber;
                dbInsert.ReportingSubTotalNumber = SubTotalNumber;
                dbInsert.ReportingTotalDisplayNumber = TotalDisplayNumber;
                dbInsert.ReportingTotalNumber = TotalNumber;
                dbInsert.Type = ClassificationType;
                dbInsert.UserId = UserID;
                context.Classifications.Add(dbInsert);
                if (ClassificationType == "S")
                {
                    var dbInsertMasterClassification = new MasterClassification();
                    dbInsertMasterClassification.NewClassificationVerb = txtStypeChartAccountNumber.Text;
                    dbInsertMasterClassification.CategoryId = CategoryId;
                    dbInsertMasterClassification.ReportingTotal = txtStypeClassificationName.Text + "-" + txtStypeChartAccountRange.Text;
                    dbInsertMasterClassification.RepotingTotalType = ClassificationType;
                    dbInsertMasterClassification.ReportingTotalRange = "0001-4999";
                    dbInsertMasterClassification.SubTotalDisplayNumber = txtStypeChartAccountRange.Text;// TotalDisplayNumber;
                    dbInsertMasterClassification.TotalDisplayNumber = TotalDisplayNumber;
                    dbInsertMasterClassification.UserId = UserID;
                    context.MasterClassifications.Add(dbInsertMasterClassification);
                }
                if (ClassificationType == "T")
                {
                    ////Create a automatic S type classification 
                    //var dbInsertMasterClassification = new MasterClassification();
                    //dbInsertMasterClassification.NewClassificationVerb = txtStypeChartAccountNumber.Text;
                    //dbInsertMasterClassification.CategoryId = CategoryId;
                    //dbInsertMasterClassification.ReportingTotal = txtStypeClassificationName.Text + "-" + txtStypeChartAccountRange.Text;
                    //dbInsertMasterClassification.RepotingTotalType = "S";
                    //dbInsertMasterClassification.ReportingTotalRange = "0001-4999";
                    //dbInsertMasterClassification.TotalDisplayNumber = TotalDisplayNumber;
                    //dbInsertMasterClassification.UserId = 1;
                    //context.MasterClassifications.Add(dbInsertMasterClassification);
                }
                //if (ClassificationType == "G")
                //{ }
                context.SaveChanges();
                RebindControl();
                return true;
            }
        }

        public void ResetControl(string ClassificationType)
        {
            if (ClassificationType == "A")
            {
                txtAtypeClassificationName.Text = string.Empty;
                txtAtypeClassificationSubAccountDesc.Text = string.Empty;
                txtAtypeClassificationValidationAccNumber.Text = string.Empty;
                txtAtypeClassificationRangeAcct.Text = string.Empty;
                txtAtypeClassificationNewAccount.Text = string.Empty;
                ddlAtypeClassificationSubTotalList.SelectedIndex = 0;
                btnAtypeClassification.Enabled = false;
            }
            if (ClassificationType == "S")
            {
                txtStypeChartAccountNumber.Text = string.Empty;
                txtStypeChartAccountRange.Text = string.Empty;
                txtStypeClassificationDesc.Text = string.Empty;
                txtStypeClassificationName.Text = string.Empty;
                txtStypeRange.Text = string.Empty;
                btnStypeClassification.Enabled = false;
                ddlStypeClassificationSubTotalList.SelectedIndex = 0;
            }
            if (ClassificationType == "G")
            {
                txtGtypeChartAccountNumber.Text = string.Empty;
                txtGtypeChartAccountDisplayNumber.Text = string.Empty;
                txtGtypeClassificationDesc.Text = string.Empty;
                txtGtypeClassificationName.Text = string.Empty;
                txtGtypeRange.Text = string.Empty;
                btnGtypeClassification.Enabled = false;
                ddlGtypeClassificationSubTotalList.SelectedIndex = 0;
            }
            if (ClassificationType == "T")
            {
                //txtTtypeChartAccountNumber.Text = string.Empty;
                //txtTtypeChartAccountDisplayNumber.Text = string.Empty;
                //txtTtypeClassificationDesc.Text = string.Empty;
                //txtTtypeClassificationName.Text = string.Empty;
                //txtTtypeRange.Text = string.Empty;
                //btnTtypeClassification.Enabled = false;
                //ddlTtypeClassificationSubTotalList.SelectedIndex = 0;
            }
        }
        #endregion



        #endregion

    }
}