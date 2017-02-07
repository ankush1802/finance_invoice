<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="UpdateClassification.aspx.cs" Inherits="KF.Web.UpdateClassification" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link href="ThemeAssets/css/bootstrap.min.css" rel="stylesheet" />
    <style>
        .custom-required {
            color: red;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
       <!--A-Type classification-->
        <div runat="server" id="AtypeClassificationSection" visible="false" class="panel panel-info">
            <div class="panel-heading">
                <div class="panel-title text-center">Update Sub-Account(A)-Subsidiary Account</div>
            </div>
            <div class="panel-body">
                <div class="row">
                    <div class="col-md-3">
                        <div class="form-group">
                            <label for="name" class="control-label">Name of Account</label>
                            <span class="form-required-field">*</span>
                            <asp:TextBox ID="txtAtypeClassificationName" CssClass="form-control" runat="server"></asp:TextBox>
                            <span class="custom-required" runat="server" id="reqAtypeClassificationValidationName"></span>
                        </div>
                    </div>
                    <div class="col-md-3">
                        <div class="form-group">
                            <label for="firstname" class="control-label">Sub-Account(A)Description </label>
                            <span class="form-required-field">*</span>
                            <asp:TextBox ID="txtAtypeClassificationSubAccountDesc" CssClass="form-control" runat="server"></asp:TextBox>
                            <span class="custom-required" runat="server" id="reqAtypeClassificationValidationSubAccount"></span>
                        </div>
                    </div>
                    <div class="col-md-3">
                        <div class="form-group">
                            <label for="email" class="control-label">Insert Account No.</label>
                            <span class="form-required-field">*</span>
                            <asp:TextBox ID="txtAtypeClassificationValidationAccNumber" OnTextChanged="txtAtypeClassificationValidationAccNumber_TextChanged" AutoPostBack="true" CssClass="form-control" runat="server"></asp:TextBox>
                            <span class="custom-required" runat="server" id="reqAtypeClassificationValidationAccNumber"></span>
                        </div>
                    </div>
                    <asp:HiddenField ID="hdnAtypePrefix" Value="" runat="server" />
                    <div class="col-md-3">
                        <div class="form-group">
                            <label for="email" class="control-label">Range of Acct #</label>
                            <span class="form-required-field">*</span>
                            <asp:TextBox ID="txtAtypeClassificationRangeAcct" CssClass="form-control" Enabled="false" runat="server"></asp:TextBox>
                            <span class="custom-required" runat="server" id="reqAtypeClassificationValidationRangeAcct"></span>
                        </div>
                    </div>
                    <div class="col-md-3">
                        <div class="form-group">
                            <label for="email" class="control-label">New Account#</label>
                            <span class="form-required-field">*</span>
                            <asp:TextBox ID="txtAtypeClassificationNewAccount" CssClass="form-control" Enabled="false" runat="server"></asp:TextBox>
                            <span class="custom-required" runat="server" id="reqAtypeClassificationValidationNewAccount"></span>
                        </div>
                    </div>
                    <div class="col-md-3">
                        <div class="form-group">
                            <label for="lastname" class="control-label">Sub-Totals (S)</label>
                            <span class="form-required-field">*</span>
                            <asp:DropDownList ID="ddlAtypeClassificationSubTotalList" AutoPostBack="true" OnSelectedIndexChanged="ddlAtypeClassificationSubTotalList_SelectedIndexChanged" CssClass="form-control" runat="server"></asp:DropDownList>
                            <span class="custom-required" runat="server" id="reqAtypeClassificationValidationSubTotalList"></span>
                        </div>
                    </div>
                </div>
                <div runat="server" visible="false" id="AtypeErrorDiv" class="alert alert-danger col-sm-12 text-center">
                    <p runat="server" id="AtypeErrorMessage"></p>
                </div>
                <div runat="server" visible="false" id="AtypeSuccess" class="alert alert-success col-sm-12 text-center">
                    <p> Classification successfully saved. </p>
                </div>
                <div class="clearfix">
                    <div class="pull-right">
                        <asp:Button ID="btnAtypeClassification" OnClick="btnAtypeClassification_Click" Enabled="false" CssClass="btn btn-primary" runat="server" Text="Update A Classification" />
                    </div>
                </div>
            </div>
        </div>
           <!--S-Type classification-->
        <div class="panel panel-info" runat="server" id="StypeClassificationSection" visible="false">
            <div class="panel-heading">
                <div class="panel-title text-center">Update Sub-Total(S)-Account has subsidiaries</div>
            </div>
            <div class="panel-body">
                <div class="row">
                    <div class="col-md-3">
                        <div class="form-group">
                            <label for="name" class="control-label">Name of Account</label>
                            <span class="form-required-field">*</span>
                            <asp:TextBox ID="txtStypeClassificationName" CssClass="form-control" runat="server"></asp:TextBox>
                            <span class="custom-required" runat="server" id="reqStypeClassificationName"></span>
                        </div>
                    </div>
                    <div class="col-md-3">
                        <div class="form-group">
                            <label for="firstname" class="control-label">Sub-Account(A)Description </label>
                            <span class="form-required-field">*</span>
                            <asp:TextBox ID="txtStypeClassificationDesc" CssClass="form-control" runat="server"></asp:TextBox>
                            <span class="custom-required" runat="server" id="reqStypeClassificationDesc"></span>
                        </div>
                    </div>
                    <div class="col-md-3">
                        <div class="form-group">
                            <label for="email" class="control-label">Insert Account No.</label>
                            <span class="form-required-field">*</span>
                            <asp:TextBox ID="txtStypeChartAccountNumber" OnTextChanged="txtStypeChartAccountNumber_TextChanged" AutoPostBack="true" CssClass="form-control" runat="server"></asp:TextBox>
                            <span class="custom-required" runat="server" id="Span3"></span>
                        </div>
                    </div>
                    <asp:HiddenField ID="hdnStypePrefix" Value="" runat="server" />
                     <asp:HiddenField ID="hdnSTypeMasterClassificationSubTotalDisplayNumber" Value="" runat="server" />
                    <div class="col-md-3">
                        <div class="form-group">
                            <label for="email" class="control-label">Range of Acct #</label>
                            <span class="form-required-field">*</span>
                            <asp:TextBox ID="txtStypeRange" CssClass="form-control" Enabled="false" runat="server"></asp:TextBox>
                            <span class="custom-required" runat="server" id="reqStypeRange"></span>
                        </div>
                    </div>
                    <div class="col-md-3">
                        <div class="form-group">
                            <label for="email" class="control-label">New Account#</label>
                            <span class="form-required-field">*</span>
                            <asp:TextBox ID="txtStypeChartAccountRange" CssClass="form-control" Enabled="false" runat="server"></asp:TextBox>
                            <span class="custom-required" runat="server" id="reqStypeChartAccountRange"></span>
                        </div>
                    </div>
                    <div class="col-md-3">
                        <div class="form-group">
                            <label for="lastname" class="control-label">Sub-Totals (T)</label>
                            <span class="form-required-field">*</span>
                            <asp:DropDownList ID="ddlStypeClassificationSubTotalList" AutoPostBack="true" OnSelectedIndexChanged="ddlStypeClassificationSubTotalList_SelectedIndexChanged" CssClass="form-control" runat="server"></asp:DropDownList>
                            <span class="custom-required" runat="server" id="reqddlStypeClassificationSubTotalList"></span>
                        </div>
                    </div>
                </div>
                <div runat="server" visible="false" id="StypeErrorDiv" class="alert alert-danger col-sm-12 text-center">
                    <p runat="server" id="StypeErrorMessage"></p>
                </div>
                <div runat="server" visible="false" id="StypeSuccess" class="alert alert-success col-sm-12 text-center">
                    <p> Classification successfully saved. </p>
                </div>
                <div class="clearfix">
                    <div class="pull-right">
                        <asp:Button ID="btnStypeClassification" OnClick="btnStypeClassification_Click" Enabled="false" CssClass="btn btn-primary" runat="server" Text="Update S Classification" />
                    </div>
                </div>
            </div>
        </div>
        
           <!--G-Type classification-->
         <div id="GtypeClassificationSection" runat="server" visible="false" class="panel panel-info">
            <div class="panel-heading">
                <div class="panel-title text-center">Update Accounts (G) - Account has no subsidiaries</div>
            </div>
            <div class="panel-body">
                <div class="row">
                    <div class="col-md-3">
                        <div class="form-group">
                            <label for="name" class="control-label">Name of Account</label>
                            <span class="form-required-field">*</span>
                            <asp:TextBox ID="txtGtypeClassificationName" CssClass="form-control" runat="server"></asp:TextBox>
                        </div>
                    </div>
                    <div class="col-md-3">
                        <div class="form-group">
                            <label for="firstname" class="control-label">Account Name(G)Description</label>
                            <span class="form-required-field">*</span>
                            <asp:TextBox ID="txtGtypeClassificationDesc" CssClass="form-control" runat="server"></asp:TextBox>
                        </div>
                    </div>
                    <div class="col-md-3">
                        <div class="form-group">
                            <label for="email" class="control-label">Insert Account No.</label>
                            <span class="form-required-field">*</span>
                            <asp:TextBox ID="txtGtypeChartAccountNumber" OnTextChanged="txtGtypeChartAccountNumber_TextChanged" AutoPostBack="true" CssClass="form-control" runat="server"></asp:TextBox>
                        </div>
                    </div>
                    <asp:HiddenField ID="hdnGtypePrefix" Value="" runat="server" />
                    <div class="col-md-3">
                        <div class="form-group">
                            <label for="email" class="control-label">Range of Acct #</label>
                            <span class="form-required-field">*</span>
                            <asp:TextBox ID="txtGtypeRange" CssClass="form-control" Enabled="false" runat="server"></asp:TextBox>
                        </div>
                    </div>
                    <div class="col-md-3">
                        <div class="form-group">
                            <label for="email" class="control-label">New Account#</label>
                            <span class="form-required-field">*</span>
                            <asp:TextBox ID="txtGtypeChartAccountDisplayNumber" CssClass="form-control" Enabled="false" runat="server"></asp:TextBox>
                        </div>
                    </div>
                    <div class="col-md-3">
                        <div class="form-group">
                            <label for="lastname" class="control-label">Sub-Totals (T)</label>
                            <span class="form-required-field">*</span>
                            <asp:DropDownList ID="ddlGtypeClassificationSubTotalList" AutoPostBack="true" OnSelectedIndexChanged="ddlGtypeClassificationSubTotalList_SelectedIndexChanged" CssClass="form-control" runat="server"></asp:DropDownList>
                        </div>
                    </div>
                </div>
                <div runat="server" visible="false" id="GtypeErrorDiv" class="alert alert-danger col-sm-12 text-center">
                    <p runat="server" id="GtypeErrorMessage"></p>
                </div>
                <div runat="server" visible="false" id="GtypeSuccess" class="alert alert-success col-sm-12 text-center">
                    <p> Classification successfully saved. </p>
                </div>
                <div class="clearfix">
                    <div class="pull-right">
                        <asp:Button ID="btnGtypeClassification" OnClick="btnGtypeClassification_Click" Enabled="false" CssClass="btn btn-primary" runat="server" Text="Update G Classification" />
                    </div>
                </div>
            </div>
        </div>
    </form>
</body>
</html>
