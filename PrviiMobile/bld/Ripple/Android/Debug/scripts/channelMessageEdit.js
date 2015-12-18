(function () {
    'use strict';
    var prviiAppchannelMessageEdit = angular.module('prviiApp.channelMessageEdit', []);

    prviiAppchannelMessageEdit.controller('channelMessageEditController', ['$rootScope', '$scope', '$http', '$location', '$routeParams', 'ngProgress',
    function ($rootScope, $scope, $http, $location, $routeParams, ngProgress) {
         $scope.show = false;
        ngProgress.start();
        $scope.FileList1 = [];
        var messageId = $routeParams.messageId;
       // alert('Edit...' + messageId);
        var messageData = { ID: messageId };
     //  debugger
       // var d = new Date();
      //  $scope.MinDate = new Date(d.getFullYear(), d.getMonth(), d.getDate());
        $http.post($rootScope.SERVICE_URL + "ChannelMessage/GetChannelMessageById", messageData)
                .success(function (serverResponse, status, headers, config) {
                    //debugger
                    // alert('d');
                    $scope.Status = serverResponse.Status;
                    $scope.ID = serverResponse.ID;
                    $scope.Subject = serverResponse.Subject;
                    $scope.Message = serverResponse.Message;
                    $scope.IsScheduled = serverResponse.IsScheduled;
                    $scope.MessageTypeID = serverResponse.MessageTypeID;
                    $scope.MessageView = serverResponse;
                    $scope.channelId = serverResponse.ChannelID;
                    $scope.Attachments = serverResponse.Attachments;
                    var d = new Date(serverResponse.ScheduledOn);                 
                    var DD = d.getDate();
                    if ((1 <= DD) && (DD <= 9))
                       DD = '0' + DD;

                    var MM = d.getMonth();
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
                    debugger
                    $scope.ScheduledOn = YYYY + '-' + MM + '-' + DD + 'T' + HH + ':' + MN + ':' + SS;
                    $scope.IsSubcriberMessage = true;
                    for (var i = 1; $scope.Attachments.length >= i; i++) {
                        $scope.FileList1[i] = { ID: 0, ChannelMessageID: 0, Name: Attachments.Name, MimeType: Attachments.MimeType, Content: Attachments.Content };
                    }

                }).error(function (serverResponse, status, headers, config) {
                    alert(status + " - Error Occured!");
                }

            );
        ngProgress.complete();
        $scope.show = true;
             
        // debugger
       

        $scope.FileList = [];

        $scope.setFile = function (element) {
            $scope.$apply(function ($scope) {

                $scope.files = [];

                if (element.files.length > 1)
                    $scope.files = element.files;

                var reader = new FileReader();

                function readFile(index) {
                    if (index >= element.files.length) return;

                    var file = element.files[index];
                    reader.onload = function (e) {
                        // get file content
                        var bin = e.target.result;
                        $scope.FileList[index] = { ID: 0, ChannelMessageID: 0, Name: file.name, MimeType: file.type, Content: bin };

                        readFile(index + 1)
                    }
                    reader.readAsDataURL(file);
                }

                readFile(0);

            });
        };
      
        
        $scope.DeleteFile = function (AttechmentID) {
            alert('DeleteFile..');
            $scope.show = false;
            ngProgress.start();
            var messageData = { ID: AttechmentID };
            $http.post($rootScope.SERVICE_URL + "ChannelMessage/DeleteMessageAttachmentById", messageData)
                    .success(function (serverResponse, status, headers, config) {
                        alert(serverResponse);
                    }).error(function (serverResponse, status, headers, config) {
                        alert(status + " - Error Occured!");
                    }
                );
            ngProgress.complete();
            $scope.show = true;
        }
        $scope.downloadFile = function () {
            alert('download..');
            $scope.show = false;
            ngProgress.start();
            var filePath = $rootScope.SERVICE_URL + "ChannelMessage/GetAttachment";
            window.open(filePath, '_blank', '');
            ngProgress.complete();
            $scope.show = true;
        }

        $scope.UpdateMessage = function () {
            // alert('called...');
           // debugger
            $scope.show = false;
            ngProgress.start();
             var messageToCreate = { ID: messageId, ChannelID: $scope.channelId, Subject: $scope.Subject, Message: $scope.Message, ScheduledOn: $scope.ScheduledOn, IsScheduled: $scope.IsScheduled, MessageTypeID: $scope.MessageTypeID, Attachments: $scope.FileList };
            //var loggedUser = $rootScope.GetLoggedUser();

            $http.post($rootScope.SERVICE_URL + "ChannelMessage/UpdateChannelMessage", messageToCreate)
                    .success(function (serverResponse, status, headers, config) {
                        $location.path('/channelMessageList/' + $scope.channelId);
                    }).error(function (serverResponse, status, headers, config) {
                        alert(status + " - Error Occured!");
                    }
                );
            ngProgress.complete();
            $scope.show = true;
        }


        $scope.createMessage = function () {
            //  alert('called...');
            //debugger
            $scope.show = false;
            ngProgress.start();
            //$scope.FileListTotal = [];
            //var FileListTotal[] = $scope.FileList.concat($scope.FileList1);
            var messageToCreate = { ID: 0, ChannelID: $scope.channelId, Subject: $scope.Subject, Message: $scope.Message, ScheduledOn: $scope.ScheduledOn, IsScheduled: $scope.IsScheduled, MessageTypeID: $scope.MessageTypeID, Attachments: $scope.FileList };
            //var loggedUser = $rootScope.GetLoggedUser();

            $http.post($rootScope.SERVICE_URL + "ChannelMessage/CreateChannelMessage", messageToCreate)
                    .success(function (serverResponse, status, headers, config) {
                        //alert('success');
                        $location.path('/channelMessageList/' + $scope.channelId);
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