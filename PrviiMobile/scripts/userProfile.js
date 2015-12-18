(function () {
    'use strict';
    var prviiAppuserProfile = angular.module('prviiApp.userProfile', []);

    prviiAppuserProfile.controller('userProfileController', ['$rootScope', '$scope', '$http', '$routeParams', 'ngProgress','$location',
    function ($rootScope, $scope, $http, $routeParams, ngProgress, $location) {
        $scope.show = false;
        ngProgress.start();

        var dataToPost = { ID: $rootScope.USER_ID };
        $http.post($rootScope.SERVICE_URL + "UserProfile/GetUserProfileById", dataToPost)
                    .success(function (serverResponse, status, headers, config) {
                        $scope.userDetails = serverResponse;
                    }).error(function (serverResponse, status, headers, config) {
                        alert(status + " - Error Occured!");
                    }
                );
        ngProgress.complete();
        $scope.show = true;
    }]
    )
})();