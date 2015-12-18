(function () {
    'use strict';
    var prviiAppnewChannelsList = angular.module('prviiApp.newChannelsList', []);

    prviiAppnewChannelsList.controller('newChannelsListController', ['$rootScope', '$scope', '$parse', '$http', '$routeParams', '$location', 'ngProgress', '$compile',
    function ($rootScope, $scope, $parse, $http, $routeParams, $location, ngProgress, $compile) {
        //debugger
        if (device.platform == "iOS") {
            $rootScope.IsIosDeveice = true;
            store.verbosity = store.DEBUG;

        }
        else {
            $rootScope.IsIosDeveice = false;
        }

        $scope.show = false;
        ngProgress.start();
        // var subscriberId = $routeParams.subscriberId;
        var subscriberId = $rootScope.USER_ID;
        var dataToPost = { ID: subscriberId };
        // debugger

        //GetChannels
        $http.post($rootScope.SERVICE_URL + "channel/GetUnSubscribChannelList", dataToPost)
                    .success(function (serverResponse, status, headers, config) {
                      // debugger                      
                        $scope.channelList = serverResponse;
                        if (device.platform == "iOS") {
                            $scope.newChannelList = [];
                            $scope.onPageShow();

                        }
                    }).error(function (serverResponse, status, headers, config) {
                         alert(status + " - Error Occured!");
                    }
                );
        ngProgress.complete();
        $scope.show = true;

        // channelId, channelName, channelPrice
        $scope.channelSubscribe = function channelSubscribe() {

            var paymentDetails = new PayPalPaymentDetails(
            "5.00", // subtotal (amount ex shipping and tax)
             "3.00", // shipping
             "2.00"  // tax
          );


            var payment = new PayPalPayment(
             "20.00", // amount (the sum of the fields above)
             "USD",   // currency (in ISO 4217 format)
             "Telerik T-Shirt", // description of the payment
             "Sale",  // Sale (immediate payment) or Auth (authorization only)
             paymentDetails // the object prepared above, optional
           );

            PayPalMobile.renderSinglePaymentUI(
            payment,
            function (payment) {
                // alert("payment success1: " + payment.response.id + "---" + JSON.stringify(payment));
            },
            function (errorresult) {
                //alert(errorresult)
            }
          );


        }

        $scope.Go = function Go(channelId, channelName, channelPrice) {

            ////create paypal payment details object
            var paymentDetails = new PayPalPaymentDetails(
              channelPrice.toFixed(2), // subtotal (amount ex shipping and tax)
               "0.00", // shipping
               "0.00"  // tax
            );

            var payment = new PayPalPayment(
              channelPrice.toFixed(2), // amount (the sum of the fields above)
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
                             // alert('subscription successful');
                             $rootScope.srcSubscriber = 'partials/subscriberChannelList.html';
                             $rootScope.channel = false;
                             $rootScope.subscribchannelList = true;
                             $rootScope.srcChannel = "";
                             // $rootScope.includeSubscriberChannel('subscriberChannelList.html', subscribchannelList)

                             //$location.path('/subscriberChannelList/' + subscriberId);
                         }).error(function (serverResponse, status, headers, config) {
                             //     debugger
                             // alert(status + " - Error Occured!");
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

        $scope.GoFuture = function Go(channelId, channelName, channelPrice) {
            // alert(channelId + "," + channelName + "," + channelPrice.toFixed(2));
            // alert("amar");
            // alert('buy now!');


            PayPalMobile.renderFuturePaymentUI(function (authorization) {
                //  alert("authorization: " + JSON.stringify(authorization, null, 4));
            },
            function (errorresult) { alert(errorresult) }
            );
        }


        $scope.AddToCart = function AddToCart(channelId) {
            if ($rootScope.USER_ChannelID == channelId) {
                alert("You cannot subscribe yourself!");
                return;
            }
            var subscriberId = $rootScope.USER_ID;
            var dataToPost = { UserID: subscriberId, ChannelID: channelId };
            //alert(channelId);
            $scope.show = false;
            ngProgress.start();

            $http.post($rootScope.SERVICE_URL + "ShoppingCart/AddShoppingCartItem", dataToPost)
                  .success(function (serverResponse, status, headers, config) {
                      //debugger
                      $scope.cartID = serverResponse;
                      //alert(serverResponse)
                      $location.path('/cart');
                  }).error(function (serverResponse, status, headers, config) {
                      // alert(status + " - Error Occured!");
                  }
              );
            ngProgress.complete();
            $scope.show = true;

        }


        $scope.Buy = function (pid, channelId) {
            store.order(pid);
        }

        $scope.onPageShow = function onPageShow() {
            // debugger
            angular.forEach($scope.channelList, function (value, key) {
                //  debugger
                store.register({
                    id: "com.globrin.PrviiMobile." + value.ID,
                    alias: value.Name,
                    type: store.CONSUMABLE
                });
            });


            store.refresh();
            store.when("product").updated(function (p) {
                $scope.render(p);
            });

            // store.validator = "http://apiuat.prvii.org/api/InAppPurchase/ValidateReceipt";
            store.validator = function (product, callback) {
                //debugger
                product.finish();
                var ss = angular.toJson(product);
                console.log(angular.toJson(product));
               // debugger
                var dataToPost1 = { receiptData: product.transaction.transactionReceipt, receiptToken: product.transaction.appStoreReceipt };
                $http.post($rootScope.SERVICE_URL + "InAppPurchase/ValidateReceipt", dataToPost1)
                            .success(function (serverResponse, status, headers, config) {
                                
                               // alert(serverResponse);
                                var pp = angular.toJson(serverResponse);
                                var obj = JSON.parse(serverResponse);
                                var ProductID = obj.receipt.product_id;
                                //alert(ProductID.replace('com.globrin.PrviiMobile.', ''));
                                var channelID = ProductID.replace('com.globrin.PrviiMobile.', '');
                                var subscriberId = $rootScope.USER_ID;
                                var dataToPost = { UserID: subscriberId, ChannelID: channelID, ResponseJSON: serverResponse };
                               // debugger
                                $http.post($rootScope.SERVICE_URL + "ShoppingCart/subscribedByIos", dataToPost)
                                      .success(function (serverResponse, status, headers, config) {
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
                                          //$scope.cartID = serverResponse;
                                          //var dataToPost2 = { ID: subscriberId };
                                          //$http.post($rootScope.SERVICE_URL + "channel/GetUnSubscribChannelList", dataToPost2)
                                          //         .success(function (serverResponse, status, headers, config) {
                                          //              // debugger
                                          //             //$route.reload();
                                          //             $scope.channelList = serverResponse;
                                          //             if (device.platform == "iOS") {
                                          //                 $scope.newChannelList = [];
                                          //                 onPageShow();

                                          //             }
                                          //         }).error(function (serverResponse, status, headers, config) {
                                          //             alert(status + " - Error Occured!");
                                          //         }
                                          //     );
                                      }).error(function (serverResponse, status, headers, config) {
                                          alert(status + " - Error Occured!");
                                      }
                                  );
                            }).error(function (serverResponse, status, headers, config) {
                               alert(status + " - Error Occured!");
                            }
                        );
            };

            store.when("product").approved(function (order) {
               // debugger
                order.verify();
                // order.finish();                
            });

            // store.refresh();
        }

        $scope.render = function render(product) {
            // debugger
            var elId = product.id;


            if (!product.loaded) {
                $scope[elId] = false;
                return;
            }
            else if (!product.valid) {
                $scope[elId] = false;
            }
            else if (product.valid) {
                if (!product.canPurchase) {
                    $scope[elId] = false;
                }
                if (product.canPurchase) {
                   debugger
                    $scope[elId] = true;

                    var IsValidData = true;
                    angular.forEach($scope.newChannelList, function (v, k) {
                        if (v.ProductID == elId) {
                            IsValidData = false;
                        }
                    });
                    if (IsValidData) {
                        angular.forEach($scope.channelList, function (value, key) {
                            if ("com.globrin.PrviiMobile." + value.ID == elId) {

                                // angular.element(document.getElementById('space-for-buttons')).append($compile("<div><button class='btn btn-default' data-alert=" + scope.count + ">Show alert #" + scope.count + "</button></div>")(scope));
                                // var channelID = elId.replace('com.globrin.PrviiMobile.', '');

                                angular.element(document.getElementById('space-for-AppStoreProduct')).append($compile("<div class='row'><div class='col-sm-3'><div class='celebrity-img'><img ng-src='{{SERVICE_URL}}channel/GetMedia?channelID=" + value.ID + "&mediaTypeID=1' /><div class='clearfix'></div></div></div>" +
                                               "<div class='col-sm-3'><div class='celebrity-name'><a ng-click='includeSubscriberChannelFullDetails('subscriberchannelFullDetails.html',channel.ID)'>" + product.title + "</a></div></div>" +
                                              "  <div class='col-sm-3'><div class='celebrity-name'>" + product.price + "</div></div>" +
                                              "  <div class='col-sm-3'><button class='unsubscribe-btn'  data-alert='" + product.id + "'>Subscribe</button></div></div>")($scope));

                                $scope.newChannelList.push({
                                    ID: value.ID,
                                    Name: value.Name,
                                    Email: value.Email,
                                    Phone: value.Phone,
                                    Price: value.Price,
                                    BillingCycleID: value.BillingCycleID,
                                    NoOfBillingPeriod: value.NoOfBillingPeriod,
                                    StatusID: value.StatusID,
                                    IsActive: value.IsActive,
                                    PriceApple: product.price,
                                    ProductID: "com.globrin.PrviiMobile." + value.ID
                                });
                            }
                        });
                    }
                }

            }
        }



        $scope.$watch('newChannelList', function (newValue, oldValue) {
            //debugger

            if (newValue === oldValue) { return; }
            $scope.newChannelList = newValue;
        }, true);




    }])

    //Directive for showing an alert on click
    prviiAppnewChannelsList.directive("alert", function () {
        return function (scope, element, attrs) {
            element.bind("click", function () {
                console.log(attrs);
                //alert("This is alert #" + attrs.alert);
                element.attr('ng-disabled', true);
                store.order(attrs.alert);
            });
        };
    });




})();