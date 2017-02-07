var ActiveUserListGrid;
$(document).ready(function () {
    ActiveUserListGrid = $("#ActiveUserListGrid").grid({
        dataKey: "Id",
        uiLibrary: "bootstrap",
        columns: [

            { field: "Fullname", title: "Name", sortable: false },
            { field: "Email", title: "Email", sortable: false },
             //{ field: "CorporationAddress", title: "Corporation Address", sortable: false },
            { field: "Company", title: "Company", sortable: false },
            { field: "TaxStartYear", title: "Tax Start Year", width: 120, sortable: false },
            { field: "TaxEndYear", title: "Tax End Year", width: 120, sortable: false },
            { field: "UserStatus", title: "Status", width: 70, sortable: false },
            { field: "EmailSendStatus", title: "Key Issued", width: 100, sortable: false },
              {
                  field: "SendMail", title: "Mail", width: 110, type: "icon", icon: "btn btn-primary", tooltip: "Send Mail", events: { "click": SendMail }
              },
              {
                  field: "SelectUser", title: "Select User", width: 120, type: "icon", icon: "btn btn-primary", tooltip: "Select User", events: { "click": SelectUser }
              }


        ],
        pager: { enable: true, limit: 5, sizes: [2, 5, 10, 20] }

    });
});
function SendMailButton() {
    $("#ActiveUserListGrid tr").each(function () {
        var status = $(this).find("td").eq(5).find("div").attr('title');
        if (status == "Active")
        {
            $(this).find("td").eq(5).find("div").html("<button style='color:green' class='btn btn-default' type='button'>Active</button>");
        }
        else if (status == "In-Active")
        {
            $(this).find("td").eq(5).find("div").html("<button style='color:red' class='btn btn-default' type='button'>Inactive</button>");
        }
        else if (status == "Trial")
        {
            $(this).find("td").eq(5).find("div").html("<button style='color:#357ebd' class='btn btn-default' type='button'>Trial</button>");
        }
       // var one = $(this).find("td").eq(3).find("div").attr('title');
        $(this).find("td").eq(7).find("div").html("<button class='btn btn-primary' type='button'>Send Mail</button>");
        $(this).find("td").eq(8).find("div").html("<button class='btn btn-primary' type='button'>Select User</button>");

    });

}
function SendMail(e) {
    //var url = "/Kippin/Kippin/Finance/Accounting/SendEmail?UserId=REPLACEME"; //Live
   var url = "/Kippin/Finance/Accounting/SendEmail?UserId=REPLACEME"; //Dev
   // var url = "/Accounting/SendEmail?UserId=REPLACEME";
    window.location.href = url.replace('REPLACEME', e.data.id);
}

function SelectUser(e) {
    // var url = "/Kippin/Finance/Kippin/SelectUser?UserId=REPLACEME"; //Live
    var url = "/Kippin/Finance/Accounting/SelectUser?UserId=REPLACEME"; //Dev
   // var url = "/Accounting/SelectUser?UserId=REPLACEME"; //Local
    window.location.href = url.replace('REPLACEME', e.data.id);
}
