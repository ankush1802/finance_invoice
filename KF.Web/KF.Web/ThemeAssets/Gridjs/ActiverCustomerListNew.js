﻿var ActiverCustomerListGridNew;
$(document).ready(function () {
    ActiverCustomerListGridNew = $("#ActiverCustomerListGridNew").grid({
        dataKey: "Id",
        uiLibrary: "bootstrap",
        columns: [


            { field: "CompanyName", title: "Company Name", sortable: false },
        { title: "Create", field: "Update", width: 100, type: "icon", icon: "glyphicon-edit", tooltip: "View", events: { "click": editEmployee } }

        ],
        pager: { enable: true, limit: 5, sizes: [2, 5, 10, 20] }

    });
});

function editEmployee(e) {

    var url = "/Kippin/Finance/Invoice/CreateInvoice?id=REPLACEME";
    window.location.href = url.replace('REPLACEME', e.data.id);



}
function SendMailButton() {


}