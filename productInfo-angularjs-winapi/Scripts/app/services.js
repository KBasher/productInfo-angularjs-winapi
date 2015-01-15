
var phonecatServices = angular.module('productcatServices', ['ngResource']);
phonecatServices.factory("productService", function ($resource) {
    return $resource(
        "/api/Product/:id",
        { id: "@id" },
        {
            "update": { method: "PUT" }
        }
    );
});
