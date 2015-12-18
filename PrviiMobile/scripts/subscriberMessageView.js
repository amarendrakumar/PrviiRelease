(function () {
    'use strict';
    var prviiAppsubscriberMessageView = angular.module('prviiApp.subscriberMessageView', []);

    prviiAppsubscriberMessageView.controller('subscriberMessageViewController', ['$rootScope', '$scope', '$http', '$location', '$routeParams', 'ngProgress',
        function ($rootScope, $scope, $http, $location, $routeParams, ngProgress) {

            $scope.show = false;
            ngProgress.start();
            var messageId = $rootScope.messageId;
            //var messageId = $routeParams.messageId;
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
                   
            
        }]
    )
})();