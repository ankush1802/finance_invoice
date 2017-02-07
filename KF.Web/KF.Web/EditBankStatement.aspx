<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="EditBankStatement.aspx.cs" Inherits="KF.Web.EditBankStatement" %>

<%@ Register TagPrefix="customControl" Namespace="GroupDropDownList" Assembly="GroupDropDownList" %>
<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link href="https://maxcdn.bootstrapcdn.com/bootstrap/3.3.7/css/bootstrap.min.css" rel="stylesheet" />
    <style>
        .custom-tax-form-control {
            <!-- display: block; -->
            max-width: 30% !important;
            height: 34px;
            padding: 6px 12px;
            font-size: 14px;
            line-height: 1.42857143;
            color: #555;
            background-color: #fff;
            background-image: none;
            border: 1px solid #ccc;
            border-radius: 4px;
        }

            .custom-tax-form-control[disabled], .custom-tax-form-control[readonly] {
                background-color: #eee;
                opacity: 1;
            }

        .custom-tax-input-group-addon {
            padding: 6px 12px;
            font-size: 14px;
            font-weight: 400;
            line-height: 1;
            color: #555;
            text-align: center;
            background-color: #eee;
            border: 1px solid #ccc;
            border-radius: 4px;
        }
    </style>
    <link href="ThemeAssets/css/popUpCloud.css" rel="stylesheet" />
    <script src="https://code.jquery.com/jquery-1.10.2.js"></script>
    <script src="ThemeAssets/js/bootstrap.min.js"></script>

    <!--Cloud Store Script-->

    <script type="text/jscript">
        $(document).ready(function () {
            $("#KippinCloudStore").load('/Finance/Accounting/KippinCloudHome');

            //OpenKippinCloudImage
            $(document).on("click", ".cloudGetYearFolders", function (e) {
                e.preventDefault();
                $("#KippinCloudStore").html("");
                // $("#KippinCloudStore").load("/Kippin/Finance/Kippin/KippinCloudYearListing/"+$(this).attr("data-userId"));
                $("#KippinCloudStore").load("/Finance/Accounting/KippinCloudYearListing/" + $(this).attr("data-userId"));
            });
            //Backbtn
            $(document).on("click", ".btnYearBack", function (e) {
                e.preventDefault();
                $("#KippinCloudStore").html("");
                //$("#KippinCloudStore").load("/Kippin/Finance/Kippin/KippinCloudHome");
                $("#KippinCloudStore").load("/Finance/Accounting/KippinCloudHome");
            });


            //GetCloudMonths
            $(document).on("click", ".GetCloudMonths", function (e) {
                e.preventDefault();
                $("#KippinCloudStore").html("");
                // $("#KippinCloudStore").load("/Kippin/Finance/Kippin/KippinCloudMonthListing?userId="+$(this).attr("data-UserId") +"&year="+$(this).attr("data-Year"));
                $("#KippinCloudStore").load("/Finance/Accounting/KippinCloudMonthListing?userId=" + $(this).attr("data-UserId") + "&year=" + $(this).attr("data-Year"));
            });
            //btnMonthBack
            $(document).on("click", ".btnMonthBack", function (e) {
                e.preventDefault();
                $("#KippinCloudStore").html("");
                // $("#KippinCloudStore").load("/Kippin/Finance/Kippin/KippinCloudYearListing/"+$(this).attr("data-userid"));
                $("#KippinCloudStore").load("/Finance/Accounting/KippinCloudYearListing/" + $(this).attr("data-userid"));
            });
            //GetCloudImages
            $(document).on("click", ".GetCloudImages", function (e) {
                e.preventDefault();
                $("#KippinCloudStore").html("");
                //$("#KippinCloudStore").load("/Kippin/Finance/Kippin/KippinCloudImageListing?userId="+$(this).attr("data-userid") +"&year="+$(this).attr("data-year")+"&month="+$(this).attr("data-month"));
                $("#KippinCloudStore").load("/Finance/Accounting/KippinCloudImageListing?userId=" + $(this).attr("data-userid") + "&year=" + $(this).attr("data-year") + "&month=" + $(this).attr("data-month"));
            });
            $(document).on("click", ".btnImageBack", function (e) {
                e.preventDefault();
                $("#KippinCloudStore").html("");
                // $("#KippinCloudStore").load("/Kippin/Finance/Kippin/KippinCloudMonthListing?userId="+$(this).attr("data-userid") +"&year="+$(this).attr("data-year"));
                $("#KippinCloudStore").load("/Finance/Accounting/KippinCloudMonthListing?userId=" + $(this).attr("data-userid") + "&year=" + $(this).attr("data-year"));
            });
        });

    </script>

    <!--View uploaded Bills From Cloud Store Script-->
    <script>
        $(document).ready(function () {
            $(document).on("click", ".uploadedBills", function (e) {
                e.preventDefault();
                $("#uploadBillImages").load("/Finance/Accounting/ViewStatementUploadedBills?userId=" + $(this).attr("data-UserId") + "&statementId=" + $(this).attr("data-statementId"));
                $('#uploadedBillsModalPopUp').modal({ backdrop: 'static', keyboard: false })
                $('#uploadedBillsModalPopUp').modal('show');

            });
        });

    </script>
    <!--View Image From Cloud Store Script-->
    <script>
        $(document).ready(function () {
            $(document).on("click", ".OpenKippinCloudImage", function (e) {
                e.preventDefault();
                $("#imgSelectedImage").attr('src', ".." + $(this).attr('data-imagesource'));
                $("#btnUploadSelectedCloudImage").attr('data-imagesource', $(this).attr('data-imagesource'));
                $('#kippinStorePopUpImages').modal({ backdrop: 'static', keyboard: false })
                $('#kippinStorePopUpImages').modal('show');

            });
        });

    </script>

    <!-- uploaded Bills From Device Script-->
    <script>
        $(document).ready(function () {
            $("#btnbillupload").click(function () {
                var formData = new FormData();
                formData.append("BankId", $(this).attr("data-BankId"));
                formData.append("StatementId", $(this).attr("data-StatementId"));
                formData.append("UserId", $(this).attr("data-UserId"));
                var totalFiles = document.getElementById("file").files.length;
                if (totalFiles > 0) {
                    for (var i = 0; i < totalFiles; i++) {
                        var file = document.getElementById("file").files[i];
                        formData.append("file", file);
                    }
                    $.ajax({
                        type: "POST",
                        // url: '/Kippin/Finance/Kippin/UploadBill',
                        url: '/Finance/Accounting/UploadBill',
                        data: formData,
                        dataType: 'json',
                        contentType: false,
                        processData: false,
                        success: function (response) {
                            //Here id is BankExpenseId
                            alert('Bill Successfully Uploaded!!');
                        },
                        error: function (error) {
                            alert("Error while uploading bill");
                        }
                    });
                }
                else {
                    alert("Please select the image first.");
                }

            });
        });

    </script>

    <!-- delete Bills From statement bill list Script-->
    <script>
        $(document).ready(function () {
            $(document).on("click", ".activeBillImage", function (e) {
                e.preventDefault();
                //fire ajax call to delete the bill and rebind the parial view
                var formData = new FormData();
                formData.append("StatementId", $(this).attr("data-StatementId"));
                formData.append("UserId", $(this).attr("data-UserId"));
                formData.append("FilePath", $(this).attr("data-url").replace("/Finance/", "/"));
                // alert($(this).attr("data-url").replace("/Finance/", "/"));
                $.ajax({
                    type: "POST",
                    // url: '/Kippin/Finance/Kippin/DeleteImageFromCloud',
                    url: '/Finance/Accounting/DeleteImageFromCloud',
                    data: formData,
                    dataType: 'json',
                    contentType: false,
                    processData: false,
                    success: function (response) {
                        //Here id is BankExpenseId
                        alert('Bill Successfully Removed !!');
                        $("#uploadBillImages").load("/Finance/Accounting/ViewStatementUploadedBills?userId=" + $(this).attr("data-UserId") + "&statementId=" + $(this).attr("data-statementId"));
                    },
                    error: function (error) {
                        alert("Error while removing bill from cloud.");
                    }
                });
            });
        });

    </script>

    <!-- upload Bills From cloud list Script-->
    <script>
        $(document).ready(function () {
            //uploadCloudImage
            $(document).on("click", ".uploadCloudImage", function (e) {
                e.preventDefault();
                var imagePath = $(this).attr("data-imagesource");
                if (imagePath != null) {
                    //$("#hdnSelectedImagePath").val(imagePath)
                    var formData = new FormData();
                    formData.append("BankId", $(this).attr("data-bankid"));
                    formData.append("StatementId", $(this).attr("data-statementid"));
                    formData.append("UserId", $(this).attr("data-userid"));
                    formData.append("FilePath", imagePath);
                    $.ajax({
                        type: "POST",
                        url: '/Finance/Accounting/UploadImageFromCloud',
                        data: formData,
                        dataType: 'json',
                        contentType: false,
                        processData: false,
                        success: function (response) {
                            //Here id is BankExpenseId
                            alert('Image Successfully Uploaded!!');
                            //  $("#uploadBillImages").html("");
                            // $("#uploadBillImages").load("/Finance/Accounting/ViewStatementUploadedBills?userId=" + $(this).attr("data-UserId") + "&statementId=" + $(this).attr("data-StatementId"));
                        },
                        error: function (error) {
                            alert("Error while uploading Image");
                        }
                    });
                }
            });
        });

    </script>

    <script>
        $(function () {
            $(".validPercentage").keydown(function (event) {


                if (event.shiftKey == true) {
                    event.preventDefault();
                }

                if ((event.keyCode >= 48 && event.keyCode <= 57) || (event.keyCode >= 96 && event.keyCode <= 105) || event.keyCode == 8 || event.keyCode == 9 || event.keyCode == 37 || event.keyCode == 39 || event.keyCode == 46 || event.keyCode == 190) {

                } else {
                    event.preventDefault();
                }

                if ($(this).val().indexOf('.') !== -1 && event.keyCode == 190)
                    event.preventDefault();

            });
        });
       
    </script>
</head>
<body>
    <form id="form1" runat="server">


        <div class="col-md-12" style="margin-top: 10px; margin-bottom: 10px">
            <div class="col-md-6 pull-left">
                <asp:Button ID="btnPrevious" CssClass="btn btn-primary" OnClick="btnPrevious_Click" runat="server" Text="Previous" />
            </div>
            <div class="col-md-6">
                <asp:Button ID="btnNext" CssClass="pull-right btn btn-primary" OnClick="btnNext_Click" runat="server" Text="Next" />
            </div>
        </div>
        <div runat="server" id="statementStatusDiv" class="">
            <p runat="server" id="statusText"></p>
        </div>

        <div runat="server" visible="false" id="UnsavedMessaggeDiv" class="alert alert-danger col-sm-12 text-center">
            <p>Please save your data before proceeding or data will be lost.</p>
        </div>
        <div runat="server" visible="false" id="errorDiv" class="alert alert-danger col-sm-12 text-center">
            <p runat="server" id="customerrorSummary"></p>
        </div>
        <asp:HiddenField ID="hdnStatementId" runat="server" />
        <asp:HiddenField ID="hdnOriginalAmountBasedOnCreditnDebit" runat="server" />
        <asp:HiddenField ID="hdnOriginalAmountAfterStatementReconciliation" runat="server" />

        <div class="col-md-6">
            <h3 class="text-info text-center">Bank Data</h3>
            <table class="table table-bordered table-responsive jumbotron">
                <tbody>
                    <tr>
                        <td>
                            <label for="txtCategory">Category :</label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtCategory" Enabled="false" CssClass="form-control" runat="server"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <label for="txtDescription">Description:</label>

                        </td>
                        <td>
                            <asp:TextBox ID="txtDescription" Enabled="false" CssClass="form-control" runat="server"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <label for="txtPurpose">Purpose:</label>

                        </td>
                        <td>
                            <asp:TextBox ID="txtPurpose" CssClass="form-control" runat="server"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <label for="title" class="control-label">Credit:</label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtCredit" CssClass="form-control" runat="server"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <label for="title" class="control-label">Debit:</label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtDebit" CssClass="form-control" runat="server"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <label for="ddlClassification">Classification</label>
                        </td>
                        <td>
                            <customControl:GroupDropDownList ID="ddlClassification" CssClass="form-control" AutoPostBack="true" OnSelectedIndexChanged="ddlClassification_SelectedIndexChanged" runat="server"></customControl:GroupDropDownList>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <label for="title" class="control-label">Classification Description:</label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtClassificationDescription" CssClass="form-control" runat="server"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <label for="title" class="control-label">Date:</label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtDate" Enabled="false" CssClass="form-control" runat="server"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <label for="title" class="control-label">Comment:</label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtComment" CssClass="form-control" runat="server"></asp:TextBox>
                        </td>
                    </tr>
                </tbody>
            </table>


        </div>

        <div class="col-md-6">
            <h3 class="text-info text-center">Bill Data</h3>
            <table class="table table-bordered">
                <tbody>
                    <tr>
                        <td><strong>Amount : </strong></td>
                        <td>
                            <asp:TextBox ID="txtCalculatedAmount" ReadOnly="true" CssClass="form-control" runat="server"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td><strong>Business Percentage: </strong></td>
                        <td>
                            <asp:TextBox ID="txtBusinessPercentage" OnTextChanged="txtBusinessPercentage_TextChanged" Text="0" AutoPostBack="true" CssClass="form-control validPercentage" runat="server"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td><strong>GST : </strong></td>
                        <td>
                            <asp:TextBox ID="txtGSTPerc" Text="0" OnTextChanged="txtGSTPerc_TextChanged" Width="30%" AutoPostBack="true" CssClass="custom-tax-form-control validPercentage" runat="server"></asp:TextBox>
                            <span style="width: 0%; display: grid" class="custom-tax-input-group-addon">%</span>
                            <asp:TextBox ID="txtGSTAmt" Text="0" OnTextChanged="txtGSTAmt_TextChanged" Width="30%" AutoPostBack="true" CssClass="custom-tax-form-control validPercentage" runat="server"></asp:TextBox>
                            <span style="width: 0%; display: grid" class="custom-tax-input-group-addon">Amt</span>
                        </td>
                    </tr>
                    <tr>
                        <td><strong>QST : </strong></td>
                        <td>
                            <asp:TextBox ID="txtQSTPerc" Text="0" OnTextChanged="txtQSTPerc_TextChanged" Width="30%" AutoPostBack="true" CssClass="custom-tax-form-control validPercentage" runat="server"></asp:TextBox>
                            <span style="width: 0%; display: grid" class="custom-tax-input-group-addon">%</span>
                            <asp:TextBox ID="txtQSTAmt" Text="0" OnTextChanged="txtQSTAmt_TextChanged" Width="30%" AutoPostBack="true" CssClass="custom-tax-form-control validPercentage" runat="server"></asp:TextBox>
                            <span style="width: 0%; display: grid" class="custom-tax-input-group-addon">Amt</span>
                        </td>
                    </tr>
                    <tr>
                        <td><strong>HST : </strong></td>
                        <td>
                            <asp:TextBox ID="txtHSTPerc" Text="0" OnTextChanged="txtHSTPerc_TextChanged" Width="30%" AutoPostBack="true" CssClass="custom-tax-form-control validPercentage" runat="server"></asp:TextBox>
                            <span style="width: 0%; display: grid" class="custom-tax-input-group-addon">%</span>
                            <asp:TextBox ID="txtHSTAmt" Text="0" OnTextChanged="txtHSTAmt_TextChanged" Width="30%" AutoPostBack="true" CssClass="custom-tax-form-control validPercentage" runat="server"></asp:TextBox>
                            <span style="width: 0%; display: grid" class="custom-tax-input-group-addon">Amt</span>
                        </td>
                    </tr>
                    <tr>
                        <td><strong>PST : </strong></td>
                        <td>
                            <asp:TextBox ID="txtPSTPerc" Text="0" OnTextChanged="txtPSTPerc_TextChanged" Width="30%" AutoPostBack="true" CssClass="custom-tax-form-control validPercentage" runat="server"></asp:TextBox>
                            <span style="width: 0%; display: grid" class="custom-tax-input-group-addon">%</span>
                            <asp:TextBox ID="txtPSTAmt" Text="0" OnTextChanged="txtPSTAmt_TextChanged" Width="30%" AutoPostBack="true" CssClass="custom-tax-form-control validPercentage" runat="server"></asp:TextBox>
                            <span style="width: 0%; display: grid" class="custom-tax-input-group-addon">Amt</span>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <input type="file" runat="server" style="max-width: 150px;" name="file" id="file" />
                        </td>
                        <td>
                            <a class="btn btn-info" runat="server" id="btnbillupload">Attach Bill</a>
                            <a class="btn btn-info" data-toggle="modal" data-target="#kippinStore">Upload From Store</a>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Button ID="btnSaveStatement" CssClass="btn btn-info pull-left btnSaveStatement" OnClick="btnSaveStatement_Click" runat="server" Text="Save" />
                        </td>
                        <td>
                            <%--<a class="btn btn-info" data-toggle="modal" data-target="#uploadedBills">View Uploaded Bill's</a>--%>
                            <a runat="server" id="viewUploadedBillsDiv" class="btn btn-info uploadedBills">View Uploaded Bill's</a>
                            <asp:Button ID="btnRejectStatement" CssClass="btn btn-info" OnClick="btnRejectStatement_Click" runat="server" Text="Reject" />
                        </td>
                    </tr>
                </tbody>
            </table>
        </div>

        <!--View Uploaded bills-->
        <div id="uploadedBillsModalPopUp" class="modal fade" tabindex="-1" role="dialog">
            <div class="modal-dialog">
                <div class="modal-content">
                    <div class="modal-header">
                        <button type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
                        <h4 class="modal-title">Uploaded Bills</h4>
                    </div>
                    <div id="uploadBillImages" class="modal-body">
                    </div>
                    <div class="modal-footer">
                        <button type="button" class="btn btn-danger" data-dismiss="modal">Close</button>
                    </div>
                </div>
                <!-- /.modal-content -->
            </div>
            <!-- /.modal-dialog -->
        </div>

        <!--Cloud-->
        <div id="kippinStore" class="modal fade" tabindex="-1" data-backdrop="static" data-keyboard="false" role="dialog">
            <div class="modal-dialog">
                <div class="modal-content">
                    <div class="modal-header">
                        <button type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
                        <h4 class="modal-title">Kippin Store</h4>
                    </div>
                    <div id="KippinCloudStore" class="modal-body">
                    </div>
                    <div class="modal-footer">
                        <button type="button" class="btn btn-danger" data-dismiss="modal">Close</button>
                    </div>
                </div>
                <!-- /.modal-content -->
            </div>
            <!-- /.modal-dialog -->
        </div>

        <!--kippinStorePopUpImages-->
        <div id="kippinStorePopUpImages" class="modal fade" tabindex="-1" role="dialog">
            <div class="modal-dialog">
                <div class="modal-content">
                    <div class="modal-header">
                        <button type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
                        <h4 class="modal-title">Kippin Store Image</h4>
                    </div>
                    <div id="KippinCloudStoreImages" class="modal-body">
                        <img id="imgSelectedImage" class="img-responsive" alt="" src="" />
                    </div>
                    <div class="modal-footer">
                        <button type="button" id="btnUploadSelectedCloudImage" runat="server" class="btn btn-success uploadCloudImage" data-imagesource="">Upload Image</button>
                        <button type="button" class="btn btn-danger" data-dismiss="modal">Close</button>
                    </div>
                </div>
                <!-- /.modal-content -->
            </div>
            <!-- /.modal-dialog -->
        </div>
    </form>
</body>
</html>
