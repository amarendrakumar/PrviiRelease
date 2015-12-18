(function () {
    'use strict';
    var channelSubscriberPastWeekMessageList = angular.module('prviiApp.channelSubscriberPastWeekMessageList', []);

    channelSubscriberPastWeekMessageList.controller('channelSubscriberPastWeekMessageListController', ['$rootScope', '$scope', '$http', '$routeParams', 'ngProgress',
    function ($rootScope, $scope, $http, $routeParams, ngProgress) {
        $scope.show = false;
        ngProgress.start();
         debugger
        $scope.WeekNo = 2;
        var channelID = $rootScope.channelId;
        //var channelID = $routeParams.channelId;
         debugger

         var dataToPost = { ID: channelID, UserID: $rootScope.USER_ID, WeekNo: $scope.WeekNo };
        $http.post($rootScope.SERVICE_URL + "ChannelMessage/GetChannelSubscriberMessagesPastWeek", dataToPost)
                    .success(function (serverResponse, status, headers, config) {
                           debugger 
                        $scope.channelMessageList = serverResponse;
                        $scope.channelID = channelID;

                    }).error(function (serverResponse, status, headers, config) {
                        alert(status + " - Error Occured!");
                    }
                );
        ngProgress.complete();
        $scope.show = true;
        // $scope.channelMessageList = "Amarendra : " + channelID;

        $scope.myFilter = function (channelMessageList) {
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

        $scope.SearchPastMessage = function () {
            $scope.show = false;
            ngProgress.start();
            debugger
            var weekNo = $scope.WeekNo;
            var dataToPost = { ID: channelID, UserID: $rootScope.USER_ID, WeekNo: $scope.WeekNo };
            $http.post($rootScope.SERVICE_URL + "ChannelMessage/GetChannelSubscriberMessagesPastWeek", dataToPost)
                        .success(function (serverResponse, status, headers, config) {
                                debugger 
                            $scope.channelMessageList = serverResponse;
                            $scope.channelID = channelID;

                        }).error(function (serverResponse, status, headers, config) {
                            alert(status + " - Error Occured!");
                        }
        );
            ngProgress.complete();
            $scope.show = true;
        }


        $scope.Searchrecult = function () {
            //alert("Search Message");
            $scope.show = false;
            ngProgress.start();
             debugger
            var channelID = $rootScope.channelId;
            //var channelID = $routeParams.channelId;
            var searchText = $scope.SearchText;
            var fromDate = $scope.FromDate;
            var toDate = $scope.ToDate;
             debugger
            var dataToPost = { ID: channelID, UserID: $rootScope.USER_ID, FromDate: fromDate, ToDate: toDate, SearchText: searchText };
            $http.post($rootScope.SERVICE_URL + "ChannelMessage/GetChannelSubscriberAllMessageByChannelIDSubscriberSearch", dataToPost)
                        .success(function (serverResponse, status, headers, config) {
                            //    debugger 
                            $scope.channelSubscriberAllMessageList = serverResponse;
                            $scope.channelID = channelID;

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