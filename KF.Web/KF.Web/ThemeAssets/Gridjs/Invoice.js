$(document).ready(function () {

});
var Finalinvoiceno = [];
var Finaldesc = [];
var Finalquantity = [];
var Finalrate = [];
var Finalamount = [];
var Finaldiscountamt = [];
var Finaldiscountpercentage = [];
var Finalsubtotal = [];
var Finaltaxamt = [];
var Finaltaxpercentage = [];
var FinalItemDetails = [];
var FinalqstTax = [];
var FinalhstTax = [];
var FinalgstTax = [];
var FinalpstTax = [];
var FinalcustomerService = [];

var FinalServiceType = [];
var FinalServiceTypeId = [];

var FinalClassficationItemId = [];
var FinalClassCustomerService = [];
var FinalClassCustomerServiceTypeId = [];
var FinalCustomerTypeId = [];
var FinalCustomerType = [];

var counter = 0;
var table = $('<table></table>');
//$('#dvAdditionalEmails').empty();
var finalemailIds = "";

function addItmes(item) {
    Finaldesc.push(item);
};
function addsNo(sNo) {
    Finalinvoiceno.push(sNo);
};
function addamount(amount) {
    Finalamount.push(amount);
};
function addqstTax(qstTax) {
    FinalqstTax.push(qstTax);
};
function addhstTax(hstTax) {
    FinalhstTax.push(hstTax);
};
function addrate(rate) {
    Finalrate.push(rate);
};
function adddiscount(discount) {
    Finaldiscountamt.push(discount);
};
function addtax(tax) {
    Finaltaxamt.push(tax);
};
function addgstTax(gstTax) {
    FinalgstTax.push(gstTax);
};
function addquantity(quantity) {
    Finalquantity.push(quantity);
};
function addcustomerService(customerService) {
    FinalcustomerService.push(customerService);
};
function addpstTax(pstTax) {
    FinalpstTax.push(pstTax);
};
function addsubTotal(subTotal) {
    Finalsubtotal.push(subTotal);
};


function addserviceType(servicetype) {
    FinalServiceType.push(servicetype);
};
function addserviceTypeId(servicetypeid) {
    FinalServiceTypeId.push(servicetypeid);
};

function addClassficationItemId(itemid) {
    FinalClassficationItemId.push(itemid)
}

function addClassCustomerService(CustomerService) {
    FinalClassCustomerService.push(CustomerService)
}

function addClassCustomerServiceTypeId(CustomerServiceTypeId) {
    FinalClassCustomerServiceTypeId.push(CustomerServiceTypeId)
}

function removeItem(index) {
    Finaldesc.splice(index - 1, 1);
    Finalinvoiceno.splice(index - 1, 1);
    Finalamount.splice(index - 1, 1);
    FinalqstTax.splice(index - 1, 1);
    FinalhstTax.splice(index - 1, 1);
    Finalrate.splice(index - 1, 1);
    Finaldiscountamt.splice(index - 1, 1);
    Finaltaxamt.splice(index - 1, 1);
    FinalgstTax.splice(index - 1, 1);
    Finalquantity.splice(index - 1, 1);
    FinalcustomerService.splice(index - 1, 1);
    FinalpstTax.splice(index - 1, 1);
    Finalsubtotal.splice(index - 1, 1);
    FinalServiceType.splice(index - 1, 1);
    FinalServiceTypeId.splice(index - 1, 1);
};
$(document).on("click", ".clsDltEmailId", function () {


    var checkstr = confirm('Are you sure you want to delete this?');
    if (checkstr == true) {
        var trId = $(this).closest('tr').attr('id');
        $("#" + trId).remove();
    } else {
        return false;
    }





})
$(document).on("click", ".clsModalEmails", function () {
    $("#modalEmail").dialog('open');
});

$(document).on("click", ".ClsAdditionalEmail", function (e) {
    var emailId = $("#txtEmail").val();
    if (emailId != null) {
        var reg = /^([A-Za-z0-9_\-\.])+\@([A-Za-z0-9_\-\.])+\.([A-Za-z]{2,4})$/;
        if (reg.test($("#txtEmail").val()) == false) {
            alert('Please enter valid email id');
            return false;
        }
        else {
            var emailIds = $("#hdemcc").val();
            if (emailIds != "") {
                emailIds = emailIds.split(",");
                if ($.inArray(emailId, emailIds) != -1) {
                    alert("Email Id is already exist");
                    $("#txtEmail").val('');
                    return false;
                }
                else {
                    //table.append($('<tr></tr>').append($('<td></td>').append(
                    //$('<label>').attr({
                    //    for: 'lblEmailId'
                    //}).text(emailId))));
                    //$('#dvAdditionalEmails').append(table);             

                    table.append($('<tr Id=' + counter + '></tr>').append($('<td></td>').append(
                   $('<label>').attr({
                       for: 'lblEmailId'
                       //}).text(emailId))).append($('<td></td>').append($('<button type="button" class="ui-button ui-widget ui-state-default ui-corner-all ui-button-icon-only clsDltEmailId" role="button" title="Close"><span class="ui-button-icon-primary ui-icon ui-icon-closethick"></span><span class="ui-button-text">Close</span></button>'))));
                   }).text(emailId))).append($('<td></td>')));
                    $('#dvAdditionalEmails').append(table);
                    counter++;
                    finalemailIds = finalemailIds + "," + emailId;
                    //alert($('#hdnemailCC').val());
                    if (finalemailIds.charAt(0) == ",") {
                        finalemailIds = finalemailIds.substring(1);
                    }
                    else {
                    }
                    $("#hdemcc").val(finalemailIds);
                    $("#txtEmail").val('');
                }
            }
            else {
                //table.append($('<tr></tr>').append($('<td></td>').append(
                //   $('<label>').attr({
                //       for: 'lblEmailId'
                //   }).text(emailId))));
                //$('#dvAdditionalEmails').append(table);


                table.append($('<tr Id=' + counter + '></tr>').append($('<td></td>').append(
                      $('<label>').attr({
                          for: 'lblEmailId'
                      //}).text(emailId))).append($('<td></td>').append($('<button type="button" class="ui-button ui-widget ui-state-default ui-corner-all ui-button-icon-only clsDltEmailId" role="button" title="Close"><span class="ui-button-icon-primary ui-icon ui-icon-closethick"></span><span class="ui-button-text">Close</span></button>'))));
            }).text(emailId))).append($('<td></td>')));
                $('#dvAdditionalEmails').append(table);
                counter++;
                finalemailIds = finalemailIds + "," + emailId;
                //alert($('#hdnemailCC').val());
                if (finalemailIds.charAt(0) == ",") {
                    finalemailIds = finalemailIds.substring(1);
                }
                else {
                }
                $("#hdemcc").val(finalemailIds);
                $("#txtEmail").val('');
            }
        }
    }
    else {
        return false;
    }
});
