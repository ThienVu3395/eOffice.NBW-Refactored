angular
    .module('aims.core')
    .config(blockUIConfigure)
    .config(chartJsConfigure)
    .config(compileConfigure);

function blockUIConfigure(blockUIConfig) {
    //'blockUI';
    //blockUIConfig.message('Fun with config');
    blockUIConfig.delay = 200;
    blockUIConfig.autoBlock = true;
    blockUIConfig.blockBrowserNavigation = true;
}

function chartJsConfigure(ChartJsProvider) {
    ChartJsProvider.setOptions({
        colors: [
            '#803690', '#00ADF9', '#DCDCDC',
            '#46BFBD', '#FDB45C', '#949FB1', '#4D5360'
        ]
    });
}

function compileConfigure($compileProvider) {
    $compileProvider.debugInfoEnabled(false);
}
