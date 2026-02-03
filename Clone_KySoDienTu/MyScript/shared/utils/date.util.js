angular
    .module('aims.shared.utils')
    .factory('DateUtil', DateUtil);

function DateUtil() {
    return {
        toDateString: function (bd) {
            if (!bd) return null;

            const d = new Date(bd);
            const mm = String(d.getMonth() + 1).padStart(2, '0');
            const dd = String(d.getDate()).padStart(2, '0');

            return `${d.getFullYear()}-${mm}-${dd}`;
        }
    };
}
