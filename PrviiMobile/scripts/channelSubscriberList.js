(function () {
    'use strict';
    var prviiAppchannelSubscriberList = angular.module('prviiApp.channelSubscriberList', []);

    prviiAppchannelSubscriberList.controller('channelSubscriberListController', ['$rootScope', '$scope', '$http','$routeParams','ngProgress',
    function ($rootScope, $scope, $http, $routeParams, ngProgress) {
        $scope.show = false;
        ngProgress.start();
        var channelID = $rootScope.USER_ChannelID;
       // alert(channelID);
        var dataToPost = { ID: channelID, UserID: $rootScope.USER_ID };
        $http.post($rootScope.SERVICE_URL + "ChannelSubscribers/GetChannelSubscriberList", dataToPost)
                    .success(function (serverResponse, status, headers, config) {
                        $scope.SubscriberList = serverResponse;
                        $rootScope.channelIDForSubscriber = channelID;
                        $scope.predicate = 'Username';
                    }).error(function (serverResponse, status, headers, config) {
                        alert(status + " - Error Occured!");
                    }
                );
         ngProgress.complete();
         $scope.show = true;

      
         $rootScope.IncludeChannelSubcriber = function (templateURI, subscriberId) {
             $rootScope.srcSubscriber = 'partials/' + templateURI;

             $rootScope.subscriberIdDetails = subscriberId;

             $rootScope.subscriber = true;

         }

         $rootScope.IncludeChannelSubcriberBack = function (templateURI) {
             $rootScope.srcSubscriber = 'partials/' + templateURI;

             $rootScope.subscriberIdDetails = "";

             $rootScope.subscriber = true;

         }

    }]

    )
})();