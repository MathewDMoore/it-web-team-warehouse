function LoginController($http,$window) {
    var login = this;
    login.Username = null;
    login.Password = null;
    login.UserNameCheck = function() {
        if (login.Username.toLowerCase() === 'contractshipping') {
            $('#myModal').modal({ show: true, keyboard: false, backdrop: 'static' });
//            login.
        } else {
            login.SubmitLogin();
        }

    };
    login.CapLock = function(e) {
        var kc = e.keyCode ? e.keyCode : e.which;
        var sk = e.shiftKey ? e.shiftKey : ((kc == 16) ? true : false);
        if (((kc >= 65 && kc <= 90) && !sk) || ((kc >= 97 && kc <= 122) && sk)) {
            login.ShowMayus = true;
        }
        else {
            login.ShowMayus = false;
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
    login.SubmitContractorDetails = function() {
        login.ShowModel = false;
        login.SubmitLogin();
    };
}
