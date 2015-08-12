// Karma configuration
// Generated on Wed Aug 12 2015 13:05:50 GMT-0600 (Mountain Daylight Time)

module.exports = function(config) {
  config.set({

    // base path that will be used to resolve all patterns (eg. files, exclude)
    basePath: '',


    // frameworks to use
    // available frameworks: https://npmjs.org/browse/keyword/karma-adapter
    frameworks: ['jasmine'],


    // list of files / patterns to load in the browser
    files: [
	'Scripts/libs/jquery.min.js',
	'Scripts/libs/angular.min.js',
	'Scripts/libs/angular-animate.min.js',
	'Scripts/libs/angular-mocks.js',
	'Scripts/libs/firebase.js',
	'Scripts/libs/jasmine-html.js',
	'Scripts/libs/angularfire.min.js',
	'../c4inventoryserialization/scripts/libs/angular-bootstrap/ui-bootstrap.min.js',
	'../c4inventoryserialization/scripts/libs/angular.audio.js',
	'../c4inventoryserialization/scripts/ng-table-export.js',
	'../c4inventoryserialization/scripts/underscore-min.js',

	'../c4inventoryserialization/scripts/directives.js',
	'../c4inventoryserialization/scripts/common.js',
	'../c4inventoryserialization/scripts/shippingfilters.js',
	'../c4inventoryserialization/scripts/ng-table.js',
     
	 '../c4inventoryserialization/scripts/shippingapp.js',
	 '../C4InventorySerialization/Scripts/services/*.js',
      '../C4InventorySerialization/Scripts/Controllers/*.js',
      'Scripts/Tests/*.js'
    ],


    // list of files to exclude
    exclude: [
    ],


    // preprocess matching files before serving them to the browser
    // available preprocessors: https://npmjs.org/browse/keyword/karma-preprocessor
    preprocessors: {
    },


    // test results reporter to use
    // possible values: 'dots', 'progress'
    // available reporters: https://npmjs.org/browse/keyword/karma-reporter
    reporters: ['progress'],


    // web server port
    port: 9876,


    // enable / disable colors in the output (reporters and logs)
    colors: true,


    // level of logging
    // possible values: config.LOG_DISABLE || config.LOG_ERROR || config.LOG_WARN || config.LOG_INFO || config.LOG_DEBUG
    logLevel: config.LOG_INFO,


    // enable / disable watching file and executing tests whenever any file changes
    autoWatch: false,


    // start these browsers
    // available browser launchers: https://npmjs.org/browse/keyword/karma-launcher
    browsers: ['PhantomJS'],


    // Continuous Integration mode
    // if true, Karma captures browsers, runs the tests and exits
    singleRun: true
  });
};
