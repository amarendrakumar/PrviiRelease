(function () {
    'use strict';
    var prviiAppsubscriberChannelList = angular.module('prviiApp.subscriberChannelList', []);

    prviiAppsubscriberChannelList.controller('subscriberChannelListController', ['$rootScope', '$scope', '$http', '$routeParams','ngProgress',
    function ($rootScope, $scope, $http, $routeParams, ngProgress) {
        $scope.show = false;
        ngProgress.start();
         //   debugger
          // var subscriberId = $routeParams.subscriberId;
           var subscriberId = $rootScope.USER_ID;
        var dataToPost = { ID: subscriberId };
      // debugger
        //GetChannels
        $http.post($rootScope.SERVICE_URL + "channel/GetSubscribChannelList", dataToPost)
                    .success(function (serverResponse, status, headers, config) {
                        $scope.channelList = serverResponse;
                       // debugger
                    }).error(function (serverResponse, status, headers, config) {
                        alert(status + " - Error Occured!");
                    }
                );
        ngProgress.complete();
        $scope.show = true;


      


        $scope.UnSubscribe= function UnSubscribe(channelID)
        {
            if (confirm('Are you sure, you want to UnSubscribe this Celebrity?'))
            {
                $scope.show = false;
                ngProgress.start();

                var subscriberId = $rootScope.USER_ID;
                var dataToPost = { ChannelID: channelID, UserID: subscriberId };

                $http.post($rootScope.SERVICE_URL + "ShoppingCart/ChannelUnsubscribe", dataToPost)
                            .success(function (serverResponse, status, headers, config) {
                                $scope.show = false;
                                ngProgress.start();
                                var dataToPost1 = { ID: subscriberId };
                                // debugger
                                //GetChannels
                                $http.post($rootScope.SERVICE_URL + "channel/GetSubscribChannelList", dataToPost1)
                                            .success(function (serverResponse, status, headers, config) {
                                                $scope.channelList = serverResponse;
                                                //  debugger
                                            }).error(function (serverResponse, status, headers, config) {
                                                alert(status + " - Error Occured!");
                                            }
                                        );
                                ngProgress.complete();
                                $scope.show = true;

                            }).error(function (serverResponse, status, headers, config) {
                                alert(status + " - Error Occured!");
                            }
                        );
                ngProgress.complete();
                $scope.show = true;
            }

            
          
        }


    }]
    )

  
})();