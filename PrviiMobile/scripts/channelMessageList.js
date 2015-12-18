(function () {
    'use strict';
    var prviiAppchannelMessageList = angular.module('prviiApp.channelMessageList', []);
    
    prviiAppchannelMessageList.controller('channelMessageListController', ['$rootScope', '$scope', '$http','$routeParams','ngProgress',
    function ($rootScope, $scope, $http, $routeParams, ngProgress) {
        $scope.show = false;
        ngProgress.start();
        var channelID = $rootScope.USER_ChannelID;
       // debugger
        var dataToPost = { ID: channelID };
        $http.post($rootScope.SERVICE_URL + "ChannelMessage/GetChannelMessageList", dataToPost)
                    .success(function (serverResponse, status, headers, config) {
                       // debugger 
                        $scope.channelMessageList = serverResponse;
                        $scope.channelID = channelID;
                    }).error(function (serverResponse, status, headers, config) {
                        alert(status + " - Error Occured!");
                    }
                );
        // $scope.channelMessageList = "Amarendra : " + channelID;
        $scope.predicate = '-ScheduledOn';

        ngProgress.complete();
        $scope.show = true;

        $rootScope.includeModifyApprove = function (templateURI, ViewMessageID) {
            $rootScope.srcModify = 'partials/' + templateURI;
            $rootScope.modify = true;
           // debugger
            $rootScope.viewMessageID = ViewMessageID;
        }


        $scope.myFilter = function (channelMessage) {
            if ((channelMessage.Status === 1 )|| (channelMessage.SMSStatus === 5 ))
                return true;
            else if( (channelMessage.Status === 3)|| (channelMessage.SMSStatus === 6))
                return true;
            else if ((channelMessage.Status === 0) || (channelMessage.SMSStatus === 0))
                return true;
            else
                return false;

        };
    }]
    )
})();