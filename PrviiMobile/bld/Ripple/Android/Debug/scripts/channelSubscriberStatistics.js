(function () {
    'use strict';
    var prviiAppchannelSubscriberStatistics = angular.module('prviiApp.channelSubscriberStatistics', []);

    prviiAppchannelSubscriberStatistics.controller('channelSubscriberStatisticsController', ['$rootScope', '$scope', '$http', '$routeParams','ngProgress',
    function ($rootScope, $scope, $http, $routeParams, ngProgress) {
        var channelId = $rootScope.USER_ChannelID;
        //alert(channelId);
        //   debugger
        // $scope.Ismonth = speriod;
        var currentYear = (new Date).getFullYear();
        var currentMonth = (new Date).getMonth();
        var currentQuarter = Math.floor(((currentMonth + 11) / 3) % 4) + 1;

        $scope.IsQuarter = false;
        $scope.IsMonth = false;
        $scope.IsWeek = false;
        $scope.IsYear = false;

        $scope.Reports = [{ name: '--Select--' }, { name: 'Added' }, { name: 'Lost' }, { name: 'Net' }];
        $scope.Periods = [{ name: '--Period--' }, { name: 'Today' }, { name: 'Week' }, { name: 'Month' }, { name: 'Quarter' }, { name: 'Year' }];
        $scope.Months = [{ name: '--Month--', id: '0' },
                        { name: 'January', Id: '1' },
                         { name: 'February', Id: '2' },
                         { name: 'March', Id: '3' },
                         { name: 'April', Id: '4' },
                         { name: 'May', Id: '5' },
                         { name: 'June', Id: '6' },
                         { name: 'July', Id: '7' },
                         { name: 'August', Id: '8' },
                         { name: 'September', Id: '9' },
                         { name: 'October', Id: '10' },
                         { name: 'November', Id: '11' },
                         { name: 'December', Id: '12' }];

        $scope.Quarters = [{ name: '--Quarter--', id: '0' },
                            { name: '1st', Id: '1' },
                            { name: '2nd', Id: '2' },
                            { name: '3rd', Id: '3' },
                            { name: '4th', Id: '4' }];

        $scope.Weeks = [{ name: '--Week--', id: '0' },
                           { name: '1st', Id: '1' },
                           { name: '2nd', Id: '2' },
                           { name: '3rd', Id: '3' },
                           { name: '4th', Id: '4' },
                           { name: '5th', Id: '5' }];

        $scope.Years = [{ name: '--Year--'},
                           { name: currentYear-1 },
                           { name: currentYear }
                           //, { name: currentYear + 1 }
        ];

        $scope.SpecificPeriods = [{ name: '--Specific Period--' },
       { name: 'Today' },
       { name: 'Last Week' },
       { name: 'Last Month' },
       { name: 'This Month' }];

      

        // $scope.smonth = currentMonth + 1;
        // $scope.squarter = currentQuarter;
        $scope.show = false;
        ngProgress.start();
        $scope.channelId = channelId;
        var dataToPost = { ID: channelId };
        $http.post($rootScope.SERVICE_URL + "ChannelSubscribers/GetActiveSubscribers", dataToPost)
                    .success(function (serverResponse, status, headers, config) {
                        $scope.ActiveSubscribers = serverResponse;
                    }).error(function (serverResponse, status, headers, config) {
                        alert(status + " - Error Occured!");
                    }
                );
          ngProgress.complete();
         $scope.show = true;

         $scope.changeReportType = function () {
             // debugger
             $scope.ActiveNetSubscribers = 0;
           
         }


        $scope.changeperiod = function () {
            // debugger
            $scope.ActiveNetSubscribers = 0;
            var periodType = $scope.Speriod.name;

            if (periodType == 'Month') {
                $scope.IsQuarter = false;
                $scope.IsMonth = true;
                $scope.IsWeek = false;
                $scope.IsYear = false;

            }
            else if (periodType == 'Quarter') {
                $scope.IsQuarter = true;
                $scope.IsMonth = false;
                $scope.IsWeek = false;
                $scope.IsYear = false;
            }
            else if (periodType == 'Week') {
                $scope.IsWeek = true;
                $scope.IsQuarter = false;
                $scope.IsMonth = false;
                $scope.IsYear = false;
            }
            else if (periodType == 'Year') {
                $scope.IsYear = true;
                $scope.IsMonth = false;
                $scope.IsWeek = false;
                $scope.IsQuarter = false;
            }
            else {
                $scope.IsQuarter = false;
                $scope.IsMonth = false;
                $scope.IsWeek = false;
                $scope.IsYear = false;
                var reportType = $scope.Sreport.name;
                var periodType = $scope.Speriod.name;
              

                $scope.GetCelebritySubscriberActivity($scope.channelId, reportType, periodType, 0);
            }
        }

        ///////////////////////////////Activity ///////////////////////////////////////////////////


        $scope.GetCelebritySubscriberActivity = function (channelID, periodType, period, periodValue) {
            $scope.show = false;
            ngProgress.start();
           // debugger
            var dataToPost = { channelID: channelID, periodType: periodType, periods: period, periodValue: periodValue };
            $http.post($rootScope.SERVICE_URL + "ChannelSubscribers/GetCelebritySubscriberActivity", dataToPost)
                        .success(function (serverResponse, status, headers, config) {
                            $scope.CelebritySubscriberActivity = serverResponse;
                           // alert($scope.CelebritySubscriberActivity);
                        }).error(function (serverResponse, status, headers, config) {
                            alert(status + " - Error Occured!");
                        }
                    );
            ngProgress.complete();
            $scope.show = true;
        }


        ///////////////////////////////////////////////////////////

        $scope.changeWeek = function () {
            var reportType = $scope.Sreport.name;
            var periodType = $scope.Speriod.name;
            var week = $scope.sweek.Id;

            $scope.GetCelebritySubscriberActivity($scope.channelId, reportType, periodType, week);
        }


        $scope.changeMonth = function () {
          
            var reportType=$scope.Sreport.name;
            var periodType = $scope.Speriod.name;
            var Month = $scope.smonth.Id;

            $scope.GetCelebritySubscriberActivity($scope.channelId, reportType, periodType, Month);
       
        }
        $scope.changeQuarter = function () {
            var reportType = $scope.Sreport.name;
            var periodType = $scope.Speriod.name;
            var quarter = $scope.squarter.Id;

            $scope.GetCelebritySubscriberActivity($scope.channelId, reportType, periodType, quarter);
        }

        $scope.changeYear = function () {
            var reportType = $scope.Sreport.name;
            var periodType = $scope.Speriod.name;
            var year = $scope.syear.name;

            $scope.GetCelebritySubscriberActivity($scope.channelId, reportType, periodType, year);
        }


        Date.prototype.formatMMDDYYYY = function () {
            return (this.getMonth() +1) +
            "/" +this.getDate() +
            "/" +this.getFullYear();
            }

       
        //////////////////////Start  Lost Subscribers Report ////////////////////////////////////
        //$scope.changewhojoin = function () {
        //    $scope.show = false;
        //    ngProgress.start();
        //    var periodType = $scope.SpecificPeriod.name;
        //    var dataToPost = { channelID: channelId, periodType: periodType, FromDate: '', ToDate: '' };
        //    $http.post($rootScope.SERVICE_URL + "ChannelSubscribers/GetSubscriberWhoJoinInPeriod", dataToPost)
        //                .success(function (serverResponse, status, headers, config) {
        //                    $scope.JoinSubscribers = serverResponse;
        //                }).error(function (serverResponse, status, headers, config) {
        //                    alert(status + " - Error Occured!");
        //                }
        //            );
        //    ngProgress.complete();
        //    $scope.show = true;
        //}
        
        //$scope.SubscriberwhojoinFromTo = function () {
        //    $scope.show = false;
        //    //debugger
        //    ngProgress.start();         
        //    //var fromDate = $scope.FromDate.getFullYear() + '-' + ($scope.FromDate.getMonth() + 1) + '-' + $scope.FromDate.getDate();
        //    //var toDate = $scope.ToDate.getFullYear() + '-' + ($scope.ToDate.getMonth() + 1) + '-' + $scope.ToDate.getDate();
        //    var fromDate = $scope.FromDate
        //    var toDate = $scope.ToDate
        //    var dataToPost = { channelID: channelId, periodType: 'FromToDate', FromDate: fromDate, ToDate: toDate };
        //    $http.post($rootScope.SERVICE_URL + "ChannelSubscribers/GetSubscriberWhoJoinInPeriod", dataToPost)
        //                .success(function (serverResponse, status, headers, config) {
        //                    $scope.JoinFromToSubscribers = serverResponse;
        //                    ngProgress.complete();
        //                    $scope.show = true;
        //                }).error(function (serverResponse, status, headers, config) {
        //                    alert(status + " - Error Occured!");
        //                    ngProgress.complete();
        //                    $scope.show = true;
        //                }
        //            );
           
        //}

        //$scope.changewhoLost = function () {
        //    $scope.show = false;
        //    ngProgress.start();
        //    var periodType = $scope.LSpecificPeriod.name;
        //    var dataToPost = { channelID: channelId, periodType: periodType, FromDate: '', ToDate: '' };
        //    $http.post($rootScope.SERVICE_URL + "ChannelSubscribers/GetSubscriberWhoLostInPeriod", dataToPost)
        //                .success(function (serverResponse, status, headers, config) {
        //                    $scope.lostSubscribers = serverResponse;
        //                }).error(function (serverResponse, status, headers, config) {
        //                    alert(status + " - Error Occured!");
        //                }
        //            );
        //    ngProgress.complete();
        //    $scope.show = true;
        //}

        ////////////////////End  Lost Subscribers Report ////////////////////////////////////

    }]
    )
})();