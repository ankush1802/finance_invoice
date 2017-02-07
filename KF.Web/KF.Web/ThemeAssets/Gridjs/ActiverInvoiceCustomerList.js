var ActiverInvoiceCustomerListGrid;
$(document).ready(function () {
    ActiverInvoiceCustomerListGrid = $("#ActiverInvoiceCustomerListGrid").grid({
        dataKey: "Id",
        uiLibrary: "bootstrap",
        columns: [


            { field: "CompanyName", title: "Company Name", sortable: false },
        { title: "Edit", field: "Update", width: 50, type: "icon", icon: "glyphicon-edit", tooltip: "Edit", events: { "click": editEmployee } }

        ],
        pager: { enable: true, limit: 5, sizes: [2, 5, 10, 20] }

    });
});

function editEmployee(e) {
    //var url = "/Kippin/Finance/Kippin/UpdateSubAccountant?id=REPLACEME";
    
    var url = "/Kippin/Finance/Invoice/InvoiceUpdateCustomer?id=REPLACEME";
        window.location.href = url.replace('REPLACEME', e.data.id);
   
}
function SendMailButton() {


}