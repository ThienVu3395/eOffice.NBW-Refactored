angular.module("oamsapp")
    .factory('ModalService', function ($uibModal, $window, $document) {
        var modalStack = []; // 🧠 Stack để quản lý các modal mở

        var isHandlingPopState = false; // 🧠 Cờ chặn popstate đúp

        var isBackButtonDismiss = false; // 🧠 NEW: Flag để biết có phải do back button không

        //function handlePopState(event) {
        //    if (modalStack.length > 0) {
        //        isHandlingPopState = true;
        //        isBackButtonDismiss = true; // 🔥 Đánh dấu là dismiss từ back button

        //        var topModal = modalStack[modalStack.length - 1];
        //        if (topModal) {
        //            topModal.dismiss('back button');
        //        }

        //        setTimeout(function () {
        //            isHandlingPopState = false;
        //        }, 100);
        //    }
        //    else {
        //        history.back(); // Không còn modal, cho browser back tự nhiên
        //    }
        //}

        //$window.addEventListener('popstate', handlePopState);

        return {
            open: function (options) {
                var parentElem = angular.element($document[0].querySelector('.content-wrapper'));

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

                // ✅ Hỗ trợ cả template và templateUrl
                if (options.template) {
                    modalConfig.template = options.template;
                }
                else if (options.templateUrl) {
                    modalConfig.templateUrl = options.templateUrl;
                }

                var modalInstance = $uibModal.open(modalConfig);

                // 🧠 Cứ mở mỗi modal, push thêm 1 state vào history
                history.pushState({ modalOpen: true }, '');

                modalStack.push(modalInstance);

                modalInstance.result.finally(function () {
                    var index = modalStack.indexOf(modalInstance);
                    if (index !== -1) {
                        modalStack.splice(index, 1);
                    }

                    // 🧠 Chỉ history.back() nếu:
                    // - Không phải dismiss bằng back button
                    // - Và modal này là modal trên cùng stack tại thời điểm đóng
                    if (!isBackButtonDismiss && modalInstance === modalStack[modalStack.length]) {
                        if (history.state && history.state.modalOpen) {
                            history.back();
                        }
                    }

                    isBackButtonDismiss = false; // 🔥 Reset lại flag sau khi xử lý xong
                });

                return modalInstance.result;
            },
            getModalCount: function () {
                return modalStack.length;
            }
        };
    })