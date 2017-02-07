var ActiveUserListGrid;
$(document).ready(function () {

    ActiveUserListGrid = $("#ActiveUserListGrid").grid({
        dataKey: "Id",
        uiLibrary: "bootstrap",
        columns: [

            { field: "Username", title: "Username", sortable: false },
            { field: "Email", title: "Email", sortable: false },
             //{ field: "Password", title: "Password", sortable: false },
              { field: "EmployeeStatus", title: "Status", width: 70, sortable: false },
            { field: "EmailSendStatus", title: "Key Issued", width: 100, sortable: false },
              {
                  field: "SendMail", title: "Mail", width: 110, type: "icon", icon: "btn btn-primary", tooltip: "Send Mail", events: { "click": SendMail }
              },
                  { title: "Edit", field: "Update", width: 50, type: "icon", icon: "glyphicon-edit", tooltip: "Edit", events: { "click": editEmployee } },
                  { title: "Deactivate", field: "Deactivate", width: 50, type: "icon", icon: "glyphicon-edit", tooltip: "Deactivate", events: { "click": Deactivate } },
                   { title: "Activate", field: "Deactivate", width: 50, type: "icon", icon: "glyphicon-edit", tooltip: "Activate", events: { "click": Activate } },
                    { title: "Delete", field: "Deactivate", width: 50, type: "icon", icon: "glyphicon-edit", tooltip: "Delete", events: { "click": Delete } },

        ],
        pager: { enable: true, limit: 5, sizes: [2, 5, 10, 20] }

    });
});
function Delete(e) {
    var warningMsg = confirm("Are you sure you want to delete this employee?");
    if (warningMsg == true) {
        //  var url = "/Kippin/Finance/Kippin/DeleteEmployee?Id=REPLACEME";
        var url = "/Kippin/Finance/Accounting/DeleteEmployee?Id=REPLACEME";
        window.location.href = url.replace('REPLACEME', e.data.id);
    }
    else {
        return false;
    }
}
function Activate(e) {
    var warningMsg = confirm("Are you sure you want to activate this employee?");
    if (warningMsg == true) {
        //var url = "/Kippin/Finance/Kippin/ActivateEmployee?Id=REPLACEME";
        var url = "/Kippin/Finance/Accounting/ActivateEmployee?Id=REPLACEME";
        window.location.href = url.replace('REPLACEME', e.data.id);
    }
    else {
        return false;
    }
}
function Deactivate(e) {
    var warningMsg = confirm("Are you sure you want to deactivate this employee?");
    if (warningMsg == true) {
        // var url = "/Kippin/Finance/Kippin/DeactivateEmployee?Id=REPLACEME";
        var url = "/Kippin/Finance/Accounting/DeactivateEmployee?Id=REPLACEME";
        window.location.href = url.replace('REPLACEME', e.data.id);
    }
    else {
        return false;
    }
}

function editEmployee(e) {
    // var url = "/Kippin/Finance/Accounting/EncryptSubAccountantId?id=REPLACEME";
    var url = "/Kippin/Finance/Accounting/EncryptSubAccountantId?id=REPLACEME";
    window.location.href = url.replace('REPLACEME', e.data.id);
}
function SendMailButton() {
    $("#ActiveUserListGrid tr").each(function () {
        var status = $(this).find("td").eq(2).find("div").attr('title');
        if (status == "Active") {
            $(this).find("td").eq(2).find("div").html("<button style='color:green' class='btn btn-default' type='button'>Active</button>");
        }
        else {
            $(this).find("td").eq(2).find("div").html("<button style='color:red' class='btn btn-default' type='button'>Inactive</button>");
        }
        // var one = $(this).find("td").eq(3).find("div").attr('title');
        $(this).find("td").eq(4).find("div").html("<button class='btn btn-primary' type='button'>Send Mail</button>");
        $(this).find("td").eq(5).find("div").html("<button class='btn btn-primary' type='button'>Update Employee</button>");
        $(this).find("td").eq(6).find("div").html("<button class='btn btn-primary' type='button'>Deactivate Employee</button>");
        $(this).find("td").eq(7).find("div").html("<button class='btn btn-primary' type='button'>Activate Employee</button>");
        $(this).find("td").eq(8).find("div").html("<button class='btn btn-primary' type='button'>Delete Employee</button>");
    });

}
function SendMail(e) {
    //var url = "/Kippin/Finance/Accounting/SendEmployeeEmail?UserId=REPLACEME";
    var url = "/Kippin/Finance/Accounting/SendEmployeeEmail?UserId=REPLACEME";
    window.location.href = url.replace('REPLACEME', e.data.id);
}
