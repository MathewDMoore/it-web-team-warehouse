﻿window.onload = function () {
    document.getElementById("ctl00_MainContent_UserName").focus();
    document.getElementById("ctl00_MainContent_UserName").select();
    $("#ctl00_MainContent_Password").keypress(function (e) {
        if (e.which === 13) {
            $("#LoginBtn").click();
        }
    });
    $("#modalLastName").keypress(function (e) {
        if (e.which === 13) {
            $("#submitName").click();
        }
    }); $("#modalFirstName").keypress(function (e) {
        if (e.which === 13) {
            $("#submitName").click();
        }
    });
};

var EnteredUserName, Password, ContractorFirstName, ContractorLastName, errorDiv;

function capLock(e) {
    var kc = e.keyCode ? e.keyCode : e.which;
    var sk = e.shiftKey ? e.shiftKey : ((kc === 16) ? true : false);

    if (((kc >= 65 && kc <= 90) && !sk) || ((kc >= 97 && kc <= 122) && sk))
        document.getElementById("divMayus").style.visibility = "visible";
    else
        document.getElementById("divMayus").style.visibility = "hidden";
};

function UserNameCheck() {
    EnteredUserName = document.getElementById("ctl00_MainContent_UserName").value;
    Password = document.getElementById("ctl00_MainContent_Password").value;
    if (EnteredUserName.toLowerCase() === "contractshipping") {
        $("#myModal").modal({ show: true, keyboard: false, backdrop: "static" });

        setTimeout(function() {
            $("#modalFirstName").focus();
        }, 300);       
        errorDiv = $(".alert");
        errorDiv.hide();
        $("#modalFirstName").focus();
    }
    else {
        SubmitLogin();
    }

}

function SubmitContractorDetails() {
    ContractorFirstName = document.getElementById("modalFirstName").value;
    ContractorLastName = document.getElementById("modalLastName").value;

    if (ContractorFirstName && ContractorLastName) {
        SubmitLogin();
       
    } else {
        errorDiv.show();
        errorDiv.text("Please enter First and Last names.");
 
    }
}

function SubmitLogin() {

    var loginData = {};
    loginData.userName = EnteredUserName;
    loginData.password = Password;
    loginData.contractorName = ContractorFirstName + ContractorLastName;
    var preparedData = $.toJSON(loginData);
    $.ajax({
        type: "POST",
        url: "/ship/services/UserAuthenticationService.svc/UserAuthenticationLogin",
        data: preparedData,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (result) {

            if (result.ErrorMessage != null && result.ErrorMessage !== "") {
                $("#ctl00_MainContent_loggedUser").text(result.ErrorMessage);
            }
            else {
                $.cookie(result.CookieName, result.EncryptedTicket);
                var returnUrl = $.getUrlVar("ReturnUrl");
                if (returnUrl === undefined) {
                    returnUrl = "../Content/ScanOrder.aspx";
                }   
                window.location = unescape(returnUrl);
            }
        }
    });
}