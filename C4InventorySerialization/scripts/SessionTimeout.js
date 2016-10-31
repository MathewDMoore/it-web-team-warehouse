var BaseConfiguration = (function () {
    function BaseConfiguration($httpProvider) {
        console.info("Loading main");
        $httpProvider.interceptors.push(["$q", function ($q) { return new BaseInterceptor($q); }]);
    }
    BaseConfiguration.$inject = ["$httpProvider"];
    return BaseConfiguration;
}());
var BaseInterceptor = (function () {
    function BaseInterceptor($q) {
        var _this = this;
        this.$q = $q;
        this.response = function (response) {
            return response || _this.$q.when(response);
        };
        this.responseError = function (rejection) {
            var sessionTimeout = rejection.data.indexOf("ThrowSessionTimeoutException") > 0;
            if (sessionTimeout)
                window.location.href = "Logout.aspx";
            return _this.$q.reject(rejection);
        };
        console.log("Loading BaseInterceptor");
    }
    return BaseInterceptor;
}());
