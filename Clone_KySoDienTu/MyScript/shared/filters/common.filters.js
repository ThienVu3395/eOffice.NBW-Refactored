angular
    .module('aims.shared.filters')
    .filter('unsafe', unsafeFilter)
    .filter('hanxuly', hanXuLyFilter)
    .filter('hanxulylabel', hanXuLyLabelFilter)
    .filter('HanXuLyCongVan', hanXuLyCongVanFilter)
    .filter('timeago', timeAgoFilter)
    .filter('notifyMenu', notifyMenuFilter)
    .filter('propsFilter', propsFilter);

/* ===================== unsafe ===================== */
function unsafeFilter($sce) {
    return function (val) {
        return $sce.trustAsHtml(val);
    };
}

/* ===================== hanxuly ===================== */
function hanXuLyFilter() {
    return function (time, local) {
        if (!time) return 'never';

        local = local || Date.now();

        time = normalizeTime(time);
        local = normalizeTime(local);

        if (!time || !local) return;

        var offset = Math.abs((local - time) / 1000);
        var DAY = 86400;

        if (time < local) return 'text-danger';
        if (offset > DAY * 2) return 'text-success';
        if (offset < DAY * 2) return 'text-warning';
    };
}

/* ===================== hanxulylabel ===================== */
function hanXuLyLabelFilter() {
    return function (time) {
        if (!time) return false;
        var today = new Date();
        today = new Date(today.getFullYear(), today.getMonth(), today.getDate(), 0, 0);
        return new Date(time) < today;
    };
}

/* ===================== HanXuLyCongVan ===================== */
function hanXuLyCongVanFilter() {
    return function (time, trangthai) {
        if (!time) return 'never';

        var local = Date.now();
        time = normalizeTime(time);

        if (!time) return;

        if (time <= local) {
            if (trangthai == 3) return null;
            return 'text-danger';
        }

        return null;
    };
}

/* ===================== timeago ===================== */
function timeAgoFilter() {
    return function (time, local, raw) {
        if (!time) return 'never';

        local = local || Date.now();
        time = normalizeTime(time);
        local = normalizeTime(local);

        if (!time || !local) return;

        var offset = Math.abs((local - time) / 1000);

        var MINUTE = 60,
            HOUR = 3600,
            DAY = 86400,
            WEEK = 604800,
            MONTH = 2592000,
            YEAR = 31556926,
            DECADE = 315569260;

        var span;

        if (offset <= MINUTE) span = ['', raw ? 'now' : 'less than a minute'];
        else if (offset < MINUTE * 60) span = [Math.round(offset / MINUTE), 'min'];
        else if (offset < HOUR * 24) span = [Math.round(offset / HOUR), 'hr'];
        else if (offset < DAY * 7) span = [Math.round(offset / DAY), 'day'];
        else if (offset < WEEK * 4) span = [Math.round(offset / WEEK), 'week'];
        else if (offset < MONTH * 12) span = [Math.round(offset / MONTH), 'month'];
        else if (offset < YEAR * 10) span = [Math.round(offset / YEAR), 'year'];
        else span = ['', 'a long time'];

        if (span[0] > 1) span[1] += 's';
        span = span.join(' ');

        if (raw === true) return span;
        return time <= local ? span + ' ago' : 'in ' + span;
    };
}

/* ===================== notifyMenu ===================== */
function notifyMenuFilter() {
    return function (input, item) {
        if (!angular.isArray(input)) return [];

        for (var i = 0; i < input.length; i++) {
            if (input[i].ID == item && input[i].value > 0) {
                return input[i];
            }
        }
        return [];
    };
}

/* ===================== propsFilter ===================== */
function propsFilter() {
    return function (items, props) {
        var out = [];

        if (angular.isArray(items)) {
            var keys = Object.keys(props);

            items.forEach(function (item) {
                var itemMatches = false;

                for (var i = 0; i < keys.length; i++) {
                    var prop = keys[i];
                    var text = props[prop].toLowerCase();
                    if (item[prop].toString().toLowerCase().indexOf(text) !== -1) {
                        itemMatches = true;
                        break;
                    }
                }

                if (itemMatches) {
                    out.push(item);
                }
            });
        } else {
            // Let the output be the input untouched
            out = items;
        }

        return out;
    };
}

/* ===================== helpers ===================== */
function normalizeTime(value) {
    if (angular.isDate(value)) return value.getTime();
    if (typeof value === 'string') return new Date(value).getTime();
    if (typeof value === 'number') return value;
    return null;
}
