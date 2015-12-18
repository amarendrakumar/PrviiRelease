(function () {
    'use strict';
    var prviiAppChannelList = angular.module('prviiApp.channelMessageCreate', []);

    prviiAppChannelList.controller('channelMessageCreateController', ['$rootScope', '$scope', '$http', '$location', '$routeParams', 'ngProgress',
    function ($rootScope, $scope, $http, $location, $routeParams, ngProgress) {
        // var channelID = $routeParams.ChannelID;
        var channelID = $rootScope.USER_ChannelID;
       
        $scope.channelId = channelID;
        $scope.ErrorShow = false;
        $scope.DeliveryMechanisms = false;
        $scope.ButtonText = "Save";
        $scope.AllSubscribers = true;
        $scope.show = false;
      
        ngProgress.start();


        var dataToPost = { ID: channelID, UserID: $rootScope.USER_ID };
        $http.post($rootScope.SERVICE_URL + "channel/GetSubscribers", dataToPost)
                    .success(function (serverResponse, status, headers, config) {
                        //  debugger
                        $scope.SubscriberList = serverResponse;
                        angular.forEach($scope.SubscriberList, function (item) {
                            $scope.selectedUsers.push(item.ID);
                        });

                    }).error(function (serverResponse, status, headers, config) {
                        alert(status + " - Error Occured!");
                    }
                );


        $http.post($rootScope.SERVICE_URL + "channel/GetMediaURLs", dataToPost)
                   .success(function (serverResponse, status, headers, config) {
                       // debugger
                       $scope.MediaList = serverResponse;
                   }).error(function (serverResponse, status, headers, config) {
                       // debugger
                       alert(status + " - Error Occured!");
                   }
               );




        ngProgress.complete();
        $scope.show = true;
        // End Bind Subscribers related to celebrity

        $scope.IsScheduled = false;

        
        var d = new Date();
        var DD = d.getDate();
        if ((1 <= DD) && (DD <= 9))
            DD = '0' + DD;

        var MM = d.getMonth() + 1;
        if ((0 <= MM) && (MM <= 9))
            MM = '0' + MM;

        var YYYY = d.getFullYear();

        var HH = d.getHours();
        if ((0 <= HH) && (HH <= 9))
            HH = '0' + HH;

        var MN = d.getMinutes();
        if ((0 <= MN) && (MN <= 9))
            MN = '0' + MN;
        var SS = d.getSeconds();
        if ((0 <= SS) && (SS <= 9))
            SS = '0' + SS;
        // debugger
        $scope.ScheduledOnDate = new Date(YYYY, MM - 1, DD);
        $scope.ScheduledOnTime = new Date(YYYY, MM - 1, DD, HH, MN, 0);

        //---------------------------------------------------------------------------------
        $scope.FileList = [];



        $scope.someSelected = function () {
            var result = false;
            if ($scope.IsEmail == true) {
                result = true;
            }
            else if ($scope.IsSMS == true) {
                result = true;
            }
            else {
                result = false;
            }
            return result;
        }

        $scope.IsScheduledMessage = function () {
            if ($scope.IsScheduled)
                $scope.ButtonText = "Save/Send";
            else
                $scope.ButtonText = "Save";
        }



        $scope.selectedUsers = [];
        $scope.selectedmedia = [];

        $scope.UerSelect = function () {
            if ($scope.AllSubscribers) {
                angular.forEach($scope.SubscriberList, function (item) {
                    $scope.selectedUsers.push(item.ID);
                });
            }
            else {
                $scope.selectedUsers = [];
            }
        }


        $scope.selectedUsersEmail = [];
        $scope.toggleSelectionEmail = function toggleSelectionEmail(fruitName) {
            var idx = $scope.selectedUsersEmail.indexOf(fruitName);
            // is currently selected
            if (idx > -1) {
                $scope.selectedUsersEmail.splice(idx, 1);
            }
                // is newly selected
            else {
                $scope.selectedUsersEmail.push(fruitName);

            }
            
        };

        $scope.selectedUsersSMS = [];
        $scope.toggleSelectionSMS = function toggleSelectionSMS(fruitName) {
            var idx = $scope.selectedUsersSMS.indexOf(fruitName);
            // is currently selected
            if (idx > -1) {
                $scope.selectedUsersSMS.splice(idx, 1);
            }
                // is newly selected
            else {
                $scope.selectedUsersSMS.push(fruitName);

            }
           
        };


        $scope.toggleSelection = function toggleSelection(fruitName) {
            var idx = $scope.selectedUsers.indexOf(fruitName);
            // is currently selected
            if (idx > -1) {
                $scope.selectedUsers.splice(idx, 1);
            }
                // is newly selected
            else {
                $scope.selectedUsers.push(fruitName);

            }
            if (!$scope.selectedUsers.length > 0) {
                $scope.ErrorShow = true;
            }
            else {
                $scope.ErrorShow = false;
            }
        };

        $scope.toggleMedia = function toggleSelection(fruitName) {
            var idx = $scope.selectedmedia.indexOf(fruitName);
            // is currently selected
            if (idx > -1) {
                $scope.selectedmedia.splice(idx, 1);
            }
                // is newly selected
            else {
                $scope.selectedmedia.push(fruitName);
            }
        };



        $scope.Cancel = function () {
            $rootScope.create = false;

        }



        $scope.createMessageApprove = function () {
            $scope.ApproveMessage = true;
            if (!$scope.selectedUsers.length > 0) {
                // alert("Please checked at least one subscribers.")
                $scope.ErrorShow = true;
                return false;
            }


            if ($scope.IsSMS) {
                if ($scope.Message.length >= 156) {
                    if (!confirm('The Message length for SMS is exceeding 155 characters. Click, Ok, to send multiple messages else click Cancel to modify the message length.')) {
                        return false;
                    }
                }
            }
            //debugger
            $scope.SentOnly = "";
            $scope.SIDs = [];
            $scope.itemsSMS = [];
            $scope.itemsEmail = [];
            $scope.selectedUsersEmail = [];
            $scope.selectedUsersSMS = [];
            angular.forEach($scope.SubscriberList, function (item) {
                // debugger
                for (var i = 0; $scope.selectedUsers.length >= i; i++) {
                    if ($scope.selectedUsers[i] == item.ID) {
                        // debugger
                        var DType = item.DeliverType.split(",");
                        $scope.stext = "";
                        $scope.semail = "";

                        if (DType.length == 1) {
                            if (DType[0] == "Text")
                                $scope.stext = DType[0];
                            else
                                $scope.semail = DType[0];

                            if (DType[0] == "Email")
                                $scope.semail = DType[0];
                            else
                                $scope.stext = DType[0];

                        }
                        else if (DType.length == 2) {
                            if (DType[0] == "Text")
                                $scope.stext = DType[0];
                            else
                                $scope.semail = DType[0];

                            if (DType[0] == "Email")
                                $scope.semail = DType[0];
                            else
                                $scope.stext = DType[0];
                            if (DType[1] == "Text")
                                $scope.stext = DType[1];
                            else
                                $scope.semail = DType[1];

                            if (DType[1] == "Email")
                                $scope.semail = DType[1];
                            else
                                $scope.stext = DType[1];
                        }

                       
                        if ($scope.IsSMS) {
                            if ($scope.stext == "")
                            {
                                $scope.itemsEmail.push({
                                    ID: item.ID,
                                    Name: item.Name
                                });
                                $scope.selectedUsersEmail.push(item.ID);
                            }
                               
                          //  debugger
                        }

                        if ($scope.IsEmail) {
                            if ($scope.semail == "")
                            {
                                $scope.itemsSMS.push({
                                    ID: item.ID,
                                    Name: item.Name
                                });
                                $scope.selectedUsersSMS.push(item.ID);
                              //  debugger
                            }
                                
                        }
                      
                    }

                }

            });

            if (($scope.IsSMS) && (!$scope.IsEmail)) {

                if ($scope.itemsEmail.length > 0) {
                    $scope.SentOnly = "Email";
                    $scope.DeliveryMechanisms = true;
                    return false;
                }
            }
            else if ((!$scope.IsSMS) && ($scope.IsEmail)) {
                if ($scope.itemsSMS.length > 0) {
                    $scope.SentOnly = "SMS";
                    $scope.DeliveryMechanisms = true;
                    return false;
                }
            }
         
            
            SaveMessageData();
         

           
        }

        function SaveMessageData()
        {
          //  debugger
           
            if ($scope.ScheduledOnDate != null) {
                var month = $scope.ScheduledOnDate.getUTCMonth() + 1;
                var month1 = $scope.ScheduledOnDate.getMonth() + 1;
                if ((0 <= month1) && (month1 <= 9))
                    month1 = '0' + month1;

                var dd = $scope.ScheduledOnDate.getDate();
                if ((0 <= dd) && (dd <= 9))
                    dd = '0' + dd;

                var HH = $scope.ScheduledOnTime.getHours();
                if ((0 <= HH) && (HH <= 9))
                    HH = '0' + HH;

                var MN = $scope.ScheduledOnTime.getMinutes();
                if ((0 <= MN) && (MN <= 9))
                    MN = '0' + MN;

                $scope.scheduledOnDateTime = $scope.ScheduledOnDate.getFullYear() + '-' + month1 + '-' + dd + 'T' + HH + ':' + MN;
            }
            else {
                $scope.scheduledOnDateTime = "";
            }
            var messageToCreate = {
                ID: 0, ChannelID: channelID, Subject: $scope.Subject, Message: $scope.Message, EmailMessage: $scope.EmailMessage, IsScheduled: $scope.IsScheduled,
                ScheduledOn: $scope.scheduledOnDateTime, IsSMS: $scope.IsSMS, IsEmail: $scope.IsEmail, SubscriberIDs: $scope.selectedUsers,
                SendToAll: $scope.AllSubscribers, MediaURLIDs: $scope.selectedmedia, SentOnly: $scope.SentOnly, SIDs: $scope.SIDs
            };
            if ($scope.ApproveMessage)
            {
                SaveApproveMessage(messageToCreate);
            }
            else
            {
                SaveCreateMessage(messageToCreate);
            }
           

        }

        function SaveApproveMessage(messageToCreate)
        {
            $scope.show = false;
            ngProgress.start();
           // debugger
            $http.post($rootScope.SERVICE_URL + "ChannelMessage/CreateChannelMessageApprove", messageToCreate)
                   .success(function (serverResponse, status, headers, config) {
                        //debugger
                       if (serverResponse == 'success') {
                           $rootScope.srcViewAll = 'partials/channelAllMessageList.html'
                           $rootScope.viewAll = true;
                           $rootScope.create = false;
                           $rootScope.review = false;
                           $rootScope.modify = false;
                           $rootScope.delete = false;
                       }
                       else {
                           alert(serverResponse);
                       }

                   }).error(function (serverResponse, status, headers, config) {
                       alert(status + " - Error Occured!");
                   }
               );

            ngProgress.complete();
            $scope.show = true;

        }

        function SaveCreateMessage(messageToCreate)
        {
            $scope.show = false;
            ngProgress.start();
           // debugger
            $http.post($rootScope.SERVICE_URL + "ChannelMessage/CreateChannelMessage", messageToCreate)
                 .success(function (serverResponse, status, headers, config) {
                    // debugger
                     if (serverResponse == 'success') {
                         $rootScope.srcViewAll = 'partials/channelAllMessageList.html'
                         $rootScope.viewAll = true;
                         $rootScope.create = false;
                         $rootScope.review = false;
                         $rootScope.modify = false;
                         $rootScope.delete = false;
                     }
                     else {
                         alert(serverResponse);
                     }
                 }).error(function (serverResponse, status, headers, config) {
                     alert(status + " - Error Occured!");
                 }
             );

            ngProgress.complete();
            $scope.show = true;

        }


        $scope.createMessageNew = function () {
            $scope.ApproveMessage = false;
            if (!$scope.selectedUsers.length > 0) {
                // alert("Please checked at least one subscribers.")
                $scope.ErrorShow = true;
                return false;
            }


            if ($scope.IsSMS) {
                if ($scope.Message.length >= 156) {
                    if (!confirm('The Message length for SMS is exceeding 155 characters. Click, Ok, to send multiple messages else click Cancel to modify the message length.')) {
                        return false;
                    }
                }
            }
            //debugger
            $scope.SentOnly = "";
            $scope.SIDs = [];
            $scope.itemsSMS = [];
            $scope.itemsEmail = [];
            $scope.selectedUsersEmail = [];
            $scope.selectedUsersSMS = [];
            angular.forEach($scope.SubscriberList, function (item) {
                // debugger
                for (var i = 0; $scope.selectedUsers.length >= i; i++) {
                    if ($scope.selectedUsers[i] == item.ID) {
                        // debugger
                        var DType = item.DeliverType.split(",");
                        $scope.stext = "";
                        $scope.semail = "";

                        if (DType.length == 1) {
                            if (DType[0] == "Text")
                                $scope.stext = DType[0];
                            else
                                $scope.semail = DType[0];

                            if (DType[0] == "Email")
                                $scope.semail = DType[0];
                            else
                                $scope.stext = DType[0];

                        }
                        else if (DType.length == 2) {
                            if (DType[0] == "Text")
                                $scope.stext = DType[0];
                            else
                                $scope.semail = DType[0];

                            if (DType[0] == "Email")
                                $scope.semail = DType[0];
                            else
                                $scope.stext = DType[0];
                            if (DType[1] == "Text")
                                $scope.stext = DType[1];
                            else
                                $scope.semail = DType[1];

                            if (DType[1] == "Email")
                                $scope.semail = DType[1];
                            else
                                $scope.stext = DType[1];
                        }


                        if ($scope.IsSMS) {
                            if ($scope.stext == "") {
                                $scope.itemsEmail.push({
                                    ID: item.ID,
                                    Name: item.Name
                                });
                                $scope.selectedUsersEmail.push(item.ID);
                            }

                            //  debugger
                        }

                        if ($scope.IsEmail) {
                            if ($scope.semail == "") {
                                $scope.itemsSMS.push({
                                    ID: item.ID,
                                    Name: item.Name
                                });
                                $scope.selectedUsersSMS.push(item.ID);
                                //  debugger
                            }

                        }

                    }

                }

            });
            if (($scope.IsSMS) && (!$scope.IsEmail)) {

                if ($scope.itemsEmail.length > 0) {
                    $scope.SentOnly = "Email";
                    $scope.DeliveryMechanisms = true;
                    return false;
                }
            }
            else if ((!$scope.IsSMS) && ($scope.IsEmail)) {
                if ($scope.itemsSMS.length > 0) {
                    $scope.SentOnly = "SMS";
                    $scope.DeliveryMechanisms = true;
                    return false;
                }
            }

           
            SaveMessageData();

        }


       

        $scope.DeliveryYes=function()
        {
           // debugger
            if ($scope.SentOnly == "SMS")
            {
                var sms = $scope.EmailMessage;
                if (sms.length >= 156)
                    $scope.Message = sms.substring(0, 155);
                else
                    $scope.Message = sms.substring(0, sms.length);

                $scope.IsSMS = true;

                $scope.SIDs = $scope.selectedUsersSMS;
            }
               

            if ($scope.SentOnly == "Email")
            {
                $scope.EmailMessage = $scope.Message;
                $scope.Subject = " Subject : - " + $scope.Message;
                $scope.IsEmail = true;
                $scope.SIDs = $scope.selectedUsersEmail;
            }
               

            SaveMessageData();
            $scope.DeliveryMechanisms = false;
        }

        $scope.DeliveryNo = function () {
           // debugger
            $scope.SentOnly = "";
            $scope.SIDs = [];
            SaveMessageData();
            $scope.DeliveryMechanisms = false;
        }

        $scope.CancelDelivery = function () {
            $scope.DeliveryMechanisms = false;
        }

    }]
    )

    prviiAppChannelList.directive('ngConfirmClick', [
        function () {
            return {
                link: function (scope, element, attr) {
                    var msg = attr.ngConfirmClick || "Are you sure?";
                    var clickAction = attr.confirmedClick;
                    element.bind('click', function (event) {
                        if (scope.IsScheduled) {
                            if (window.confirm(msg)) {
                                scope.$eval(clickAction)
                            }
                            else {
                                scope.IsScheduled = false;
                                scope.ButtonText = "Save";
                            }
                        }
                        else {
                            scope.$eval(clickAction)
                        }

                    });
                }
            };
        }])


})();