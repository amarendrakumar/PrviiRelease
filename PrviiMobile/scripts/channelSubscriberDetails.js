(function () {
    'use strict';
    var prviiAppchannelSubscriberDetails = angular.module('prviiApp.channelSubscriberDetails', []);

    prviiAppchannelSubscriberDetails.controller('channelSubscriberDetailsController', ['$rootScope', '$scope', '$http', '$routeParams','ngProgress',
    function ($rootScope, $scope, $http, $routeParams, ngProgress) {
        $scope.show = false;
       ngProgress.start();
        //var subscriberId = $routeParams.subscriberId;
       var subscriberId = $rootScope.subscriberIdDetails;
      // alert(subscriberId);

        $scope.subscriberId = subscriberId;
        var dataToPost = { ID: subscriberId, UserID: $rootScope.USER_ID };
        $http.post($rootScope.SERVICE_URL + "UserProfile/GetUserProfileById", dataToPost)
                    .success(function (serverResponse, status, headers, config) {
                        $scope.SubscriberDetails = serverResponse;
                    }).error(function (serverResponse, status, headers, config) {
                        alert(status + " - Error Occured!");
                    }
                );
         ngProgress.complete();
         $scope.show = true;
    }]
    )
})();