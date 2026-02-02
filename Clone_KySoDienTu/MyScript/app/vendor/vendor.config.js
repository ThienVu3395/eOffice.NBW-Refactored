// Vendor config đi với vendor module, dùng để cấu hình các thư viện ngoài
angular
    .module('aims.vendor')
    .config(blockUIConfigure)
    .config(chartJsConfigure);

function blockUIConfigure(blockUIConfig) {
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