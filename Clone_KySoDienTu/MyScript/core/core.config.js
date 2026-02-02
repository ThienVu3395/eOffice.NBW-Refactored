// Đây mới là core config đúng nghĩa
angular
    .module('aims.core')
    .config(compileConfigure);

function compileConfigure($compileProvider) {
    $compileProvider.debugInfoEnabled(false);
}
