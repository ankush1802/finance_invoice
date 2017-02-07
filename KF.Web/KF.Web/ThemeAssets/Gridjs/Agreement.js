function printPage() {
    //Print Page
    window.print();

}
function validateEmail(email) {
    var reg = /^[\w\-\.\+]+\@[a-zA-Z0-9\.\-]+\.[a-zA-z0-9]{2,4}$/;
    if (reg.test(email)) {
        return true;
    }
    else {
        return false;
    }
}
function sendMail() {
    var email = $("#recipient-name").val();
    if ($.trim(email).length == 0) {
        alert('Please Enter Valid Email Address');
        return false;
    }
    if (validateEmail(email)) {
       
        var url = "/Kippin/Finance/Account/SendAgreement?Email=REPLACEME";
        window.location.href = url.replace('REPLACEME', $.trim(email));
        return false;
    }
    else {
        alert('Invalid Email Address');
        return false;
    }

   
}