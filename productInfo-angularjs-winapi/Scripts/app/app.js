var productcatApp = angular.module('productcatApp', [
'ngRoute',
'productcatControllers',
'productcatServices',
'angularFileUpload'
]);
productcatApp.config(['$routeProvider',
function ($routeProvider) {
    $routeProvider.
    when('/products', {
        templateUrl: 'Home/ProductList',
        controller: 'ProductListCtrl'
    }).
    when('/products/:productId', {
        templateUrl: 'Home/ProductDetail',
        controller: 'ProductDetailCtrl'
    }).
    when('/productNew', {
        templateUrl: 'Home/ProductNew',
        controller: 'ProductNewCtrl'
    }).
    when('/productUpdate/:productId', {
        templateUrl: 'Home/ProductUpdate',
        controller: 'ProductUpdateCtrl'
    }).
    otherwise({
        redirectTo: '/products'
    });
}]);




