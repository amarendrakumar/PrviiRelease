(function () {
    'use strict';
    var prviiAppchannelSubscriberAllMessageList = angular.module('prviiApp.channelSubscriberAllMessageList', []);

    prviiAppchannelSubscriberAllMessageList.controller('channelSubscriberAllMessageListController', ['$rootScope', '$scope', '$http', '$routeParams', 'ngProgress',
    function ($rootScope, $scope, $http, $routeParams, ngProgress) {
        $scope.show = false;
        ngProgress.start();
        // debugger
        var channelID = $rootScope.channelId;
       // var channelID = $rootScope.channelId;
        // debugger
        var dataToPost = { ID: channelID, UserID: $rootScope.USER_ID };
        $http.post($rootScope.SERVICE_URL + "ChannelMessage/GetChannelSubscriberAllMessageByChannelID", dataToPost)
                    .success(function (serverResponse, status, headers, config) {
                           //debugger 
                        $scope.channelSubscriberAllMessageList = serverResponse;
                        $scope.channelID = channelID;

                    }).error(function (serverResponse, status, headers, config) {
                        alert(status + " - Error Occured!");
                    }
                );
        ngProgress.complete();
        $scope.show = true;
        // $scope.channelMessageList = "Amarendra : " + channelID;

        $scope.myFilter = function (channelSubscriberAllMessageList) {
            // debugger
            //var dd = new Date(channelMessageList.DeliveredOn);
            //var messageDate = dd.getDay();
            //var cd = new Date();
            //cd.setDate(cd.getDate() - 7);
            //var daysa1 = cd.getDate();
            //if (messageDate >= daysa1)
            //    return true;          
            //else
            //    return false;

            return true;

        };
    }]
    )
})();