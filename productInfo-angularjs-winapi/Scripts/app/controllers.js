var productcatControllers = angular.module('productcatControllers', []);
productcatControllers.controller('ProductListCtrl', ['$scope', '$routeParams', '$location', '$route', 'productService',
function ($scope, $routeParams, $location, $route, productService) {
    productService.query(function (data) {
        $scope.products = data;
    });
    $scope.deleteProduct = function (product) {
        debugger;
        productService.delete({ id: product.id }, function () {
            $route.reload();
        });
    }
    $scope.newProduct = function () {
        $location.path('/productNew');
    }
}]);
productcatControllers.controller('ProductDetailCtrl', ['$scope', '$routeParams', '$location', 'productService',
function ($scope, $routeParams, $location, productService) {
    productService.get({ id: $routeParams.productId }, function (data) {
        $scope.product = data;
    });
    $scope.backToProduct = function () {
        $location.path('/products');
    }
}]);
productcatControllers.controller('ProductNewCtrl', ['$scope', '$routeParams', '$location', 'productService', '$upload',
function ($scope, $routeParams, $location, productService, $upload) {
    $scope.product = { name: "", imageUrl: "", ProductDetail: { number: 0, price: 0.0, description: "", companyName: "" } };
    
    $scope.addNewProduct = function (picFile, product) {
        if (picFile != null) {
            var file = picFile[0];

            $upload.upload({
                url: '/api/Product',
                data: { product: product },
                file: file,
                progress: function (e) { }
            }).then(function (data, status, headers, config) {
                $location.path('/products');
            });
        }
        else {
            $upload.upload({
                url: '/api/Product',
                data: { product: product },
                progress: function (e) { }
            }).then(function (data, status, headers, config) {
                $location.path('/products');
            });
        }
    };
    $scope.fileUploadOnly = function (files) {

        debugger;
        if (files != null) {
            var file = files[0];

            $upload.upload({
                url: '/api/Product',
                file: file,
                progress: function (e) { }
            }).then(function (data, status, headers, config) {
                $location.path('/products');
            });
        }
    };
    $scope.cancelNewProduct = function () {
        $location.path('/products');
    };
}]);
productcatControllers.controller('ProductUpdateCtrl', ['$scope', '$routeParams', '$location', 'productService', '$upload',
function ($scope, $routeParams, $location, productService, $upload) {
    productService.get({ id: $routeParams.productId }, function (data) {
        $scope.product = data;
    });
    $scope.updateProduct = function (picFile, product) {
        var post = productService.get({}, { id: product.id }, function (data) {
            debugger;
            post.name = product.name;
            post.imageUrl = product.imageUrl;
            post.ProductDetail.number = product.ProductDetail.number;
            post.ProductDetail.price = product.ProductDetail.price;
            post.ProductDetail.description = product.ProductDetail.description;
            post.ProductDetail.companyName = product.ProductDetail.companyName;
            
            if (picFile != null) {
                var file = picFile[0];

                $upload.upload({
                    method:'PUT',
                    url: '/api/Product',
                    data: { product: post },
                    file: file,
                    progress: function (e) { }
                }).then(function (data, status, headers, config) {
                    $location.path('/products');
                });
            }
            else {
                $upload.upload({
                    method: 'PUT',
                    url: '/api/Product',
                    data: { product: product },
                    progress: function (e) { }
                }).then(function (data, status, headers, config) {
                    $location.path('/products');
                });
            }
        });
    }
    $scope.cancelUpdateProduct = function () {
        $location.path('/products');
    }
}]);







