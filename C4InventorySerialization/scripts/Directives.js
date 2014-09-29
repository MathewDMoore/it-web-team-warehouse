var Directives = angular.module("myDirectives", []);

Directives.directive('onEnter', function () {
    return {
        link: function (scope, element, attrs) {
            element.bind('keypress', function (key) {
                if (key.charCode === 9) {
                    key.preventDefault();
                    scope.$apply(attrs.onEnter + '()');
                    var inputs = $('input:text');
                    inputs[inputs.length - 1].focus();

                }
            });
        }
    };
})
    .directive("pieChart", function () {
        return {
            restrict: 'E',
            scope: { ChartData: "=data", Colors: "@colors", Filter: "=filter" },
            template: '<div style="position: relative;margin-top: -116px;">' +
                        '<div class="flot-chart-label" style="top:180px;position:relative;">{{Filter.value}}</div>' +
                        '<div class="flot-chart-sublabel" style="top:185px;position:relative;">{{Tooltip}}&nbsp;</div>' +
                        '<div class="flot-chart"></div>' +
					    '<div class="flot-chart-legend"></div>' +
                      '</div>',
            replace: true,
            link: function (scope, elem, attrs) {
                scope.$watch('ChartData', function (newVal, oldVal) {
                    if (newVal && newVal.Chart && newVal.Chart.length > 0) {
                        if (newVal.Chart.length > 1 && scope.AllColors) {
                            scope.Colors = scope.AllColors;
                        }

                        if (scope.Colors) {
                            if (Object.prototype.toString.call(scope.Colors) != '[object Array]') {
                                scope.Colors = scope.$eval(scope.Colors);
                            }
                            _populateDatChart();
                        }
                    }
                }, true);

                var plot = null;
                function _populateDatChart() {
                    scope.OrigFilter = angular.copy(scope.Filter);
                    plot = $.plot(elem.find('.flot-chart'), scope.ChartData.Chart, {
                        series: {
                            pie: {
                                show: true,
                                radius: .9,
                                innerRadius: .5,
                                stroke: {
                                    width: 4,
                                    color: "#F9F9F9"
                                },
                            },
                        },
                        legend: {
                            show: true,
                            noColumns: 2,
                            margin: [5, 5],
                            container: elem.find('.flot-chart-legend'),
                        },
                        grid: {
                            clickable: true,
                            hoverable: true,
                        },
                        highlightSeries: {
                            color: "black",
                            _optimized: true
                        },
                        colors: scope.Colors,
                    });

                    elem.find(".flot-chart-legend tr td").bind("mouseenter",
                        function () {
                            var s = $(this).text();
                            var ss = plot.getData();

                            _.each(elem.find(".legendLabel"), function (i) {
                                var jItem = $(i);

                                jItem.css("font-weight", "normal"); if (jItem.html() == s) { jItem.css("font-weight", "bold"); }
                            });

                            for (var j = 0; j < ss.length; j++) {
                                if (ss[j].label == s) {
                                    plot.highlight(ss[j], 'plothover');
                                    scope.Filter.value = ss[j].data[0][1];
                                    scope.Tooltip = ss[j].label;
                                    break;
                                }
                            }

                            scope.$apply();
                        }
                    );
                    elem.find(".flot-chart-legend tr td").bind("mouseleave",
                        function () {
                            _.each(elem.find(".legendLabel"), function (i) {
                                var jItem = $(i);
                                jItem.css("font-weight", "normal");
                            });


                            scope.Tooltip = null;

                            // Workaround for rare bug where count text is reset to higher 
                            // unfiltered value when a section of the pie is selected
                            if (scope.ChartData.Chart.length > 1) {
                                scope.Filter.value = angular.copy(scope.OrigFilter.value);
                            }

                            var s = $(this).text();
                            var ss = plot.getData();
                            for (var j = 0; j < ss.length; j++) {
                                if (ss[j].label == s) {
                                    plot.unhighlight(ss[j], 'plothover');
                                    break;
                                }
                            }

                            scope.$apply();
                        }
                    );

                    elem.find('.flot-chart').bind('plothover', function (event, pos, item) {
                        if (item) {
                            _.each(elem.find(".legendLabel"), function (i) {
                                var jItem = $(i);
                                jItem.css("font-weight", "normal"); if (jItem.html() == item.series.label) { jItem.css("font-weight", "bold"); }
                            });

                            scope.Filter.value = item.series.data[0][1];
                            scope.Tooltip = item.series.label;
                        } else {
                            _.each(elem.find(".legendLabel"), function (i) {
                                var jItem = $(i);
                                jItem.css("font-weight", "normal");
                            });

                            scope.Tooltip = null;

                            // Workaround for rare bug where count text is reset to higher 
                            // unfiltered value when a section of the pie is selected
                            if (scope.ChartData.Chart.length > 1) {
                                scope.Filter.value = angular.copy(scope.OrigFilter.value);
                            }
                        }

                        scope.$apply();
                    });

                    elem.find('.flot-chart').bind("plotclick", function (event, pos, obj) {
                        if (!obj) {
                            return;
                        }

                        scope.Filter.filter(obj.series.label);
                        if (!scope.AllColors && scope.Colors.length > 0) {
                            scope.AllColors = angular.copy(scope.Colors);
                        }
                        scope.Colors = [obj.series.color];

                        scope.$apply();
                    });
                };
            }
        };
    })
.directive("focusSelect", function ($timeout) {
    return {
        scope: { Select: "=select", ShouldFocus: "=focusSelect", ShouldDisable: "=isSaving" },
        link: function (scope, element, attrs) {
            //            $timeout(function () { element.focus(); }, 800);

            scope.$watch("Select", function (newValue) {
                if (newValue) {
                    element.select();
                }
                scope.Select = false;
            });
            scope.$watch("ShouldFocus", function (newValue) {
                if (newValue) {
                    element.focus();
                    scope.ShouldFocus = false;
                }
            });
            scope.$watch("ShouldDisable", function (newValue) {
                element.disabled = newValue;

            });
        }
    };
})
.directive('ngEnter', function ($document) {
    return {
        scope: {
            ngEnter: "&"
        },
        link: function (scope, element, attrs) {
            var enterWatcher = function (event) {
                if (event.which === 13) {
                    scope.ngEnter();
                    scope.$apply();
                    console.log('ENTER')
                    event.preventDefault();
                    $document.unbind("keydown keypress", enterWatcher);
                }
            };
            $document.bind("keydown keypress", enterWatcher);
        }
    }
});