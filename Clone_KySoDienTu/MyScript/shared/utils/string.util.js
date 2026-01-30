angular
    .module('aims.shared.utils')
    .factory('StringUtil', StringUtil);

function StringUtil() {
    function formatDonViName(name) {
        if (!name) return '';
        return name.replace(/\b([A-ZĐ])\.\s+/g, '$1.');
    }

    return {
        formatDonViName
    };
}
