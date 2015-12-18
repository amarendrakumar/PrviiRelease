(function () {
    'use strict';
    var prviiAppappleIndex = angular.module('prviiApp.appleIndex', []);

    prviiAppappleIndex.controller('appleIndexController', ['$scope', '$rootScope', '$http', '$location', 'ngProgress',
function ($scope, $rootScope, $http, $location, ngProgress) {

    $scope.ExistingUser = function ()
    {
         //alert("Existing User");
        debugger
       // navigator.notification.alert("Test Notification alert", callbackfunctiontest, "Prvii alert", "Done");
        
        $location.path('/login');
    }

    //function callbackfunctiontest()
    //{
    //    debugger
    //    alert("callbackfunctiontest User");
    //}
    $scope.NewUser = function () {
       // alert("New User");
       // $location.path('/InAppChannelList');
        $location.path('/AppleRegistration');
    }


    $scope.Continue = function () {
        alert($scope.EmailID);
        var dataToPost = { Email: $scope.EmailID };
        $http.post($rootScope.SERVICE_URL + "userprofile/authenticate", dataToPost)
                .success(function (serverResponse, status, headers, config) {
                    alert("success");
                }).error(function (serverResponse, status, headers, config) {
                    alert(status + " - Error Occured!");
                    $location.path('/logout');
                });
        
        $location.path('/InAppChannelList');
    }
}]);

})();