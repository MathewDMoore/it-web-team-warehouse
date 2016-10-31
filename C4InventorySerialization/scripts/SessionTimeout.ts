class BaseConfiguration {
	static $inject = ["$httpProvider"];

	constructor($httpProvider: ng.IHttpProvider) {
		console.info("Loading main");
		$httpProvider.interceptors.push(["$q", ($q) => { return new BaseInterceptor($q); }]);
	}
}

class BaseInterceptor {

	constructor(private $q: ng.IQService) {
		console.log("Loading BaseInterceptor");
	}

	response = (response) => {
		return response || this.$q.when(response);
	}

	responseError = (rejection) => {
		var sessionTimeout = rejection.data.indexOf("ThrowSessionTimeoutException") > 0;

		if(sessionTimeout) 
			window.location.href = "Logout.aspx";
		
		return this.$q.reject(rejection);
	}
}