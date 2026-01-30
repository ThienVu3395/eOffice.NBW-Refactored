angular
    .module('aims.shared.ui')
    .controller('ModalConfirmCtrl', ModalConfirmCtrl);

ModalConfirmCtrl.$inject = [
    '$uibModalInstance',
    'confirmMessage',
    'ModalService'
];

function ModalConfirmCtrl(
    $uibModalInstance,
    confirmMessage,
    ModalService
) {

    var $ctrl = this;

    $ctrl.message = confirmMessage || 'Bạn có chắc chắn không?';

    $ctrl.modalCount = ModalService.getModalCount();

    $ctrl.ok = function () {
        $uibModalInstance.close(true);
    };

    $ctrl.cancel = function () {
        $uibModalInstance.dismiss('cancel');
    };
}
