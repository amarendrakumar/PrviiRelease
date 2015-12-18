(function () {
    'use strict';
    var prviiAppChannelMessageView = angular.module('prviiApp.channelMessageView', []);

    prviiAppChannelMessageView.controller('channelMessageViewController', ['$rootScope', '$scope', '$http', '$location', '$routeParams', 'ngProgress',
        function ($rootScope, $scope, $http, $location, $routeParams, ngProgress) {

        $scope.show = false;
        ngProgress.start();
        var messageId = $routeParams.messageId;
       // alert('delete...' + messageId);
        var messageData = { ID: messageId };
        //debugger
        $http.post($rootScope.SERVICE_URL + "ChannelMessage/GetChannelMessageById", messageData)
                .success(function (serverResponse, status, headers, config) {
                    // alert('success');
                    //debugger
                    $scope.MessageView = serverResponse;
                    $scope.IsSubcriberMessage = true;
                }).error(function (serverResponse, status, headers, config) {
                    alert(status + " - Error Occured!");
                }
            );
        ngProgress.complete();
        $scope.show = true;



     
       
        $scope.downloadFile = function (AttachId) {
             alert('download..' + AttachId);
            //debugger
            $scope.show = false;
            ngProgress.start();
            var filePath = $rootScope.SERVICE_URL + "ChannelMessage/GetAttachment?AttachId=" + AttachId;
           // window.open(filePath, '_blank', '');
            // window.open(filePath, '_self', '');
            $scope.showiframe = true;
            $scope.downloadFileUrl = filePath;
            ngProgress.complete();
            $scope.show = true;
        }


        $scope.downloadFile1 = function (AttachId) {
            debugger
            // alert('download..' + AttachId);
            $scope.show = false;
            ngProgress.start();
            var filePath = $rootScope.SERVICE_URL + "ChannelMessage/GetAttachment?AttachId=" + AttachId;
            var filePath1 = $rootScope.SERVICE_URL + "ChannelMessage/GetAttachment1?AttachId=" + AttachId;

            // window.open(filePath, '_blank', '');
            // window.open(filePath, '_self', '');
            $scope.downloadFileUrl = filePath;
           

            ngProgress.complete();
            $scope.show = true;
        }




        $scope.ApprovedMessage = function (messageId,channelID)
        {
            $scope.show = false;
            ngProgress.start();
            var messageData = { ID: messageId };
            $http.post($rootScope.SERVICE_URL + "ChannelMessage/ChannelMessageApproved", messageData)
                    .success(function (serverResponse, status, headers, config) {
                        $location.path('/channelMessageList/'+channelID);  
                    }).error(function (serverResponse, status, headers, config) {
                        alert(status + " - Error Occured!");
                    }
                );
            ngProgress.complete();
            $scope.show = true;
        }


        $scope.DeleteMessage = function (messageId, channelId) {
            $scope.show = false;
            ngProgress.start();
            var messageData = { ID: messageId };
            $http.post($rootScope.SERVICE_URL + "ChannelMessage/ChannelMessageDelete", messageData)
                    .success(function (serverResponse, status, headers, config) {
                        $location.path('/channelMessageList/' + channelId);
                    }).error(function (serverResponse, status, headers, config) {
                        alert(status + " - Error Occured!");
                    }
                );
            ngProgress.complete();
            $scope.show = true;
        }
    }]
    )
})();