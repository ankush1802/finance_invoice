using KF.Dto.Modules.Finance;
using KF.Entity;
using KF.Utilities.Common;
using KF.Web.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace KF.Web
{
    public partial class EditBankStatement : System.Web.UI.Page
    {
        #region Properties
        // private KF.Entity.KFentities _context = new Entity.KFentities();
        public string PreviousUrl
        {
            get { return Convert.ToString(ViewState["PreviousUrl"]); }
            set { ViewState["PreviousUrl"] = value; }
        }

        public string NextUrl
        {
            get { return Convert.ToString(ViewState["NextUrl"]); }
            set { ViewState["NextUrl"] = value; }
        }

        public Boolean IsUnsavedChanges
        {
            get { return Convert.ToBoolean(ViewState["IsUnsavedChanges"]); }
            set { ViewState["IsUnsavedChanges"] = value; }
        }
        public Int32 StatementID
        {
            get { return Convert.ToInt32(ViewState["StatementID"]); }
            set { ViewState["StatementID"] = value; }
        }
        public int Year
        {
            get { return Convert.ToInt32(ViewState["Year"]); }
            set { ViewState["Year"] = value; }
        }
        public UserRegistrationDto UserData
        {
            get { return ViewState["UserDetails"] as UserRegistrationDto; }
            set { ViewState["UserDetails"] = value; }
        }
        public ClassificationList classificationDbList
        {
            get { return ViewState["classificationDbList"] as ClassificationList; }
            set { ViewState["classificationDbList"] = value; }
        }
        public List<string> StatementBillList
        {
            get { return ViewState["StatementBillList"] as List<string>; }
            set { ViewState["StatementBillList"] = value; }
        }
        public ReconcillationBankExpenseDto BindStatementDetails
        {
            get { return ViewState["BindStatementDetails"] as ReconcillationBankExpenseDto; }
            set { ViewState["BindStatementDetails"] = value; }
        }

        #endregion

        #region Events
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (!string.IsNullOrEmpty(Request.QueryString["args"]))
                {
                    Dictionary<String, String> decryptedQueryString = new Dictionary<String, String>();
                    decryptedQueryString.ToDecryptedQueryString(Request.QueryString["args"].ToString());

                    if (decryptedQueryString.Where(s => s.Key == "StatementID").Any())
                    {
                        var statementData = decryptedQueryString.Where(s => s.Key == "StatementID").FirstOrDefault();
                        StatementID = Convert.ToInt32(statementData.Value);
                    }
                    if (decryptedQueryString.Where(s => s.Key == "Year").Any())
                    {
                        var yearData = decryptedQueryString.Where(s => s.Key == "Year").FirstOrDefault();
                        if (!string.IsNullOrEmpty(yearData.Value))
                        {
                            Year = Convert.ToInt32(yearData.Value);
                        }
                    }
                }
                else
                {
                    Response.Redirect("/Kippin/Finance/Accounting/StatementList");
                }

                // UserData = ClsDropDownList.GetUserDetailsByStatementId(StatementID);
                ViewState["UserDetails"] = ClsDropDownList.GetUserDetailsByStatementId(StatementID);
                GetClassificationList(UserData.Id);
                // txtCalculatedAmount.Text = txtTotal.Text;

                BindDataStatementData();

                #region Statement Bills
                var Chkfolder = Server.MapPath("~/OcrImages/" + UserData.Id + "/" + StatementID);
                List<string> BillImageList = new List<string>();
                if (Directory.Exists(Chkfolder))
                {
                    var imgList = Directory.EnumerateFiles(Server.MapPath("~/OcrImages/" + UserData.Id + "/" + StatementID + "/"))
                            .Select(fn => "~/OcrImages/" + UserData.Id + "/" + StatementID + "/" + Path.GetFileName(fn));
                    foreach (var item in imgList)
                    {
                        BillImageList.Add(item);
                    }
                }
                StatementBillList = BillImageList;
                #endregion

                IsUnsavedChanges = true;
            }



        }
        protected void btnSaveStatement_Click(object sender, EventArgs e)
        {
            if (ddlClassification.SelectedIndex > 0)
            {
                decimal TotalTax = 0;
                using (var context = new KFentities())
                {
                    int StatementId = Convert.ToInt32(hdnStatementId.Value);
                    var bankStatementData = context.BankExpenses.Where(s => s.Id == StatementId).FirstOrDefault();
                    bankStatementData.Purpose = !string.IsNullOrEmpty(txtPurpose.Text) ? txtPurpose.Text : string.Empty;
                    bankStatementData.Comments = !string.IsNullOrEmpty(txtComment.Text) ? txtComment.Text : string.Empty;
                    bankStatementData.ClassificationId = Convert.ToInt32(ddlClassification.SelectedValue);
                    bankStatementData.CategoryId = context.Classifications.Where(d => d.Id == bankStatementData.ClassificationId).Select(f => f.CategoryId).FirstOrDefault();

                    var oldchk = context.DirectorAccountLogs.Where(s => s.StatementId == StatementId).FirstOrDefault();
                    if (!string.IsNullOrEmpty(txtBusinessPercentage.Text))
                    {
                        double businessPercentage = Convert.ToDouble(txtBusinessPercentage.Text);
                        if (businessPercentage > 0)
                        {
                            bankStatementData.ActualPercentage = businessPercentage;

                            #region add director loan


                            decimal directorAmt = Convert.ToDecimal(hdnOriginalAmountBasedOnCreditnDebit.Value) - ((Convert.ToDecimal(businessPercentage) / 100) * Convert.ToDecimal(hdnOriginalAmountBasedOnCreditnDebit.Value));
                            if (oldchk == null)
                            {
                                var dbinsertPercentageAmount = new DirectorAccountLog();
                                dbinsertPercentageAmount.DateCreated = DateTime.Now;
                                dbinsertPercentageAmount.StatementId = StatementId;
                                dbinsertPercentageAmount.UserId = bankStatementData.UserId;
                                dbinsertPercentageAmount.Percentage = Convert.ToDecimal(bankStatementData.ActualPercentage);

                                if (directorAmt > 0)
                                {
                                    dbinsertPercentageAmount.PercentageAmount = directorAmt;
                                    context.DirectorAccountLogs.Add(dbinsertPercentageAmount);
                                }
                            }
                            else
                            {
                                //decimal totalamt = Decimal.Multiply(Convert.ToDecimal(bankStatementData.ActualTotal), Convert.ToDecimal(bankStatementData.ActualPercentage));
                                //decimal directorAmt = Decimal.Subtract(totalamt, Convert.ToDecimal(bankStatementData.ActualTotal));
                                if (directorAmt > 0)
                                {
                                    oldchk.PercentageAmount = directorAmt;
                                    oldchk.Percentage = Convert.ToDecimal(bankStatementData.ActualPercentage);
                                }
                            }


                            #endregion
                        }

                    }
                    else
                    {
                        if (oldchk != null)
                        {
                            oldchk.DateModified = DateTime.Now;
                            oldchk.Percentage = 0;
                            oldchk.PercentageAmount = 0;
                            bankStatementData.ActualPercentage = 0;
                        }
                    }

                    if (!string.IsNullOrEmpty(txtGSTAmt.Text))
                    {
                        decimal GstTaxAmount = Convert.ToDecimal(txtGSTAmt.Text);
                        decimal GSTTaxPercentage = Convert.ToDecimal(txtGSTPerc.Text);
                        if (GstTaxAmount > 0)
                        {
                            TotalTax = Decimal.Add(TotalTax, GstTaxAmount);
                            bankStatementData.GSTtax = GstTaxAmount;
                            bankStatementData.GSTPercentage = GSTTaxPercentage;
                        }
                    }
                    else
                    {
                        bankStatementData.GSTtax = 0;
                        bankStatementData.GSTPercentage = 0;
                    }
                    if (!string.IsNullOrEmpty(txtQSTAmt.Text))
                    {
                        decimal QstTaxAmount = Convert.ToDecimal(txtQSTAmt.Text);
                        decimal QSTTaxPercentage = Convert.ToDecimal(txtQSTPerc.Text);
                        if (QstTaxAmount > 0)
                        {
                            TotalTax = Decimal.Add(TotalTax, QstTaxAmount);
                            bankStatementData.QSTtax = QstTaxAmount;
                            bankStatementData.QSTPercentage = QSTTaxPercentage;
                        }
                    }
                    else
                    {
                        bankStatementData.QSTtax = 0;
                        bankStatementData.QSTPercentage = 0;
                    }
                    if (!string.IsNullOrEmpty(txtHSTAmt.Text))
                    {
                        decimal HstTaxAmount = Convert.ToDecimal(txtHSTAmt.Text);
                        decimal HSTTaxPercentage = Convert.ToDecimal(txtHSTPerc.Text);
                        if (HstTaxAmount > 0)
                        {
                            TotalTax = Decimal.Add(TotalTax, HstTaxAmount);
                            bankStatementData.HSTtax = HstTaxAmount;
                            bankStatementData.HSTPercentage = HSTTaxPercentage;
                        }
                    }
                    else
                    {
                        bankStatementData.HSTtax = 0;
                        bankStatementData.HSTPercentage = 0;
                    }
                    if (!string.IsNullOrEmpty(txtPSTAmt.Text))
                    {
                        decimal PstTaxAmount = Convert.ToDecimal(txtPSTAmt.Text);
                        decimal PSTTaxPercentage = Convert.ToDecimal(txtPSTPerc.Text);
                        if (PstTaxAmount > 0)
                        {
                            TotalTax = Decimal.Add(TotalTax, PstTaxAmount);
                            bankStatementData.PSTtax = PstTaxAmount;
                            bankStatementData.PSTPercentage = PSTTaxPercentage;
                        }
                    }
                    else
                    {
                        bankStatementData.PSTtax = 0;
                        bankStatementData.PSTPercentage = 0;
                    }
                    bankStatementData.ActualTotal = Convert.ToDecimal(txtCalculatedAmount.Text);
                    bankStatementData.TotalTax = TotalTax;
                    bankStatementData.Total = Convert.ToDecimal(hdnOriginalAmountBasedOnCreditnDebit.Value);
                    bankStatementData.StatusId = 4;
                    context.SaveChanges();
                    statementStatusDiv.Attributes.Add("class", "alert alert-success col-sm-12 text-center");
                    statusText.InnerText = "Statement Locked.";
                    IsUnsavedChanges = true;
                }
            }

            else
            {
                errorDiv.Visible = true;
                customerrorSummary.InnerText = "Please Select Classification.";
            }

        }

        protected void btnPrevious_Click(object sender, EventArgs e)
        {
            if (IsUnsavedChanges == true)
            {
                Response.Redirect(PreviousUrl);
            }
            else
            {
                //show warning
                UnsavedMessaggeDiv.Visible = true;
                IsUnsavedChanges = true;
            }
        }

        protected void btnNext_Click(object sender, EventArgs e)
        {
            if (IsUnsavedChanges == true)
            {
                Response.Redirect(NextUrl);
            }
            else
            {
                //show error
                UnsavedMessaggeDiv.Visible = true;
                IsUnsavedChanges = true;
            }
        }
        #endregion

        #region Dropdown List
        private void GetClassificationList(int userID)
        {
            var classificationList = new List<ClassificationList>();
            // ClsDropDownList
            using (var db = new KFentities())
            {
                try
                {
                    List<Classification> objList = new List<Classification>();

                    var userData = db.UserRegistrations.Where(u => u.Id == userID).FirstOrDefault();
                    switch (userData.OwnershipId)
                    {
                        case 1:
                            objList = db.Classifications.Where(i => i.IsSole == true && i.IndustryId == null && i.IsDeleted == false).ToList();
                            break;
                        case 2:
                            objList = db.Classifications.Where(i => i.IsIncorporated == true && i.IndustryId == null && i.IsDeleted == false).ToList();
                            break;
                        case 3:
                            objList = db.Classifications.Where(i => i.IsPartnerShip == true && i.IndustryId == null && i.IsDeleted == false).ToList();
                            break;
                        default:
                            objList = db.Classifications.Where(i => i.IsPartnerShip == true && i.IsDeleted == false && i.IndustryId == null).ToList();
                            break;

                    }

                    var industryClassification = db.Classifications.Where(i => i.IndustryId == userData.IndustryId && i.IsDeleted == false).ToList();
                    if (userData.SubIndustryId != null)
                    {
                        objList.AddRange(industryClassification.Where(s => s.SubIndustryId == userData.SubIndustryId && s.IsDeleted == false).ToList());
                    }
                    objList.AddRange(db.Classifications.Where(i => i.UserId == userID && i.IsDeleted == false
                     && i.ChartAccountDisplayNumber != "1050-1060" && i.ChartAccountDisplayNumber != "1050-1061"
                     && i.ChartAccountDisplayNumber != "1050-1062" && i.ChartAccountDisplayNumber != "1050-1063"
                     && i.ChartAccountDisplayNumber != "1050-1064" && i.ChartAccountDisplayNumber != "1050-1073"
                     && i.ChartAccountDisplayNumber != "1100-1080" && i.ChartAccountDisplayNumber != "1100-1081"
                     && i.ChartAccountDisplayNumber != "1100-1082" && i.ChartAccountDisplayNumber != "1100-1083"
                     && i.ChartAccountDisplayNumber != "1100-1087" && i.ChartAccountDisplayNumber != "1100-1088"
                     && i.ChartAccountDisplayNumber != "1100-1089"
                        ).ToList());
                    objList = objList.Where(d => d.Type.Trim() != "H" && d.Type.Trim() != "S" && d.Type.Trim() != "T" && d.Type.Trim() != "Z").ToList();
                    objList = objList.Where(s => s.IsDeleted == false && s.ChartAccountDisplayNumber != "3550-3561" && s.ChartAccountDisplayNumber != "3550-3600").ToList();

                    var reorderedClassificationList = new List<Classification>();
                    reorderedClassificationList.AddRange(objList.Where(d => d.CategoryId == 1).ToList()); //Asset
                    reorderedClassificationList.AddRange(objList.Where(d => d.CategoryId == 4).ToList()); //Liability
                    reorderedClassificationList.AddRange(objList.Where(d => d.CategoryId == 5).ToList()); //Equity
                    reorderedClassificationList.AddRange(objList.Where(d => d.CategoryId == 3).ToList()); //Revenue
                    reorderedClassificationList.AddRange(objList.Where(d => d.CategoryId == 2).ToList()); //Expense

                    var ddlClassificationSource = new List<ListItem>();
                    foreach (var item in reorderedClassificationList)
                    {
                        var ddlrowItem = new ListItem();
                        ddlrowItem.Text = item.ClassificationType;
                        ddlrowItem.Value = Convert.ToString(item.Id);
                        ddlrowItem.Attributes["DataGroupField"] = db.Categories.Where(s => s.Id == item.CategoryId).Select(w => w.CategoryType).FirstOrDefault();
                        ddlClassification.Items.Add(ddlrowItem);
                    }

                    ddlClassification.DataBind();
                    ListItem selectAnItem = new ListItem("Select an Classification", "-1");
                    selectAnItem.Attributes.Add("DataGroupField", "");
                    ddlClassification.Items.Insert(0, selectAnItem);
                }
                catch (Exception)
                {
                    throw;
                }


            }



        }

        protected void ddlClassification_SelectedIndexChanged(object sender, EventArgs e)
        {
            IsUnsavedChanges = false;
            if (ddlClassification.SelectedIndex > 0)
            {
                var data = ClsDropDownList.GetCategoryById(Convert.ToInt32(ddlClassification.SelectedItem.Value));
                txtCategory.Text = data.CategoryType;
                // txtDescription.Text = data.ClassificationDesc;
                txtClassificationDescription.Text = data.ClassificationDesc;
            }
        }

        #endregion

        #region Methods
        private void BindDataStatementData()
        {
            ReconcillationBankExpenseDto Obj = new ReconcillationBankExpenseDto();
            using (var context = new KFentities())
            {
                BankExpense data = new BankExpense();
                data = context.BankExpenses.Where(i => i.Id == StatementID && i.IsVirtualEntry != true).FirstOrDefault();
                if (Year > 0)
                {
                    //prevRecId
                    Nullable<int> yearValue = context.tblYears.Where(s => s.Id == Year).Select(d => d.Year).FirstOrDefault();
                    var nextRecId = context.BankExpenses.Where(i => i.Id > StatementID && i.Date.Value.Year == yearValue && i.BankId == data.BankId && i.IsVirtualEntry != true && i.UserId == data.UserId && i.IsDeleted == false).FirstOrDefault();
                    var prevRecId = context.BankExpenses.Where(i => i.Id < StatementID && i.Date.Value.Year == yearValue && i.BankId == data.BankId && i.IsVirtualEntry != true && i.UserId == data.UserId && i.IsDeleted == false).OrderByDescending(a => a.Id).FirstOrDefault();
                    if (prevRecId != null)
                    {
                        Obj.PrevRecId = prevRecId.Id;
                    }
                    else
                    {
                        Obj.PrevRecId = 0;
                        btnPrevious.Enabled = false;
                    }
                    if (nextRecId != null)
                    {
                        Obj.NextRecId = nextRecId.Id;

                    }
                    else
                    {
                        Obj.NextRecId = 0;
                        btnNext.Enabled = false;
                    }
                }
                else
                {
                    var prevRecId = context.BankExpenses.Where(i => i.Id > StatementID && i.BankId == data.BankId && i.UserId == data.UserId && i.IsVirtualEntry != true && i.IsDeleted == false).FirstOrDefault();
                    var nextRecId = context.BankExpenses.Where(i => i.Id < StatementID && i.BankId == data.BankId && i.UserId == data.UserId && i.IsVirtualEntry != true && i.IsDeleted == false).OrderByDescending(a => a.Id).FirstOrDefault();
                    if (prevRecId != null)
                    {
                        Obj.PrevRecId = prevRecId.Id;
                    }
                    else
                    {
                        Obj.PrevRecId = 0;
                        btnPrevious.Enabled = false;
                    }
                    if (nextRecId != null)
                    {
                        Obj.NextRecId = nextRecId.Id;
                    }
                    else
                    {
                        Obj.NextRecId = 0;
                        btnNext.Enabled = false;
                    }
                }

                #region Next / Previous
                if (Obj.PrevRecId > 0)
                {
                    Dictionary<String, String> encryptedQueryString = new Dictionary<String, String>();
                    encryptedQueryString = new Dictionary<String, String> { { "StatementID", Obj.PrevRecId.ToString() }, { "Year", Year > 0 ? Year.ToString() : string.Empty } };
                    String encryptQueryString = Security.ToEncryptedQueryString(encryptedQueryString);
                    //return RedirectToAction("StatementReconcilation", new { args = encryptQueryString });//Accounting/StatementList
                    //PreviousUrl = String.Format("/EditBankStatement.aspx?args={0}", encryptQueryString);
                    PreviousUrl = String.Format("/Kippin/Finance/EditBankStatement.aspx?args={0}", encryptQueryString);
                }
                if (Obj.NextRecId > 0)
                {
                    Dictionary<String, String> encryptedQueryString = new Dictionary<String, String>();
                    encryptedQueryString = new Dictionary<String, String> { { "StatementID", Obj.NextRecId.ToString() }, { "Year", Year > 0 ? Year.ToString() : string.Empty } };
                    String encryptQueryString = Security.ToEncryptedQueryString(encryptedQueryString);
                    //return RedirectToAction("StatementReconcilation", new { args = encryptQueryString });
                    NextUrl = String.Format("/Kippin/Finance/EditBankStatement.aspx?args={0}", encryptQueryString);
                }
                #endregion

                #region Bind Bank Data
                if (data.ClassificationId == 1)
                {
                    ddlClassification.SelectedIndex = -1;
                }
                else
                {
                    ddlClassification.SelectedValue = Convert.ToString(data.ClassificationId);
                }
                var classificationDetails = ClsDropDownList.GetCategoryById(Convert.ToInt32(data.ClassificationId));
                txtCategory.Text = classificationDetails.CategoryType;
                txtDescription.Text = data.Description;
                txtClassificationDescription.Text = classificationDetails.ClassificationDesc;
                txtPurpose.Text = data.Purpose;
                txtDate.Text = data.Date.Value.ToShortDateString();
                txtComment.Text = data.Comments;
                txtCredit.Text = Convert.ToString(data.Credit);
                txtDebit.Text = Convert.ToString(data.Debit);
                if (data.Credit > 0)
                {
                    hdnOriginalAmountBasedOnCreditnDebit.Value = data.Credit.ToString();
                }
                else if (data.Debit > 0)
                {
                    hdnOriginalAmountBasedOnCreditnDebit.Value = data.Debit.ToString();
                }
                //if (data.ActualTotal > 0)
                //{
                //    hdnOriginalAmountAfterStatementReconciliation.Value = data.ActualTotal.ToString();

                //}
                #endregion

                #region Bind Bill Data
                if (data.ActualTotal > 0)
                {
                    txtCalculatedAmount.Text = data.ActualTotal.ToString();
                }
                else
                {
                    txtCalculatedAmount.Text = Convert.ToDecimal(data.Credit) > Convert.ToDecimal(data.Debit) ? data.Credit.ToString() : data.Debit.ToString();
                }

                txtBusinessPercentage.Text = Convert.ToString(data.ActualPercentage);
                txtGSTAmt.Text = Convert.ToString(data.GSTtax);
                txtQSTAmt.Text = Convert.ToString(data.QSTtax);
                txtHSTAmt.Text = Convert.ToString(data.HSTtax);
                txtPSTAmt.Text = Convert.ToString(data.PSTtax);

                txtGSTPerc.Text = Convert.ToString(data.GSTPercentage);
                txtQSTPerc.Text = Convert.ToString(data.QSTPercentage);
                txtHSTPerc.Text = Convert.ToString(data.HSTPercentage);
                txtPSTPerc.Text = Convert.ToString(data.PSTPercentage);
                #endregion

                Obj.Id = data.Id;
                hdnStatementId.Value = data.Id.ToString();
                Obj.UserId = Convert.ToInt32(data.UserId);
                Obj.TotalBalance = Convert.ToDecimal(data.Total);
                data.ClassificationId = context.Classifications.Where(u => u.Id == data.ClassificationId).Select(d => d.Id).FirstOrDefault();

                //Obj.Category = data.Category1.CategoryType;
                Obj.Category = context.Categories.Where(s => s.Id == data.CategoryId).Select(s => s.CategoryType).FirstOrDefault();
                Obj.CategoryId = Convert.ToInt32(data.CategoryId);
                Obj.ClassificationId = data.ClassificationId == 1 ? null : data.ClassificationId;

                Obj.ClassificationDescription = (data.Classification.Desc == null ? "" : data.Classification.Desc);

                Obj.Comment = string.IsNullOrEmpty(data.Comments) ? "Not Available" : data.Comments;
                Obj.Credit = data.Credit;
                Obj.Vendor = data.Vendor;
                Obj.Purpose = data.Purpose;
                Obj.AccountClassificationId = Convert.ToInt32(data.AccountClassificationId);
                Obj.BankId = Convert.ToInt32(data.BankId);
                Obj.StatementId = Convert.ToInt32(data.Id);
                Obj.Status = context.Status.Where(k => k.Id == data.StatusId).Select(a => a.StatusType).FirstOrDefault();
                if (data.Date.Value.Date == null)
                {
                    Obj.Date = DateTime.Now.Date;
                }
                else
                {
                    Obj.Date = data.Date.Value.Date;
                }

                Obj.Debit = data.Debit;
                Obj.Total = data.Total;
                Obj.UserId = Convert.ToInt32(data.UserId);
                Obj.Description = data.Description;
                Obj.Classification = context.Classifications.Where(u => u.Id == data.ClassificationId).Select(o => o.ClassificationType).FirstOrDefault();
                Obj.Description = data.Description;
                Obj.Vendor = data.Vendor;

                //1	Pending
                //2	Submitted
                //3	Rejected
                //4	Locked
                if (data.StatusId == 4)
                {
                    statementStatusDiv.Attributes.Add("class", "alert alert-success col-sm-12 text-center");
                    statusText.InnerText = "Statement Locked.";
                }
                else if (data.StatusId == 3)
                {
                    statementStatusDiv.Attributes.Add("class", "alert alert-danger col-sm-12 text-center");
                    statusText.InnerText = "Statement Rejected.";
                }
                else if (data.StatusId == 2)
                {
                    statementStatusDiv.Attributes.Add("class", "alert alert-info col-sm-12 text-center");
                    statusText.InnerText = "Statement Submitted.";
                }
                else
                {
                    statementStatusDiv.Attributes.Add("class", "alert alert-info col-sm-12 text-center");
                    statusText.InnerText = "Statement Pending.";
                }

                viewUploadedBillsDiv.Attributes.Add("data-UserId", data.UserId.ToString());
                viewUploadedBillsDiv.Attributes.Add("data-statementId", data.Id.ToString());


                btnbillupload.Attributes.Add("data-UserId", data.UserId.ToString());
                btnbillupload.Attributes.Add("data-StatementId", data.Id.ToString());
                btnbillupload.Attributes.Add("data-BankId", data.BankId.ToString());



                btnUploadSelectedCloudImage.Attributes.Add("data-UserId", data.UserId.ToString());
                btnUploadSelectedCloudImage.Attributes.Add("data-StatementId", data.Id.ToString());
                btnUploadSelectedCloudImage.Attributes.Add("data-BankId", data.BankId.ToString());

                if (data.BankId == 6 || data.BankId == 8) //For MJV and INV(invoice entries)
                {
                    txtBusinessPercentage.Enabled = false;

                    txtGSTPerc.Enabled = false;
                    txtGSTAmt.Enabled = false;

                    txtQSTPerc.Enabled = false;
                    txtQSTAmt.Enabled = false;

                    txtHSTPerc.Enabled = false;
                    txtHSTAmt.Enabled = false;

                    txtPSTPerc.Enabled = false;
                    txtPSTAmt.Enabled = false;

                    txtComment.Enabled = false;
                    
                }
                txtCredit.Enabled = false;
                txtDebit.Enabled = false;
                txtClassificationDescription.Enabled = false;
                var fiscalYearPosting = context.FiscalYearPostings.Where(f => f.UserId == data.UserId).OrderByDescending(g => g.Id).ToList();
                if (fiscalYearPosting.Count > 0)
                {
                    foreach (var item in fiscalYearPosting)
                    {
                        if (data.Date >= item.TaxStartYear && data.Date <= item.TaxEndYear)
                        {
                            btnSaveStatement.Enabled = false;
                            btnRejectStatement.Enabled = false;
                        }
                    }

                }
            }
        }

        #region Calculate Actual Total
        private void CalculateActualTotal()
        {
            IsUnsavedChanges = false;
            decimal Total = 0;
            decimal ActualTotal = 0;
            decimal NewActualTotal = 0;
            decimal GST = 0;
            decimal QST = 0;
            decimal HST = 0;
            decimal PST = 0;
            decimal Tax = 0;
            //if (!string.IsNullOrEmpty(hdnOriginalAmountAfterStatementReconciliation.Value))
            //{
            //    if (Convert.ToDecimal(hdnOriginalAmountAfterStatementReconciliation.Value) > 0)
            //    {
            //        Total = Convert.ToDecimal(hdnOriginalAmountAfterStatementReconciliation.Value);
            //    }
            //}
            //else
            //{
            Total = Convert.ToDecimal(hdnOriginalAmountBasedOnCreditnDebit.Value);
            //}

            if (!string.IsNullOrEmpty(txtBusinessPercentage.Text))
            {
                decimal actPerc = Convert.ToDecimal(txtBusinessPercentage.Text);
                if (actPerc > 0)
                    ActualTotal = (Total * actPerc) / 100;
                else
                    ActualTotal = Total;
            }
            else
            {
                ActualTotal = Total;
            }
            if (!string.IsNullOrEmpty(txtGSTAmt.Text))
            {
                GST = Convert.ToDecimal(txtGSTAmt.Text);
            }
            if (!string.IsNullOrEmpty(txtQSTAmt.Text))
            {
                QST = Convert.ToDecimal(txtQSTAmt.Text);
            }
            if (!string.IsNullOrEmpty(txtHSTAmt.Text))
            {
                HST = Convert.ToDecimal(txtHSTAmt.Text);
            }
            if (!string.IsNullOrEmpty(txtPSTAmt.Text))
            {
                PST = Convert.ToDecimal(txtPSTAmt.Text);
            }
            Tax = GST + QST + HST + PST;
            NewActualTotal = Decimal.Subtract(ActualTotal, Tax);
            if (Tax > ActualTotal)
            {
                //revert back
                errorDiv.Visible = true;
                customerrorSummary.InnerText = "Tax amount is greater than bill data amount. Please update";
                btnSaveStatement.Enabled = false;
            }
            else
            {
                if (NewActualTotal > 0)
                {
                    errorDiv.Visible = false;
                    txtCalculatedAmount.Text = String.Format("{0:0.00}", NewActualTotal.ToString());
                    btnSaveStatement.Enabled = true;
                }
                else
                {
                    //revert back
                    errorDiv.Visible = true;
                    customerrorSummary.InnerText = "Amount should be greater than tax amount. Please update";
                    btnSaveStatement.Enabled = false;
                }
            }
        }
        #endregion

        #region Calculate New Percentage
        private string AutoCalculatePercentage(string intialtax, bool isPercentage)
        {
            IsUnsavedChanges = false;
            if (!string.IsNullOrEmpty(intialtax))
            {
                decimal Total = 0;
                decimal IntialtaxValue = Convert.ToDecimal(intialtax);
               // if (!string.IsNullOrEmpty(hdnOriginalAmountAfterStatementReconciliation.Value))
               // {
                   // if (Convert.ToDecimal(hdnOriginalAmountAfterStatementReconciliation.Value) > 0)
                   // {
                    //    Total = Convert.ToDecimal(hdnOriginalAmountAfterStatementReconciliation.Value);
                   // }
               // }
               // else
               // {
                    Total = Convert.ToDecimal(hdnOriginalAmountBasedOnCreditnDebit.Value);
               // }

                if (isPercentage)
                    return String.Format("{0:0.00}", Convert.ToDecimal((Convert.ToDecimal(Total) * Convert.ToDecimal(IntialtaxValue)) / 100));
                else
                    return String.Format("{0:0.00}", Convert.ToDecimal((Convert.ToDecimal(IntialtaxValue) * 100) / Convert.ToDecimal(Total)));
            }
            return string.Empty;
        }
        #endregion
        #endregion

        #region GST Tax Calculation Events
        protected void txtGSTPerc_TextChanged(object sender, EventArgs e)
        {
            txtGSTAmt.Text = AutoCalculatePercentage(txtGSTPerc.Text, true);
            CalculateActualTotal();
        }

        protected void txtGSTAmt_TextChanged(object sender, EventArgs e)
        {
            txtGSTPerc.Text = AutoCalculatePercentage(txtGSTAmt.Text, false);
            CalculateActualTotal();
        }
        #endregion

        #region QST Tax Calculation Events
        protected void txtQSTPerc_TextChanged(object sender, EventArgs e)
        {
            txtQSTAmt.Text = AutoCalculatePercentage(txtQSTPerc.Text, true);
            CalculateActualTotal();
        }

        protected void txtQSTAmt_TextChanged(object sender, EventArgs e)
        {
            txtQSTPerc.Text = AutoCalculatePercentage(txtQSTAmt.Text, false);
            CalculateActualTotal();
        }
        #endregion

        #region HST Tax Calculation Events
        protected void txtHSTPerc_TextChanged(object sender, EventArgs e)
        {
            txtHSTAmt.Text = AutoCalculatePercentage(txtHSTPerc.Text, true);
            CalculateActualTotal();
        }

        protected void txtHSTAmt_TextChanged(object sender, EventArgs e)
        {
            txtHSTPerc.Text = AutoCalculatePercentage(txtHSTAmt.Text, false);
            CalculateActualTotal();
        }
        #endregion

        #region PST Tax Calculation Events
        protected void txtPSTPerc_TextChanged(object sender, EventArgs e)
        {
            txtPSTAmt.Text = AutoCalculatePercentage(txtPSTPerc.Text, true);
            CalculateActualTotal();
        }

        protected void txtPSTAmt_TextChanged(object sender, EventArgs e)
        {
            txtPSTPerc.Text = AutoCalculatePercentage(txtPSTAmt.Text, false);
            CalculateActualTotal();
        }
        #endregion

        #region Business Percentage Events
        protected void txtBusinessPercentage_TextChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtBusinessPercentage.Text))
            {
                decimal actualPercentage = Convert.ToDecimal(txtBusinessPercentage.Text);
                if (actualPercentage >= 0 && actualPercentage <= 100)
                {
                    CalculateActualTotal();
                   
                }
                else
                {
                    errorDiv.Visible = true;
                    customerrorSummary.InnerText = "Please enter a valid percentage";
                    btnSaveStatement.Enabled = false;
                }
            }
        }
        #endregion

        protected void btnRejectStatement_Click(object sender, EventArgs e)
        {
            if (ddlClassification.SelectedIndex > 0)
            {
                using (var context = new KFentities())
                {
                    int StatementId = Convert.ToInt32(hdnStatementId.Value);
                    var bankStatementData = context.BankExpenses.Where(s => s.Id == StatementId).FirstOrDefault();
                    bankStatementData.Purpose = !string.IsNullOrEmpty(txtPurpose.Text) ? txtPurpose.Text : string.Empty;
                    bankStatementData.Comments = !string.IsNullOrEmpty(txtComment.Text) ? txtComment.Text : string.Empty;
                    bankStatementData.ClassificationId = Convert.ToInt32(ddlClassification.SelectedValue);
                    bankStatementData.CategoryId = context.Classifications.Where(d => d.Id == bankStatementData.ClassificationId).Select(f => f.CategoryId).FirstOrDefault();
                    bankStatementData.StatusId = 3;
                    context.SaveChanges();
                    statementStatusDiv.Attributes.Add("class", "alert alert-alert col-sm-12 text-center");
                    statusText.InnerText = "Statement Rejected.";
                    IsUnsavedChanges = true;
                }
            }

            else
            {
                errorDiv.Visible = true;
                customerrorSummary.InnerText = "Please Select Classification.";
            }
        }

    }

    public class ClassificationList
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public int ClassificationId { get; set; }
        public string ClassificationName { get; set; }
    }
}