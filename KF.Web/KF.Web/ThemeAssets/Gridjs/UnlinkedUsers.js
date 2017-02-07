var UnlinkedUserListGrid;
$(document).ready(function () {
    UnlinkedUserListGrid = $("#UnlinkedUserListGrid").grid({
        dataKey: "UserId",
        uiLibrary: "bootstrap",
        columns: [

            { field: "Fullname", title: "Name", sortable: false },
            { field: "Email", title: "Email", sortable: false },
             { field: "CorporationAddress", title: "Corporation Address", sortable: false },
            { field: "TaxStartYear", title: "Tax Start Year", width: 120, sortable: false },
            { field: "TaxEndYear", title: "Tax End Year", width: 120, sortable: false },
            { field: "UserStatus", title: "Status", width: 70, sortable: false },
            { field: "SelectUser", title: "Select User", width: 120, type: "icon", icon: "btn btn-primary", tooltip: "Select User", events: { "click": SelectUser } }
        ],
        pager: { enable: true, limit: 5, sizes: [2, 5, 10, 20] }

    });
});
function SendMailButton() {
    $("#UnlinkedUserListGrid tr").each(function () {
        $(this).find("td").eq(5).find("div").html("<button style='color:red' class='btn btn-default' type='button'>Unlinked</button>");
        $(this).find("td").eq(6).find("div").html("<button class='btn btn-primary' type='button'>Select User</button>");

    });

}


function SelectUser(e) {
   
    //var url = "/Kippin/Finance/UnlinkedUsers/Reports?id=REPLACEME";
    var url = "/UnlinkedUsers/Reports?id=REPLACEME";
    window.location.href = url.replace('REPLACEME', e.data.id);
    
}
//$('#ActiveUserListGrid').find('td').find('span').text("Send Mail");
//ActiveUserListGrid
//glyphicon glyphicon-send