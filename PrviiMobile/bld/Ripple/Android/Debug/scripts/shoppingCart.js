(function () {
    'use strict';
    var prviiAppshoppingCart = angular.module('prviiApp.shoppingCart', []);

    prviiAppshoppingCart.controller('shoppingCartController', ['$rootScope', '$scope', '$http', '$routeParams', '$location', 'ngProgress',
    function ($rootScope, $scope, $http, $routeParams, $location, ngProgress) {
        $scope.show = false;
        ngProgress.start();
        var subscriberId = $rootScope.USER_ID;
        var dataToPost = { UserID: subscriberId };
       
        $http.post($rootScope.SERVICE_URL + "ShoppingCart/BindCart", dataToPost)
                    .success(function (serverResponse, status, headers, config) {
                        debugger
                        $scope.TotalPrice = 0.00;
                        $scope.shoppingCartList = serverResponse;
                        $scope.ShoppingCartID = serverResponse[0].ShoppingCartID;

                        for (var i = 0; i < $scope.shoppingCartList.length; i++) {
                            $scope.TotalPrice = $scope.TotalPrice + $scope.shoppingCartList[i].Price;
                        }
                        $scope.TotalPrice = $scope.TotalPrice.toFixed(2);
                    }).error(function (serverResponse, status, headers, config) {
                        alert(status + " - Error Occured!");
                    }
                );
        ngProgress.complete();
        $scope.show = true;


        $scope.selectedUsers = [];

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
        };

        // channelId, channelName, channelPrice
        $scope.channelSubscribe = function channelSubscribe() {

            alert(channelPrice);

            ////create paypal payment details object
            //var paymentDetails = new PayPalPaymentDetails(
            //  channelPrice, // subtotal (amount ex shipping and tax)
            //   "00", // shipping
            //   "0"  // tax
            //);

            var paymentDetails = new PayPalPaymentDetails(
             "5.00", // subtotal (amount ex shipping and tax)
              "3.00", // shipping
              "2.00"  // tax
           );

            debugger
            var payment = new PayPalPayment(
             "20.00", // amount (the sum of the fields above)
             "USD",   // currency (in ISO 4217 format)
             "Telerik T-Shirt", // description of the payment
             "Sale",  // Sale (immediate payment) or Auth (authorization only)
             paymentDetails // the object prepared above, optional
           );


            ////create paypal payment object
            //var payment = new PayPalPayment(
            //  channelPrice, // amount (the sum of the fields above)
            //  "USD",   // currency (in ISO 4217 format)
            //  channelName, // description of the payment
            //  "Sale",  // Sale (immediate payment) or Auth (authorization only)
            //  paymentDetails // the object prepared above, optional
            //);

            debugger
            PayPalMobile.renderSinglePaymentUI(
            payment,
            function (payment) { alert("payment success1: " + payment.response.id + "---" + JSON.stringify(payment)); },
            function (errorresult) { alert(errorresult) }
          );

            ////initiate paypal payment
            //PayPalMobile.renderSinglePaymentUI(
            //  payment,
            //  function (payment) {
            //      //alert("payment success: " + payment.response.id + "---" + JSON.stringify(payment));
            //      //  debugger
            //      var dataToPost = { UserID: subscriberId, ChannelID: channelId, Price: channelPrice, PaymentTransactionID : payment.response.id };
            //      $http.post($rootScope.SERVICE_URL + "channel/Subscribe", dataToPost)
            //             .success(function (serverResponse, status, headers, config) {
            //                 //  debugger
            //                 alert('subscription successful');
            //                 $location.path('/subscriberChannelList/' + subscriberId);
            //             }).error(function (serverResponse, status, headers, config) {
            //                 //     debugger
            //                 alert(status + " - Error Occured!");
            //             }
            //         );
            //  },
            //  function (errorresult) {
            //      alert(errorresult)
            //  }
            //);

        }

        $scope.Go = function Go(channelId, channelName, channelPrice) {
            alert(channelId + "," + channelName + "," + channelPrice);
            // alert("amar");
            // alert('buy now!');

            ////create paypal payment details object
            var paymentDetails = new PayPalPaymentDetails(
              channelPrice, // subtotal (amount ex shipping and tax)
               "0.00", // shipping
               "0.00"  // tax
            );

            var payment = new PayPalPayment(
              channelPrice, // amount (the sum of the fields above)
              "USD",   // currency (in ISO 4217 format)
             channelName, // description of the payment
              "Sale",  // Sale (immediate payment) or Auth (authorization only)
              paymentDetails // the object prepared above, optional
            );

            PayPalMobile.renderSinglePaymentUI(
              payment,
              function (payment) {
                  //alert("payment success go: " + payment.response.id + "---" + JSON.stringify(payment));
                  var dataToPost = { UserID: subscriberId, ChannelID: channelId, Price: channelPrice, PaymentTransactionID: payment.response.id };
                  $http.post($rootScope.SERVICE_URL + "channel/Subscribe", dataToPost)
                         .success(function (serverResponse, status, headers, config) {
                             //  debugger
                             alert('subscription successful');
                             $rootScope.srcSubscriber = 'partials/subscriberChannelList.html';
                             $rootScope.channel = false;
                             $rootScope.subscribchannelList = true;
                             $rootScope.srcChannel = "";
                             // $rootScope.includeSubscriberChannel('subscriberChannelList.html', subscribchannelList)

                             //$location.path('/subscriberChannelList/' + subscriberId);
                         }).error(function (serverResponse, status, headers, config) {
                             //     debugger
                             alert(status + " - Error Occured!");
                         }
                     );

              },
              function (errorresult) { alert(errorresult) }
            );

            //PayPalMobile.renderFuturePaymentUI(function (authorization) {
            //    alert("authorization: " + JSON.stringify(authorization, null, 4));
            //},
            //function (errorresult) { alert(errorresult) }
            //);
        }

        $scope.GoFuture = function GoFuture(channelId, channelName, channelPrice) {
             alert("amar kumar");
            // alert('buy now!');
            debugger
            alert(channelId + "," + channelName + "," + channelPrice);
            // alert("amar");
             alert('buy now!');
            ////create paypal payment details object
            var paymentDetails = new PayPalPaymentDetails(
             channelPrice, // subtotal (amount ex shipping and tax)
               "0.00", // shipping
               "0.00"  // tax
            );
            debugger
            var payment = new PayPalPayment(
              channelPrice, // amount (the sum of the fields above)
              "USD",   // currency (in ISO 4217 format)
             channelName, // description of the payment
              "Sale",  // Sale (immediate payment) or Auth (authorization only)
              paymentDetails // the object prepared above, optional
            );
            debugger
            PayPalMobile.renderFuturePaymentUI(function (authorization) {
                debugger
                alert("authorization code : " + authorization.response.code);
                debugger
                var dataToPost = {
                    clientId: "AZAm1RAlrdDzXRBC5h8KPThw9BcdHzjtGs4lp1SQgl73PaR0wpVqveJimO0v", secret: "EJ6YYxCbJ0Ny7L_NgqI0GV8Q-qdn85t4hjavpOAxQmau6Uj1nJiY0fYYEnnl",
                    url: "http://www.prvii.com", code: authorization.response.code
                };
                debugger
                $http.post($rootScope.SERVICE_URL + "ShoppingCart/createAccessToken", dataToPost)
                             .success(function (serverResponse, status, headers, config) {
                                 debugger
                                 alret('Amar');

                             }).error(function (serverResponse, status, headers, config) {
                                 debugger
                                 alret('Amar error');
                                 alert(status + " - Error Occured!");
                             }
                         );

                debugger

                alert("test");
               // alert("authorization response_type : " + authorization.response_type);
               // debugger
               // var metadataId = PayPalMobile.getClientMetadataI;
               // debugger
               //  PayPalConfiguration.caller
               // debugger
               // alert("authorization: " + JSON.stringify(authorization, null, 4));
               // debugger
               // var dataToPost = { CartID: $scope.ShoppingCartID, Token: authorization.response.code };
               // alert(dataToPost);

               // debugger
               // var paypal_sdk = require('paypal-rest-sdk');
               // paypal_sdk.configure({
               //     'host': 'api.sandbox.paypal.com',
               //     'client_id': 'AZAm1RAlrdDzXRBC5h8KPThw9BcdHzjtGs4lp1SQgl73PaR0wpVqveJimO0v',
               //     'client_secret': 'EJ6YYxCbJ0Ny7L_NgqI0GV8Q-qdn85t4hjavpOAxQmau6Uj1nJiY0fYYEnnl' });
               // paypal_sdk.generate_token(function(error, token){
               //     if (error) {
               //         debugger
               //         console.error(error);
               //     } else {
               //         debugger
               //         console.log(token);
               //     }
               // });
  
               // debugger
               //// https://api.paypal.com/v1/oauth2/token&Content-Type:application/x-www-form-urlencoded&Authorization:BasicQWZV...==&grant_type=authorization_code&response_type=token&redirect_uri=urn:ietf:wg:oauth:2.0:oob&code=EBYhRW3ncivudQn8UopLp4A28'

               // $http.post("https://api.paypal.com/v1/oauth2/token&Content-Type:application/x-www-form-urlencoded&Authorization:Basic&grant_type=authorization_code&response_type=token&redirect_uri=urn:ietf:wg:oauth:2.0:oob&code="+authorization.response.code)
               //                .success(function (serverResponse, status, headers, config) {
               //                    alret('Amar');

               //                }).error(function (serverResponse, status, headers, config) {
               //                    alret('Amar error');
               //                    alert(status + " - Error Occured!");
               //                }
               //            );

               // debugger
               // $http.post($rootScope.SERVICE_URL + "ShoppingCart/CreateRecurringPaymentsProfile", dataToPost)
               //             .success(function (serverResponse, status, headers, config) {
               //                 alret('Amar');
                                
               //             }).error(function (serverResponse, status, headers, config) {
               //                 alret('Amar error');
               //                 alert(status + " - Error Occured!");
               //             }
               //         );
            },
            function (errorresult) { alert(errorresult) }
            );
        }


        $scope.RemoveItem = function RemoveItem() {
            debugger
            var subscriberId = $rootScope.USER_ID;
            var ChannelIds = $scope.selectedUsers;
            var shoppingCartID = $scope.ShoppingCartID;
            if (shoppingCartID > 0)
            {
                if (ChannelIds.length > 0)
                {
                    var dataToPost = { CartID: shoppingCartID, itemList: ChannelIds };
                    $scope.show = false;
                    ngProgress.start();

                    $http.post($rootScope.SERVICE_URL + "ShoppingCart/ShoppingCartRemoveItems", dataToPost)
                          .success(function (serverResponse, status, headers, config) {
                              debugger
                              $scope.cartID = serverResponse;
                              // alert(serverResponse)
                              // $location.path('/cart');
                              $scope.ShoppingCartID = serverResponse;
                              var subscriberId = $rootScope.USER_ID;
                              var dataToPost = { UserID: subscriberId };

                              $http.post($rootScope.SERVICE_URL + "ShoppingCart/BindCart", dataToPost)
                                          .success(function (serverResponse, status, headers, config) {
                                                debugger
                                              $scope.shoppingCartList = serverResponse;
                                              $scope.TotalPrice = 0.00;
                                              if (serverResponse.length > 0)
                                                  $scope.ShoppingCartID = serverResponse[0].ShoppingCartID;
                                              else
                                                  $scope.ShoppingCartID = 0;

                                              for (var i = 0; i < $scope.shoppingCartList.length; i++) {
                                                  $scope.TotalPrice = $scope.TotalPrice + $scope.shoppingCartList[i].Price;
                                              }
                                              $scope.TotalPrice = $scope.TotalPrice.toFixed(2);
                                          }).error(function (serverResponse, status, headers, config) {
                                              alert(status + " - Error Occured!");
                                          }
                                      );
                          }).error(function (serverResponse, status, headers, config) {
                              alert(status + " - Error Occured!");
                          }
                      );
                    ngProgress.complete();
                    $scope.show = true;
                }
                else
                {
                    alert("Please select a Celebrity to remove to!")
                    return false;
                }
                
                
            }
            else
            {
                alert("Please select a Celebrity to remove to!")
                return false;
            }
            

        }

        $scope.ShoppingContinue = function () {
           // debugger
            $location.path('/home');
          // debugger
            $rootScope.group = false;
            $rootScope.celeberity = false;
            $rootScope.subscribchannel = true;
            $rootScope.contact = false;
            $rootScope.subscribchannelList = false;
            $rootScope.srcSubscriber = "";
            $rootScope.channel = true;
            $rootScope.srcChannel = 'partials/newChannelsList.html';
           // debugger
        }

        $scope.Checkout = function (shoppingCartID, TotalPrice)
        {
           // debugger
            if (shoppingCartID > 0)
            {
                if (TotalPrice > 0) {

                    var dataToPost = { CartID: shoppingCartID };

                    $http.post($rootScope.SERVICE_URL + "ShoppingCart/ShoppingCartCheckout", dataToPost)
                                .success(function (serverResponse, status, headers, config) {
                                   // debugger
                                    var ref = window.open(serverResponse.ExpressCheckoutURL, '_blank', 'location=no', 'closebuttoncaption=back');
                                    ref.show();
                                    //debugger
                                    ref.addEventListener('loadstart', function (event) {
                                        // alert("Loader ");
                                    });
                                    ref.addEventListener('loadstop', function (event) {

                                       // alert(event.type + '---' + event.url + '--' + event.code);

                                        if (event.url.indexOf('CheckoutCompleteMobile.aspx') != -1) {
                                            ref.close();
                                        }

                                        if (event.url.indexOf('CheckoutCancelMobile.aspx') != -1) {
                                            ref.close();
                                        }
                                    });
                                    ref.addEventListener('exit', function (event) {

                                        
                                        $http.post($rootScope.SERVICE_URL + "ShoppingCart/GetShoppingCartChannelID", dataToPost)
                                            .success(function (serverResponse, status, headers, config) {
                                                $rootScope.srcSubscriber = "";
                                                $rootScope.srcSubscriber = 'partials/subscriberChannelMessage.html';
                                                $rootScope.subscribchannelList = true;
                                                $rootScope.channelId = serverResponse;
                                                $location.path('/home');

                                            }).error(function (serverResponse, status, headers, config) {
                                                alert(status + " - Error Occured!");
                                            }
                                        )
                                        


                                        //// alert("End Bro");
                                        //// debugger
                                        //$location.path('/home');
                                        //// debugger
                                        //$rootScope.group = false;
                                        //$rootScope.celeberity = false;
                                        //$rootScope.subscribchannel = true;
                                        //$rootScope.contact = false;
                                        //$rootScope.subscribchannelList = true;
                                        //$rootScope.srcSubscriber = 'partials/subscriberChannelList.html';
                                        //$rootScope.channel = false;
                                        //$rootScope.srcChannel = '';
                                    });


                                }).error(function (serverResponse, status, headers, config) {
                                    alert(status + " - Error Occured!");
                                }
                            );
                }
                else
                {
                    alert("Total Price should be greater than 0.")
                    return false;
                }
            }
            else
            {
                alert("Please select a Celebrity to subscribe to!")
                return false;
            }
            //var Page = 'http://203.92.34.35/prviiwebTest/WebPages/ShoppingCarts/checkout.aspx?CartID=' + shoppingCartID;

            //var ref = window.open(Page, '_blank', 'location=no', 'closebuttoncaption=back');
            //ref.show();
            ////debugger
            //ref.addEventListener('loadstart', function (event) {
            //   // debugger
            //    //alert(event.type + '---' + event.url + '--' + event.code);
            //   // alert(event.url)
            //   // alert("loadstart  ttt" + event);
            //});
            //ref.addEventListener('loadstop', function (event) {
            //  //  alert("loadstop iiiiiiiii" + event);
            //    // alert(event.type + '---' + event.url + '--' + event.code);

            //    if(event.url.indexOf('CheckoutCompleteMobile.aspx')!=-1)
            //    {
            //        ref.close();
            //    }

            //    if (event.url.indexOf('CheckoutCancelMobile.aspx') != -1) {
            //        ref.close();
            //    }
            //});
            //ref.addEventListener('exit', function (event) { alert("End Bro ttttttttt"); });

          
            
         
        }



    }]
    )
})();