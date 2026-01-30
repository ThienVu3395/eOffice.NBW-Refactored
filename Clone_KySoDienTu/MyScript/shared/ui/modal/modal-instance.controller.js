angular
    .module('aims.shared.ui')
    .controller('ModalInstanceCtrl', ModalInstanceCtrl);

ModalInstanceCtrl.$inject = ['$uibModalInstance'];

function ModalInstanceCtrl($uibModalInstance) {

    var $ctrl = this;

    $ctrl.ok = function (result) {
        $uibModalInstance.close(result);
    };

    $ctrl.cancel = function () {
        $uibModalInstance.dismiss('cancel');
    };
}
