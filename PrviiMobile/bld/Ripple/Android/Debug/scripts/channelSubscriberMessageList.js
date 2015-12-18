(function () {
    'use strict';
    var prviiAppchannelSubscriberMessageList = angular.module('prviiApp.channelSubscriberMessageList', []);

    prviiAppchannelSubscriberMessageList.controller('channelSubscriberMessageListController', ['$rootScope', '$scope', '$http', '$routeParams', 'ngProgress',
    function ($rootScope, $scope, $http, $routeParams, ngProgress) {
        $scope.show = false;
        ngProgress.start();
         debugger
        $scope.WeekNo = 2;
        var channelID = $rootScope.channelId;

         $scope.SearchText ="";
        //var channelID = $routeParams.channelId;
        var curr = new Date(); // get current date
        var first = curr.getDate();// - curr.getDay(); // First day is the day of the month - the day of the week
        var last = first - 14; // last day is the first day + 6

        var d = new Date();
        var DD = d.getDate();

        var firstday = new Date(curr.setDate(first));//.toUTCString();
        var lastday = new Date(curr.setDate(last));//.toUTCString();

       
        var fDD = lastday.getDate();
        if ((1 <= fDD) && (fDD <= 9))
            fDD = '0' + fDD;

        var fMM = lastday.getMonth() + 1;
        if ((0 <= fMM) && (fMM <= 9))
            fMM = '0' + fMM;

        var fYYYY = lastday.getFullYear();
               


      
        var DD = firstday.getDate();
        if ((1 <= DD) && (DD <= 9))
            DD = '0' + DD;

        var MM = firstday.getMonth() + 1;
        if ((0 <= MM) && (MM <= 9))
            MM = '0' + MM;

        var YYYY = firstday.getFullYear();

       



        // debugger
        $scope.FromDate  = new Date(fYYYY, fMM - 1, fDD, '00', '00', 0);
        $scope.ToDate = new Date(YYYY, MM - 1, DD, '00', '00', 0);
       

         debugger

        var dataToPost = { ID: channelID, UserID: $rootScope.USER_ID };
        $http.post($rootScope.SERVICE_URL + "ChannelMessage/GetChannelSubscriberMessages", dataToPost)
                    .success(function (serverResponse, status, headers, config) {
                          // debugger 
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
            var dataToPost = { ID: channelID, UserID: $rootScope.USER_ID, WeekNo: weekNo };
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
            alert("Search Message");
            $scope.show = false;
            ngProgress.start();
            debugger
            var channelID = $rootScope.channelId;
            //var channelID = $routeParams.channelId;            
            $scope.fromDate = $scope.FromDate;
            $scope.toDate = $scope.ToDate;
             debugger
             var dataToPost = { ID: channelID, UserID: $rootScope.USER_ID, FromDate: $scope.fromDate, ToDate: $scope.toDate, SearchText: $scope.SearchText };
            $http.post($rootScope.SERVICE_URL + "ChannelMessage/GetChannelSubscriberAllMessageByChannelIDSubscriberSearch", dataToPost)
                        .success(function (serverResponse, status, headers, config) {
                              debugger 
                           // $scope.channelSubscriberAllMessageList = serverResponse;
                            $scope.channelMessageList = serverResponse;

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