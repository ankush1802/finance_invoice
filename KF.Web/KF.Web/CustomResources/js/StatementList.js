var AllUserExpenseGrid;
$(document).ready(function () {
    AllUserExpenseGrid = $("#AllUserExpenseGrid").grid({
        dataKey: "Id",
        uiLibrary: "bootstrap",
        columns: [{
            field: "JVID",
            title: "JVID",
            sortable: false
        }, {
            field: "StatementDate",
            title: "Date",
            sortable: false
        }, {
            field: "StatementDescription",
            title: "Description",
            sortable: false
        }, {
            field: "Debit",
            title: "Debit",
            sortable: false
        }, {
            field: "Credit",
            title: "Credit",
            sortable: false
        }, {
            field: "StatementClassification",
            title: "Classification",
            sortable: false
        }, {
            field: "StatementBank",
            title: "Bank",
            sortable: false
        }, {
            field: "StatementUploadType",
            title: "Upload Type",
            sortable: false
        }, {
            field: "StatementStatus",
            title: "Status",
            sortable: true
        },

            {
                title: "Edit",
                field: "Delete",
                width: 50,
                type: "icon",
                icon: "glyphicon-edit",
                tooltip: "Edit",
                events: {
                    "click": editExpense
                }
            }, {
                title: "Delete",
                field: "Delete",
                width: 50,
                type: "icon",
                icon: "glyphicon-remove",
                tooltip: "Delete",
                events: {
                    "click": deleteExpense
                }
            }

        ],
        pager: {
            enable: true,
            limit: 5,
            sizes: [2, 5, 10, 20]
        }

    });
});

function editExpense(e) {
    var url = "/Kippin/Finance/Accounting/EncodeQueryString" + "?statementId=" + e.data.id + "&&year=" + $("#selectedYearId").val();//Dev
   // var url = "/Accounting/EncodeQueryString" + "?statementId=" + e.data.id + "&&year=" + $("#selectedYearId").val();//Local
    window.location.href = url;
    //$.ajax({
    //    type: "GET",
    //    url: url,
    //    data: '',
    //    contentType: "application/json; charset=utf-8",
    //    dataType: "json",
    //    success: function (data) {
    //        alert();
    //        //var url = "/Kippin/Finance/Kippin/EditExpense?id=REPLACEME&&year=" + $("#selectedYearId").val();
    //        var RedirectUrl = "/Accounting/StatementReconcilation?args=REPLACEME";
    //        window.location.href = RedirectUrl.replace('REPLACEME', data);
    //    },
    //    error: function (data) {
    //        // alert("fail");
    //    }
    //});
}

function deleteExpense(e) {
    var warningMsg = confirm("Are you sure you want to delete this record?");
    if (warningMsg == true) {
        // var url = "/Kippin/Accounting/Kippin/DeleteBankExpenseById?BankExpenseId=REPLACEME";
        var url = "/Kippin/Finance/Accounting/DeleteBankExpenseById?BankExpenseId=REPLACEME";
      //  var url = "/Accounting/DeleteBankExpenseById?BankExpenseId=REPLACEME";//Local
        window.location.href = url.replace('REPLACEME', e.data.id);
    } else {
        return false;
    }

}

function SendMailButton() { }