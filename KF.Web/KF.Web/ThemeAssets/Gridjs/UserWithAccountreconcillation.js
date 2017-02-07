var AllUserExpenseGrid;
$(document).ready(function () {
    AllUserExpenseGrid = $("#AllUserExpenseGrid").grid({
        dataKey: "Id",
        uiLibrary: "bootstrap",
        columns: [

            //{ field: "Username", title: "Username", sortable: true },
            { field: "customDate", title: "Date", sortable: false },
             { field: "Description", title: "Description", sortable: false },
              { field: "Bank", title: "Bank", sortable: false },
              { field: "Debit", title: "Debit", sortable: false },
            { field: "Credit", title: "Credit",sortable: false },
           
         //   { field: "Classification", title: "Classification", sortable: false },
           
           // { field: "Status", title: "Status", sortable: true },


        ],
        pager: { enable: true, limit: 20, sizes: [20,25,30] }

    });
});




//AllUserWithAnAccountantExpenseGrid
function SendMailButton() {
    $("#ActiveUserListGrid tr").each(function () {
        // var one = $(this).find("td").eq(3).find("div").attr('title');
        $(this).find("td").eq(7).find("div").html("<button class='btn btn-primary' type='button'>Send Mail</button>");

    });

}