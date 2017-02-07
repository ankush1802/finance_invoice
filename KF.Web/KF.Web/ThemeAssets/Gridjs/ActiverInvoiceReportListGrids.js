var ActiverInvoiceReportListGrid;
$(document).ready(function () {
    ActiverInvoiceReportListGrid = $("#ActiverInvoiceReportListGrid").grid({
        dataKey: "Id",
        uiLibrary: "bootstrap",

        columns: [

            { field: "InvoiceNumber", title: "Invoice No", sortable: false, width: 100 },
            { field: "InvoiceDate", title: "Invoice Date", sortable: false, width: 100 },
            { field: "FirstName", title: "Customer", sortable: false, width: 100 },
           { field: "Username", title: "Supplier", sortable: false, width: 100 },
           { field: "StrType", title: "Type Of Inv.", sortable: false, width: 200 },
           { field: "InvoiceJVID", title: "Inv. JVID", sortable: false, width: 100 },
               { field: "StripeJVID", title: "PayJVID", sortable: false, width: 100 },
                { field: "Total", title: "Inv. Amt", sortable: false, width: 100 },
                    { field: "DepositePayment", title: "Paid Amt", sortable: false, width: 150 },
                        { field: "BalanceDue", title: "Outstanding Balance", sortable: false, width: 200 },
                            { field: "DueDate", title: "Inv. Age", sortable: false, width: 100 },
                                { field: "In_R_FlowStatus", title: "Flow Status", sortable: false, width: 100 },
                                    { field: "Pro_Status", title: "Status", sortable: false, width: 100 },
                                        { field: "MethodOfPayment", title: "Method of Payment", sortable: false, width: 100 },
                                            { field: "", title: "Pay INV.", sortable: false, width: 100 },
                                                { field: "", title: "Insert Amount to be paid", sortable: false, width: 200 },
                                                    { field: "", title: "Insert Pay JVID#", sortable: false, width: 200 },
                                                     { title: "Manual Payment", field: "Update", width: 250, type: "icon", tooltip: "Edit", events: { "click": editEmployee } },
        { field: "CustomerId", sortable: false, css: "hide" },
        { field: "Id", sortable: false, css: "hide" },
        { field: "IsCustomer", sortable: false, css: "hide" },
 { title: "Stripe Payment", field: "Update", width: 250, type: "icon", tooltip: "Edit", events: { "click": stripePayment } },
  { field: "IsStripe", sortable: false, css: "hide" },
  { field: "UserId", sortable: false, css: "hide" },
    { title: "Cancel", field: "Update", type: "icon", tooltip: "Cancel Invoice" },
        ],
        pager: { enable: true, limit: 5, sizes: [2, 5, 10, 20] }

    });
});

function editEmployee(e) {

}
function SendMailButton() {
    $("#ActiverInvoiceReportListGrid tr").each(function () {

        $(this).find("th").eq(18).hide();
        $(this).find("td").eq(18).hide();

        $(this).find("th").eq(19).hide();
        $(this).find("td").eq(19).hide();

        $(this).find("th").eq(20).hide();
        $(this).find("td").eq(20).hide();

        $(this).find("th").eq(22).hide();
        $(this).find("td").eq(22).hide();

        $(this).find("th").eq(23).hide();
        $(this).find("td").eq(23).hide();

        //$(this).find("th").eq(24).hide();
        //$(this).find("td").eq(24).hide();

        var status = $(this).find("td").eq(11).find("div").attr('title');
        var status_cancel = $(this).find("td").eq(20).find("div").attr('title');
        var status_stripePayment = $(this).find("td").eq(22).find("div").attr('title');

        if (status == "Pending Payment" || status == "Partial Payment") {

            $(this).find("td").eq(15).find("div").html("<label for='amount' class='clsamtpopup' style='cursor: pointer; cursor: hand;'>Please Select</label>");
            $(this).find("td").eq(16).find("div").html("<label for='jvid'  class='clsjvidpopup' style='cursor: pointer; cursor: hand;'>Please Select</label>");

            $(this).find("td").eq(17).find("div").html("<button style='color:green;cursor: hand;' disabled class='btn btn-default clsFinalManualPayments' type='button'>Manual Payment</button>");

            if (status_stripePayment == "true") {
                $(this).find("td").eq(21).find("div").html("<button style='color:green;cursor: hand;' disabled class='btn btn-default clsModalStripe' type='button'>Stripe Payment</button>");
            }
            if (status_stripePayment == "false") {
                $(this).find("td").eq(21).find("div").html("<button style='color:green; display:none;' disabled class='btn btn-default' type='button'>Stripe Payment</button>");
            }
           // if (status_cancel == "Accepted") {
            if (status_cancel == "true") {
                $(this).find("td").eq(24).find("div").html("<button style='color:green;' class='btn btn-default clsCancel' type='button'>Cancel</button>");
            }
        }        
        if (status == "Inprogress") {
            $(this).find("td").eq(17).find("div").html("<button style='color:green;display:none;' disabled class='btn btn-default' type='button'>Manual Payment</button>");
            $(this).find("td").eq(21).find("div").html("<button style='color:green; display:none;' disabled class='btn btn-default' type='button'>Stripe Payment</button>");
        }
        if (status == "Deleted") {
            $(this).find("td").eq(17).find("div").html("<button style='color:green;display:none;' disabled class='btn btn-default' type='button'>Manual Payment</button>");
            $(this).find("td").eq(21).find("div").html("<button style='color:green; display:none;' disabled class='btn btn-default' type='button'>Stripe Payment</button>");
        }
        if (status == "Declined") {
            $(this).find("td").eq(17).find("div").html("<button style='color:green;display:none;' disabled class='btn btn-default' type='button'>Manual Payment</button>");
            $(this).find("td").eq(21).find("div").html("<button style='color:green; display:none;' disabled class='btn btn-default' type='button'>Stripe Payment</button>");
        }
        if (status == "Draft") {
            $(this).find("td").eq(17).find("div").html("<button style='color:green;display:none;' disabled class='btn btn-default' type='button'>Manual Payment</button>");
            $(this).find("td").eq(21).find("div").html("<button style='color:green; display:none;' disabled class='btn btn-default' type='button'>Stripe Payment</button>");
        }
        if (status == "Closed") {
            $(this).find("td").eq(17).find("div").html("<button style='color:green;display:none;' disabled class='btn btn-default' type='button'>Manual Payment</button>");
            $(this).find("td").eq(21).find("div").html("<button style='color:green; display:none;' disabled class='btn btn-default' type='button'>Stripe Payment</button>");
        }
        if (status == "Cancelled") {
            $(this).find("td").eq(17).find("div").html("<button style='color:green;display:none;' disabled class='btn btn-default' type='button'>Manual Payment</button>");
            $(this).find("td").eq(21).find("div").html("<button style='color:green; display:none;' disabled class='btn btn-default' type='button'>Stripe Payment</button>");
            $(this).find("td").eq(24).find("div").html("<button style='color:green;display:none;' class='btn btn-default clsCancel' type='button'>Cancel</button>");
        }
        if (status == "Approved") {
            $(this).find("td").eq(17).find("div").html("<button style='color:green;display:none;' disabled class='btn btn-default' type='button'>Manual Payment</button>");
            $(this).find("td").eq(21).find("div").html("<button style='color:green; display:none;' disabled class='btn btn-default' type='button'>Stripe Payment</button>");
        }
        if (status == "Sent") {
            $(this).find("td").eq(17).find("div").html("<button style='color:green;display:none;' disabled class='btn btn-default' type='button'>Manual Payment</button>");
            $(this).find("td").eq(21).find("div").html("<button style='color:green; display:none;' disabled class='btn btn-default' type='button'>Stripe Payment</button>");
        }
  


    });

};
$(document).on("click", ".clsamtpopup", function (e) {
    var rowIndex = $(this).closest('td').parent()[0].sectionRowIndex;
    rowIndex = rowIndex + 1;
    localStorage.setItem('RowIndex', rowIndex);
    if (($(this).closest('tr').find('td:eq(15)').text()) != 'Please Select') {
        $("#txtAmountPaid").val(($(this).closest('tr').find('td:eq(15)').text()));
    }
    $("#modalamtpopup").modal("show");
});

$(document).on("click", ".clsjvidpopup", function (e) {
    var rowJVIDIndex = $(this).closest('td').parent()[0].sectionRowIndex;
    rowJVIDIndex = rowJVIDIndex + 1;
    localStorage.setItem('RowJVIDIndex', rowJVIDIndex);
    if (($(this).closest('tr').find('td:eq(16)').text()) != 'Please Select') {
        $("#txtJVIDPaid").val(($(this).closest('tr').find('td:eq(16)').text()));
    }

    $("#modalJVIDpopup").modal("show");
});
$(document).on("click", ".clsModalManual", function (e) {

    var IsCustomer = $(this).closest('tr').find('td:eq(20)').text();
    var InvoiceId = $(this).closest('tr').find('td:eq(19)').text();
    var outstandingBal = $(this).closest('tr').find('td:eq(9)').text();
    var SupplierId = $(this).closest('tr').find('td:eq(23)').text();

    var IsCustomer = $(this).closest('tr').find('td:eq(20)').text();
    var InvoiceId = $(this).closest('tr').find('td:eq(19)').text();
    var outstandingBal = $(this).closest('tr').find('td:eq(9)').text();
    var SupplierId = $(this).closest('tr').find('td:eq(23)').text();
    $("#modalmanualpayment").modal("show");
    localStorage.setItem('IsCustomer', IsCustomer);
    localStorage.setItem('invoiceId', InvoiceId);
    localStorage.setItem('OutBal', outstandingBal);
    localStorage.setItem('supplierid', SupplierId);
});
$(document).on("click", ".clsModalStripe", function (e) {
    var SupplierId = $(this).closest('tr').find('td:eq(23)').text();
    var InvoiceId = $(this).closest('tr').find('td:eq(19)').text();
    var outstandingBal = $(this).closest('tr').find('td:eq(9)').text();
    var amountToBePaid = $(this).closest('tr').find('td:eq(15)').text();
    $("#modalStripPayment").modal("show");
    $("#txtOutstandingBal").val(outstandingBal);
    $("#txtBalAmountTobePaid").val(amountToBePaid);
    $("#txtCardNumber").val('');
    $("#txtCvv").val('');
    localStorage.setItem('SupplierId', SupplierId);
    localStorage.setItem('invoiceId', InvoiceId);
    localStorage.setItem('OutBal', outstandingBal);
});
$(document).on("click", ".clsSubmitManualPayment", function (e) {
    if ($("#txtAmountPaid").val() == '') {
        alert("Please enter amount to be paid.");
        return false;
    }
    else {
        var rowVal = localStorage.getItem("RowIndex");
        var amount = $("#txtAmountPaid").val();
        $('#ActiverInvoiceReportListGrid tr:eq(' + rowVal + ') td:eq(' + 15 + ')').find("div").find("label").text(amount);
        $("#txtAmountPaid").val('');
        $('#ActiverInvoiceReportListGrid tr:eq(' + rowVal + ') td:eq(' + 17 + ')').find("div").find("button").removeAttr('disabled');
        $('#ActiverInvoiceReportListGrid tr:eq(' + rowVal + ') td:eq(' + 21 + ')').find("div").find("button").removeAttr('disabled');
        $("#modalamtpopup").modal("hide");
    }
});
$(document).on("click", ".clsFinalManualPayments", function (e) {
    var rowVal = $(this).closest('td').parent()[0].sectionRowIndex;
    rowVal = rowVal + 1;
    var outStandingBal = $('#ActiverInvoiceReportListGrid tr:eq(' + rowVal + ') td:eq(' + 9 + ')').find("div").attr('title');
    var amountPaid = $('#ActiverInvoiceReportListGrid tr:eq(' + rowVal + ') td:eq(' + 15 + ')').find("div").find("label").text();
    var jvid = $('#ActiverInvoiceReportListGrid tr:eq(' + rowVal + ') td:eq(' + 16 + ')').find("div").find("label").text();
    var IsCustomer = $('#ActiverInvoiceReportListGrid tr:eq(' + rowVal + ') td:eq(' + 20 + ')').find("div").attr('title');
    var invoiceId = $('#ActiverInvoiceReportListGrid tr:eq(' + rowVal + ') td:eq(' + 19 + ')').find("div").attr('title');
    var supplierid = $('#ActiverInvoiceReportListGrid tr:eq(' + rowVal + ') td:eq(' + 23 + ')').find("div").attr('title');
    if (jvid == "Please Select") {
        jvid = 0;
    }
    if ((parseFloat(amountPaid) > parseFloat(outStandingBal)) || amountPaid == 0) {
        alert("Amount must be less than or equal to balance amount.");
        return false;
    }
    else {

        var dataToSend_res;
        if (IsCustomer == "false") {
            dataToSend_res = { SupplierManualPaidAmount: amountPaid, SupplierManualPaidJVID: jvid, IsCustomer: IsCustomer, InvoiceId: invoiceId };
        }
        else if (IsCustomer == "true") {
            dataToSend_res = { CustomerManualPaidAmount: amountPaid, CustomerManualPaidJVID: jvid, IsCustomer: IsCustomer, InvoiceId: invoiceId };
        }

        var dataToSend = JSON.stringify(dataToSend_res);

        $.ajax({
            type: "POST",
            url: '/Kippin/Finance/Invoice/InvoiceManualReport',
            data: dataToSend,
            dataType: 'json',
            contentType: false,
            processData: false,
            success: function (response) {
                $('#modalmanualpayment').modal('toggle');
                function doStuff() {
                    alert("Manual Payment Done Successfully.");
                    location.reload();
                }
                setTimeout(doStuff, 1000);
            },
            error: function (error) {
                $('#modalmanualpayment').modal('toggle');
                function doStuff() {
                    alert("Manual Payment Done Successfully.");
                    location.reload();
                }
                setTimeout(doStuff, 1000);

            }
        });
    }
});
$(document).on("click", ".clsSubmitJVIDPayment", function (e) {
    if ($("#txtJVIDPaid").val() == '') {
        alert("Please enter amount to be paid.");
        return false;
    }
    else {
        var rowVal = localStorage.getItem("RowJVIDIndex");
        var amount = $("#txtJVIDPaid").val();
        if (amount.length < 4) {
            alert("JVID Amount length cannot be less than 4 digits.");
            return false;
        }
        $('#ActiverInvoiceReportListGrid tr:eq(' + rowVal + ') td:eq(' + 16 + ')').find("div").find("label").text(amount);
        $("#txtJVIDPaid").val('');
        $("#modalJVIDpopup").modal("hide");
    }
});
function stripePayment() {

};
$(document).on("click", ".clsSubmitStripPayment", function () {

    if ($("#txtAmountStripPaid").val() == '' && $("#txtStripJvid").val() == '' && $("#txtCardNumber").val() == '' && $("#txtCvv").val() == '') {
        alert("Please enter credit card details.");
        return false;
    }
    else {
        var outStandingBal = localStorage.getItem("OutBal");
        if ($("#txtCardNumber").val().length < 14 || $("#txtCardNumber").val() == '') {
            alert("Length of credit card number should not be less than 14 digits.");
            return false;
        }

        //if ($("#txtCvv").val().length < 4 || $("#txtCvv").val() == "") {
        //    alert("Length of cvv number should not be less than 4 digits.");
        //    return false;
        //}
  
        if (($("#txtCvv").val().length == 3 || $("#txtCvv").val().length == 4) && ($("#txtCvv").val()!= "")) {
         
        }
        else {
            alert("Length of cvv number should not be less than 4 digits.");
            return false;
        }


        if ($("#drdMonths").val() == 0) {
            alert("Please select months.");
            return false;
        }
        if ($("#drdYears").val() == 0) {
            alert("Please select years.");
            return false;
        }
        if ((parseFloat($("#txtAmountStripPaid").val()) >= parseFloat(outStandingBal)) && $("#txtAmountStripPaid").val() == 0) {
            alert("Amount must be less than or equal to balance amount.");
            return false;
        }
        else {
            var amountPaid = $("#txtBalAmountTobePaid").val();
            var jvid = $("#txtStripJvid").val();
            var SupplierId = localStorage.getItem("SupplierId");
            var invoiceId = localStorage.getItem("invoiceId");
            var month = $("#drdMonths").val();
            var year = $("#drdYears").val();
            var creditCardNumber = $("#txtCardNumber").val();
            var cvv = $("#txtCvv").val();

            var dataToSend_res1;
            dataToSend_res1 = { SupplierId: SupplierId, InvoiceId: invoiceId, Month: month, Year: year, CreditCardNumber: creditCardNumber, Cvv: cvv, PaidAmount: amountPaid, PaidJVID: jvid };
            var dataToSend = JSON.stringify(dataToSend_res1);
      
            $.ajax({
                type: "POST",
                url: '/Kippin/Finance/Invoice/InvoiceStripReport',
                data: dataToSend,
                dataType: 'json',
                contentType: false,
                processData: false,
                success: function (response) {
                    $('#myModalLabel').modal('toggle');
                    function doStuff() {
                        alert("Stripe Payment Done Successfully.");
                        location.reload();
                    }
                    setTimeout(doStuff, 1000);
                },
                error: function (error) {
                    $('#myModalLabel').modal('toggle');
                    function doStuff() {
                        alert("Stripe Payment Done Successfully.");
                        location.reload();
                    }
                    setTimeout(doStuff, 1000);

                }
            });
        }


    }
});

$(document).on("click", ".clsCancel", function (e) {    
    var rowVal = $(this).closest('td').parent()[0].sectionRowIndex;
    rowVal = rowVal + 1;
    var invoiceid = $('#ActiverInvoiceReportListGrid tr:eq(' + rowVal + ') td:eq(' + 19 + ')').find("div").attr('title');
   
    var req = { ButtonType: 'Cancel', Type: 1, SectionType: 'Received', Id: invoiceid }

    $.ajax({
        type: 'POST',
        data: req,
        url: '/Kippin/Finance/Invoice/EditInvoiceReceivedInvoice',
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        data: JSON.stringify(req),
        success: function (data) {
            alert("Invoice Cancel Successfully");
            location.reload();
           // window.location.href = "@Url.Action("InvoiceReceivedInvoice", "Invoice")";
        },
        error: function (ob, errStr) {
            alert("Invoice Cancel Successfully");
            location.reload();
           // window.location.href = "@Url.Action("InvoiceReceivedInvoice", "Invoice")";
        }
    });
   
});