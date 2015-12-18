(function () {
    'use strict';
    var prviiAppmessageEditReview = angular.module('prviiApp.messageEditReview', []);

    prviiAppmessageEditReview.controller('messageEditReviewController', ['$rootScope', '$scope', '$http', '$location', '$routeParams', 'ngProgress',
    function ($rootScope, $scope, $http, $location, $routeParams, ngProgress) {

        var channelID = $rootScope.USER_ChannelID;      
        $scope.selectedUsers = [];
        $scope.selectedmedia = [];
        $scope.IsScheduled = false;
        $scope.ButtonText = "Save";
        $scope.AllSubscribers = true;       
        
        $scope.channelId = channelID;
        $scope.ErrorShow = false;
        $scope.DeliveryMechanisms = false;
        $scope.ButtonText = "Save";
      


        $scope.show = false;
        ngProgress.start();
        

        var dataToPost = { ID:  $rootScope.USER_ChannelID, UserID: $rootScope.USER_ID };
        
        $http.post($rootScope.SERVICE_URL + "channel/GetSubscribers", dataToPost)
                    .success(function (serverResponse, status, headers, config) {
                        $scope.SubscriberList = serverResponse;

                    }).error(function (serverResponse, status, headers, config) {
                        alert(status + " - Error Occured!");
                    }
                );


        $http.post($rootScope.SERVICE_URL + "channel/GetMediaURLs", dataToPost)
                   .success(function (serverResponse, status, headers, config) {                      
                       $scope.MediaList = serverResponse;
                   }).error(function (serverResponse, status, headers, config) {
                       alert(status + " - Error Occured!");
                   }
               );



        //var messageId = $routeParams.messageId;
        var messageId = $rootScope.viewMessageID;
       
        var messageData = { ID: messageId };
      

        $http.post($rootScope.SERVICE_URL + "ChannelMessage/GetChannelMessageById", messageData)
                .success(function (serverResponse, status, headers, config) {                
                    $scope.Status = serverResponse.Status;
                    $scope.ID = serverResponse.ID;
                    $scope.Subject = serverResponse.Subject;
                    $scope.Message = serverResponse.Message;
                    $scope.EmailMessage  = serverResponse.EmailMessage;
                    $scope.IsScheduled = serverResponse.IsScheduled ? false : true;
                   // $scope.MessageTypeID = serverResponse.MessageTypeID;
                    $scope.MessageView = serverResponse;
                    $scope.channelId = serverResponse.ChannelID;
                  //  $scope.Attachments = serverResponse.Attachments;
                    $scope.IsSMS = serverResponse.IsSMS;
                    $scope.IsEmail = serverResponse.IsEmail;
                    $scope.selectedUsers = serverResponse.SubscriberIDs;
                    $scope.AllSubscribers = serverResponse.SendToAll;
                    var d = new Date(serverResponse.ScheduledOn);
                   
                    $scope.ScheduledOnDate = new Date(d.getFullYear(), d.getMonth(), d.getDate());
                    $scope.ScheduledOnTime = new Date(d.getFullYear(), d.getMonth(), d.getDate(), d.getHours(), d.getMinutes(), 0);
                    $scope.selectedmedia = serverResponse.MediaURLIds;
                    if ($scope.IsScheduled)
                        $scope.ButtonText = "Save/Send";
                    else
                        $scope.ButtonText = "Save";

                }).error(function (serverResponse, status, headers, config) {
                    alert(status + " - Error Occured!");
                }

            );
        ngProgress.complete();
        $scope.show = true;

        // debugger

        $rootScope.includeModifyApproveDone = function (templateURI) {
            $rootScope.srcModify = 'partials/' + templateURI;
            $rootScope.modify = true;
        }

        $rootScope.includeMessageDelete = function (templateURI) {
            $rootScope.srcDelete = 'partials/' + templateURI;
            $rootScope.delete = true;
        }
       
        $rootScope.includeMessagePending = function (templateURI) {
            $rootScope.srcViewPending = 'partials/' + templateURI;
            $rootScope.viewPendingMessage = true;
        }

        $rootScope.includeMessageSent = function (templateURI) {
            $rootScope.srcViewSent = 'partials/' + templateURI;
            $rootScope.viewSentMessage = true;
        }

        $rootScope.includeMessageAll = function (templateURI) {
            $rootScope.srcViewAll = 'partials/' + templateURI;
            $rootScope.viewAllMessage = true;
        }


        $scope.someSelected = function () {
           
             var result = false;
            if ($scope.IsEmail == true) {
               
                result = true;
                }
                else if  ($scope.IsSMS == true)
                    {
                result = true;
                }
            else
            {
                result = false;
                }
            return result;
        }

        $scope.IsScheduledMessage = function () {
            // alert($scope.IsScheduled);
            if ($scope.IsScheduled)
                $scope.ButtonText = "Save/Send";
            else
                $scope.ButtonText = "Save";
        }

        $scope.toggleSelection = function toggleSelection(fruitName) {
            debugger
            var idx = $scope.selectedUsers.indexOf(fruitName);
            // is currently selected
            if (idx > -1) {
                $scope.selectedUsers.splice(idx, 1);
            }
                // is newly selected
            else {
                $scope.selectedUsers.push(fruitName);
            }
        };

        $scope.toggleMedia = function toggleMedia(fruitName) {
            debugger
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


        $scope.UpdateMessageApprove = function () {
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

            UpdateMessageData();
                      

        }

        $scope.UpdateMessage = function () {

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

            UpdateMessageData();


        }




        function UpdateMessageData() {
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
            if ($scope.ApproveMessage) {
                UpdateApproveMessage(messageToCreate);
            }
            else {
                SaveUpdateMessage(messageToCreate);
            }


        }

        function UpdateApproveMessage(messageToCreate) {
            $scope.show = false;
            ngProgress.start();
            // debugger
            $http.post($rootScope.SERVICE_URL + "ChannelMessage/UpdateChannelMessageApprove", messageToCreate)
                     .success(function (serverResponse, status, headers, config) {
                         //alert('success');
                         if (serverResponse == 'success') {
                             $rootScope.srcModify = 'partials/channelmessageList.html';
                             $rootScope.modify = true;
                         }
                         else {
                             alert(serverResponse);
                         }

                         //$location.path('/channelMessageList/' + channelID);
                     }).error(function (serverResponse, status, headers, config) {
                         alert(status + " - Error Occured!");
                     }
                 );

            ngProgress.complete();
            $scope.show = true;

        }

        function SaveUpdateMessage(messageToCreate) {
            $scope.show = false;
            ngProgress.start();
           
            $http.post($rootScope.SERVICE_URL + "ChannelMessage/UpdateChannelMessage", messageToCreate)
                    .success(function (serverResponse, status, headers, config) {                       
                        if (serverResponse == 'success') {
                            $rootScope.srcModify = 'partials/channelmessageList.html';
                            $rootScope.modify = true;
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



        $scope.DeleteMessage = function (ID)
        {
            
            var result=  confirm("Are you sure ?");
            if(result)
            {
                //alert(ID);
                $scope.show = false;
                ngProgress.start();
                var messageData = { ID: ID };
                $http.post($rootScope.SERVICE_URL + "ChannelMessage/ChannelMessageDelete", messageData)
                        .success(function (serverResponse, status, headers, config) {
                            //$location.path('/channelMessageList/' + channelId);
                            $rootScope.srcDelete = 'partials/channelMessageListDelete.html';
                            $rootScope.delete = true;
                        }).error(function (serverResponse, status, headers, config) {
                            alert(status + " - Error Occured!");
                        }
                    );
                ngProgress.complete();
                $scope.show = true;
            }
            
        }

        $scope.DeliveryYes = function () {
            // debugger
            if ($scope.SentOnly == "SMS") {
                var sms = $scope.EmailMessage;
                if (sms.length >= 156)
                    $scope.Message = sms.substring(0, 155);
                else
                    $scope.Message = sms.substring(0, sms.length);

                $scope.IsSMS = true;

                $scope.SIDs = $scope.selectedUsersSMS;
            }


            if ($scope.SentOnly == "Email") {
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
       
    }])    
})();