var ActiverInvoiceSentInvoiceListGrid;
$(document).ready(function () {
    ActiverInvoiceSentInvoiceListGrid = $("#ActiverInvoiceSentInvoiceListGrid").grid({
        dataKey: "Id",
        uiLibrary: "bootstrap",
        columns: [


            { field: "InvoiceDate", title: "Invoice Date", sortable: false },
            { field: "InvoiceNumber", title: "Invoice Number", sortable: false },
           { field: "FirstName", title: "Customer", sortable: false },
           { field: "Total", title: "Invoice Amt.", sortable: false },
           { field: "In_R_FlowStatus", title: "Flow Status", sortable: false },
            { field: "PdfViewPath", title: "Pdf Viewer", width: 50, tooltip: "Pdf View", events: { "click": editPdf } },
        { title: "Edit", field: "Update", width: 50, type: "icon", icon: "glyphicon-edit", tooltip: "Edit", events: { "click": editEmployee } }
          ,
           { field: "IsCustomer", title: "IsCustomer", sortable: false },
        ],
        pager: { enable: true, limit: 5, sizes: [2, 5, 10, 20] }

    });
});
function editPdf(e) {
    var row_index = $(this).parent().index() + 1;
    var col_index = 7;
    var IsCustomer = $('#ActiverInvoiceSentInvoiceListGrid tr:eq(' + row_index + ') td:eq(' + col_index + ')').find("div").attr('title');
    var Id = e.data.id;
    
    $.ajax({
        url: '/Kippin/Finance/Invoice/ReceivedInvoicePdf/',
        data: { 'id': Id, 'iscustomer': IsCustomer }
    }).done(function () {
       // alert('Added');
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
function SendMailButton() {


}

function editEmployee(e) {
    //var url = "/Kippin/Finance/Kippin/UpdateSubAccountant?id=REPLACEME";
    if (e.data.Id1 == 0)
    {

    }
    else
    {
        var url = "/Kippin/Finance/Invoice/EditInvoiceSentInvoice?id=REPLACEME";
        window.location.href = url.replace('REPLACEME', e.data.id);
    }

   
}