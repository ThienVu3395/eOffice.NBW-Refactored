angular
    .module('aims.auth')
    .factory('UserProfileService', UserProfileService);

function UserProfileService() {
    const KEYS = {
        USERNAME: 'username',
        ACCESS_TOKEN: 'accessToken',
        FIREBASE_TOKEN: 'access_tokenFirebase',
        REFRESH_TOKEN: 'refreshToken',
        ROLE: 'roleName',
        EXPIRE_AT: 'setupTime',

        FULLNAME: 'fileusername',
        EMPLOYEE_ID: 'manhanvien',
        DEPARTMENT: 'bophan',
        POSITION: 'chucvu',
        AVATAR: 'ulrimage'
    };

    const TOKEN_EXPIRE_DAYS = 14;

    // ========= AUTH =========
    function setAuthProfile({
        username,
        accessToken,
        firebaseToken,
        refreshToken,
        roleName
    }) {
        localStorage.setItem(KEYS.USERNAME, username);
        localStorage.setItem(KEYS.ACCESS_TOKEN, accessToken);
        localStorage.setItem(KEYS.FIREBASE_TOKEN, firebaseToken);
        localStorage.setItem(KEYS.REFRESH_TOKEN, refreshToken);
        localStorage.setItem(KEYS.ROLE, roleName);

        const expireAt = Date.now() + TOKEN_EXPIRE_DAYS * 24 * 60 * 60 * 1000;
        localStorage.setItem(KEYS.EXPIRE_AT, expireAt);
    }

    function isExpired() {
        const expireAt = parseInt(localStorage.getItem(KEYS.EXPIRE_AT));
        return !expireAt || Date.now() > expireAt;
    }

    function getProfile() {
        if (isExpired()) {
            clear();
            return { isLoggedIn: false };
        }

        return {
            isLoggedIn: !!localStorage.getItem(KEYS.ACCESS_TOKEN),
            username: localStorage.getItem(KEYS.USERNAME),
            accessToken: localStorage.getItem(KEYS.ACCESS_TOKEN),
            firebaseToken: localStorage.getItem(KEYS.FIREBASE_TOKEN),
            refreshToken: localStorage.getItem(KEYS.REFRESH_TOKEN),
            roleName: localStorage.getItem(KEYS.ROLE),

            fullname: localStorage.getItem(KEYS.FULLNAME),
            employeeId: localStorage.getItem(KEYS.EMPLOYEE_ID)
        };
    }

    // ========= EXTENDED PROFILE =========
    function setExtendedProfile({
        fullname,
        avatar,
        employeeId,
        department,
        position
    }) {
        localStorage.setItem(KEYS.FULLNAME, fullname);
        localStorage.setItem(KEYS.AVATAR, avatar);
        localStorage.setItem(KEYS.EMPLOYEE_ID, employeeId);
        localStorage.setItem(KEYS.DEPARTMENT, department);
        localStorage.setItem(KEYS.POSITION, position);
    }

    function getExtendedProfile() {
        return {
            fullname: localStorage.getItem(KEYS.FULLNAME),
            avatar: localStorage.getItem(KEYS.AVATAR),
            employeeId: localStorage.getItem(KEYS.EMPLOYEE_ID),
            department: localStorage.getItem(KEYS.DEPARTMENT),
            position: localStorage.getItem(KEYS.POSITION)
        };
    }

    // ========= COMMON =========
    function clear() {
        Object.values(KEYS).forEach(k => localStorage.removeItem(k));
    }

    function saveItem(key, value) {
        localStorage.setItem(`item_${key}`, value);
    }

    function getItem(key) {
        return localStorage.getItem(`item_${key}`);
    }

    function removeItem(key) {
        localStorage.removeItem(`item_${key}`);
    }

    return {
        setAuthProfile,
        getProfile,
        setExtendedProfile,
        getExtendedProfile,
        clear,
        saveItem,
        getItem,
        removeItem
    };
}
