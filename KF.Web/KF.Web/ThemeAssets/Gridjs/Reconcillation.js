var AllUserExpenseGrid;
$(document).ready(function () {
    AllUserExpenseGrid = $("#AllUserExpenseGrid").grid({
        dataKey: "Id",
        uiLibrary: "bootstrap",
        columns: [

            //{ field: "Username", title: "Username", sortable: true },
            { field: "Id", title: "JVID", sortable: false },
            { field: "customDate", title: "Date", sortable: false },
             { field: "Description", title: "Description", sortable: false },
              { field: "Debit", title: "Debit", sortable: false },
            { field: "Credit", title: "Credit", sortable: false },
           
            { field: "Classification", title: "Classification", sortable: false },
            { field: "Bank", title: "Bank", sortable: false },
             { field: "UploadType", title: "Upload Type", sortable: false },
            
            { field: "Status", title: "Status", sortable: true },
            
              { title: "Edit", field: "Delete", width: 50, type: "icon", icon: "glyphicon-edit", tooltip: "Edit", events: { "click": editExpense } },
        { title: "Delete", field: "Delete", width: 50, type: "icon", icon: "glyphicon-remove", tooltip: "Delete", events: { "click": deleteExpense } }

        ],
        pager: { enable: true, limit: 5, sizes: [2, 5, 10, 20] }

    });
});

function editExpense(e) {
  
    //var url = "/Kippin/Finance/Kippin/EditExpense?id=REPLACEME&&year=" + $("#selectedYearId").val();
    var url = "/Kippin/EditExpense?id=REPLACEME&&year=" + $("#selectedYearId").val();
    window.location.href = url.replace('REPLACEME', e.data.id);
}

function deleteExpense(e) {
    var warningMsg = confirm("Are you sure you want to delete this record?");
    if (warningMsg == true) {
        // var url = "/Kippin/Finance/Kippin/DeleteBankExpenseById?BankExpenseId=REPLACEME";
        var url = "/Kippin/DeleteBankExpenseById?BankExpenseId=REPLACEME";
        window.location.href = url.replace('REPLACEME', e.data.id);
    }
    else {
        return false;
    }
   
}
function SendMailButton() {
    //$("#AllUserExpenseGrid tr").each(function () {
    //    var status = $(this).find("td").eq(6).find("div").attr('title');
    //    //if (status == "Cash") {
    //    //    $(this).find("td").eq(6).find("div").html("MJV");
    //    //}
    //});

}