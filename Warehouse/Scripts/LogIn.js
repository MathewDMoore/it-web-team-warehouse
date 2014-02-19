window.onload = function () {
    document.getElementById('ctl00_MainContent_UserName').focus();
    document.getElementById('ctl00_MainContent_UserName').select();
};

var EnteredUserName, Password, ContractorFirstName, ContractorLastName;

function capLock(e) {
    kc = e.keyCode ? e.keyCode : e.which;
    sk = e.shiftKey ? e.shiftKey : ((kc == 16) ? true : false);
    if (((kc >= 65 && kc <= 90) && !sk) || ((kc >= 97 && kc <= 122) && sk))
        document.getElementById('divMayus').style.visibility = 'visible';
    else
        document.getElementById('divMayus').style.visibility = 'hidden';
};

function LoginController($http,$window) {
    var login = this;
    login.Username = null;
    login.Password = null;
    login.UserNameCheck = function() {
        if (login.Username.toLowerCase() === 'contractshipping') {
            $('#myModal').modal({ show: true, keyboard: false, backdrop: 'static' });
            $("#modalFirstName").focus();
        } else {
            login.SubmitLogin();
        }

    };
    login.SubmitLogin = function() {
        $http({
            method: "POST",
            url: "/ship/services/UserAuthenticationService.svc/UserAuthenticationLogin",
            data: { username: login.Username, password: login.Password, contractorName: login.ContractorFirstName + login.ContractorLastName },
        }).success(function(result) {

            if (result.ErrorMessage != null && result.ErrorMessage != '') {
                login.ErrorMessage = result.ErrorMessage;
            } else {
                var returnUrl = $.getUrlVar('ReturnUrl');
                if (returnUrl === undefined) {
                    returnUrl = '/home/ScanSerialNumber/';
                }
                $window.location = unescape(returnUrl);
            }

        });
    };
}



function SubmitContractorDetails() {
    ContractorFirstName = document.getElementById('modalFirstName').value;
    ContractorLastName = document.getElementById('modalLastName').value;
    $('#myModal').modal('hide');
    SubmitLogin();
}

