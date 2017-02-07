﻿var ActiverReceivedInvoiceListGrid;
$(document).ready(function () {
    ActiverReceivedInvoiceListGrid = $("#ActiverReceivedInvoiceListGrid").grid({
        dataKey: "Id",
        uiLibrary: "bootstrap",
        columns: [


            { field: "InvoiceDate", title: "Invoice Date", sortable: false },
            { field: "InvoiceNumber", title: "Invoice Number", sortable: false },
           { field: "Username", title: "Supplier", sortable: false },
           { field: "Total", title: "Invoice Amt.", sortable: false },
           { field: "Pro_FlowStatus", title: "Flow Status", sortable: false },
             { field: "", title: "Pdf Viewer", width: 50, tooltip: "Pdf View", events: { "click": editPdf } },
        { title: "Edit", field: "Update", width: 50, type: "icon", icon: "glyphicon-edit", tooltip: "Edit", events: { "click": editEmployee } },
        { field: "IsCustomer", title: "IsCustomer", sortable: false },
        ],
        pager: { enable: true, limit: 5, sizes: [2, 5, 10, 20] }

    });



});


function editPdf(e) {
    var row_index = $(this).parent().index() + 1;
    var col_index = 7;
    var IsCustomer = $('#ActiverReceivedInvoiceListGrid tr:eq(' + row_index + ') td:eq(' + col_index + ')').find("div").attr('title');
    var Id = e.data.id;
    $.ajax({
        url: 'http://52.27.249.143/Kippin/Finance/Invoice/ReceivedInvoicePdf/?id=' + Id + '&IsCustomer=' + IsCustomer,
        data: { 'id': Id, 'iscustomer': IsCustomer },
        success: function () {
           
        }
    }).done(function (data, status, jqXHR) {

    });

}
function SendMailButton() {
    $(document).ready(function () {
        $('#ActiverReceivedInvoiceListGrid tr').each(function () {
            $(this).find("th").eq(7).hide();
            $(this).find("td").eq(7).hide();
        });
    });

}
function imgload() {

    $("table tr").each(function () {

        var one = $(this).find("td").eq(5).find("div").html();
        $(this).find("td").eq(5).find("div").removeAttr("title");
        $(this).find("td").eq(5).find("div").html("<a href='" + one + "' data-url='" + one + "' class='pop'><img src='/Finance/ThemeAssets/img/pdfviewier.png' style='width:43px;height:30px;'></a>");
    });
}

//function editPdf(e) {
//    var url = ($(this).find('a').attr('data-url'));
//    var datauri = 'data:application/pdf;base64,' + Base64.encode(url);
//    var win = window.open(url, '_new', "width=1024,height=768,resizable=yes,scrollbars=yes,toolbar=no,location=no,directories=no,status=no,menubar=no,copyhistory=no");
//    // win.document.location.href = datauri;


//}


function editEmployee(e) {
    //var url = "/Kippin/Finance/Kippin/UpdateSubAccountant?id=REPLACEME";

    var url = "/Kippin/Finance/Invoice/EditReceivedInvoice?id=REPLACEME";
    window.location.href = url.replace('REPLACEME', e.data.id);


}