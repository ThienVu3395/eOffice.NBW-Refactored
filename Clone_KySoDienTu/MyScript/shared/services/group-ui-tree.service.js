angular
    .module('aims.shared.services')
    .factory('GroupUITreeService', GroupUITreeService);

GroupUITreeService.$inject = [
    'loginservice',
    'blockUI',
    'thongbao'
];

function GroupUITreeService(loginservice, blockUI, thongbao) {

    return {
        getAllUsersGroups,
        getAllUsersGroupsTree,
        getUsersActionID,
        deleteGroup,
        insertGroup
    };

    /* ===================== API ===================== */

    function getAllUsersGroups(isView, permissionAction, moduleKey) {
        return loginservice.postdata(
            'api/getCore/Core_GetAllUsersGroupsUI',
            $.param({
                valint1: isView,
                valint2: -1,
                valstring1: permissionAction,
                valstring2: moduleKey
            })
        );
    }

    function getAllUsersGroupsTree(isView) {
        return loginservice.postdata(
            'api/getCore/Core_GetAllUsersGroupsTree',
            $.param({
                valint1: isView,
                valint2: -1
            })
        );
    }

    function getUsersActionID(id) {
        return loginservice.postdata(
            'api/getCore/Core_GetUsersActionID',
            $.param({ valint1: id })
        );
    }

    function deleteGroup(id) {
        blockUI.start();
        return loginservice
            .postdata(
                'api/getCore/Core_DeleteUITreeGroups',
                $.param({ valint1: id })
            )
            .then(onSuccess('Xóa thành công'))
            .finally(blockUI.stop);
    }

    function insertGroup(item) {
        blockUI.start();
        return loginservice
            .postdata(
                'api/getCore/Core_InsertUITreeGroups',
                $.param(item)
            )
            .then(onSuccess('Thêm thành công'))
            .finally(blockUI.stop);
    }

    /* ===================== helpers ===================== */

    function onSuccess(message) {
        return function () {
            thongbao.noImage(message, '');
        };
    }
}
