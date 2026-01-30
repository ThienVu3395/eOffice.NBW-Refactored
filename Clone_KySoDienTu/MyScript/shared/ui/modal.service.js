angular
    .module('aims.shared.ui')
    .factory('ModalService', ModalService);

ModalService.$inject = ['$uibModal', '$window', '$document'];

function ModalService($uibModal, $window, $document) {
    var modalStack = [];          // 🧠 Stack quản lý modal
    var isHandlingPopState = false;
    var isBackButtonDismiss = false;

    function handlePopState() {
        if (modalStack.length > 0) {
            isHandlingPopState = true;
            isBackButtonDismiss = true;

            var topModal = modalStack[modalStack.length - 1];
            if (topModal) {
                topModal.dismiss('back button');
            }

            setTimeout(function () {
                isHandlingPopState = false;
            }, 100);
        } else {
            history.back();
        }
    }

    $window.addEventListener('popstate', handlePopState);

    function open(options) {
        var parentElem = angular.element(
            $document[0].querySelector('.content-wrapper')
        );

        var modalConfig = {
            animation: true,
            keyboard: false,
            ariaLabelledBy: options.ariaLabelledBy || 'modal-title',
            ariaDescribedBy: options.ariaDescribedBy || 'modal-body',
            controller: options.controller,
            controllerAs: options.controllerAs || '$ctrl',
            size: options.size || 'lg',
            backdrop: options.backdrop || 'static',
            appendTo: parentElem,
            resolve: options.resolve || {}
        };

        if (options.template) {
            modalConfig.template = options.template;
        } else if (options.templateUrl) {
            modalConfig.templateUrl = options.templateUrl;
        }

        var modalInstance = $uibModal.open(modalConfig);

        history.pushState({ modalOpen: true }, '');
        modalStack.push(modalInstance);

        modalInstance.result.finally(function () {
            var index = modalStack.indexOf(modalInstance);
            if (index !== -1) {
                modalStack.splice(index, 1);
            }

            if (!isBackButtonDismiss &&
                modalInstance === modalStack[modalStack.length]) {

                if (history.state && history.state.modalOpen) {
                    history.back();
                }
            }

            isBackButtonDismiss = false;
        });

        return modalInstance.result;
    }

    function getModalCount() {
        return modalStack.length;
    }

    return {
        open: open,
        getModalCount: getModalCount
    };
}
