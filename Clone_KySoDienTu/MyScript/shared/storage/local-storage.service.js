// Sau này:
// - Đổi sang sessionStorage
// - Thêm encrypt
// - mock unit test
// → chỉ sửa 1 chỗ
angular
    .module('aims.shared')
    .service('LocalStorageService', function () {

        this.set = (key, value) =>
            localStorage.setItem(key, angular.toJson(value));

        this.get = key =>
            angular.fromJson(localStorage.getItem(key));

        this.remove = key =>
            localStorage.removeItem(key);

        this.clear = () =>
            localStorage.clear();
    });
