
(function () {
    "use strict"
    var app = angular
        .module("oamsapp", ["aims.service", 'ui.tinymce', 'SignalR', 'summernote' /*"chart.js"*/])
        //.constant("appSettings", Settings)
        .config(['$compileProvider', function ($compileProvider) {
            $compileProvider.debugInfoEnabled(false);
        }])
        //.config(function (ChartJsProvider) {
        //    ChartJsProvider.setOptions({ colors: ['#803690', '#00ADF9', '#DCDCDC', '#46BFBD', '#FDB45C', '#949FB1', '#4D5360'] });
        //})
        .filter('unsafe', function ($sce) {
            return function (val) {
                return $sce.trustAsHtml(val);
            }
        })
        ;
})();
