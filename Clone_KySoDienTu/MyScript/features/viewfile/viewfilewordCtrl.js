angular
    .module("aims")
    .controller('viewFileWordCtrl', [
        "$scope",
        "$uibModalInstance",
        "idselect",
        function (
            $scope,
            $uibModalInstance,
            idselect) {
            var $ctrl = this;

            $ctrl.sumitformedit = function () {

            }

            $ctrl.ok = function () {
                $ctrl.presult = "0";
            };

            $ctrl.cancel = function () {
                $uibModalInstance.dismiss('cancel');
            };

            getdatafileloading();

            function detectExtension(extensionId, callback) {

            }

            //$ctrl.urldoc = '#';

            function getdatafileloading() {
                var hasExtension = false;
                var editorExtensionId = 'gbkeegbaiigmenfmjfclcdgdpimamgkj';
                var i = chrome.runtime.connect(editorExtensionId);
                debugger
                i.onMessage.addListener(
                    function (request, sender, sendResponse) {
                        debugger
                        if (request) {
                            if (request.message) {
                                if (request.message == "version") {
                                    sendResponse({ version: 1.0 });
                                }
                            }
                        }
                        return true;
                    });
                i.postMessage({
                    message: 'version'
                });
                //i.sendMessage({ message: "version" },
                //    function (reply) {
                //        if (reply) {
                //            debugger
                //            if (reply.version) {
                //                if (reply.version >= requiredVersion) {
                //                    hasExtension = true;
                //                }
                //            }
                //        }
                //        else {
                //            hasExtension = false;
                //        }
                //    });
                //debugger
                //chrome.runtime.sendMessage(editorExtensionId, { message: "version" },
                //  function (response) {
                //      debugger
                //      if (!response.success)
                //          handleError(url);
                //  });
                //var port = chrome.runtime.connect('');
            }
        }
    ]);