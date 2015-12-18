(function () {
    'use strict';
    var prviiAppnewChannelsList = angular.module('prviiApp.newChannelsList', []);

    prviiAppnewChannelsList.controller('newChannelsListController', ['$rootScope', '$scope', '$parse', '$http', '$routeParams', '$location', 'ngProgress', '$compile',
    function ($rootScope, $scope, $parse, $http, $routeParams, $location, ngProgress, $compile) {

          $scope.count = 0;

       
        if (device.platform == "iOS") {
            $rootScope.IsIosDeveice = true;
            store.verbosity = store.DEBUG;
            // alert("hui");
           //onPageShow();


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
            function (payment) { alert("payment success1: " + payment.response.id + "---" + JSON.stringify(payment)); },
            function (errorresult) { alert(errorresult) }
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
                      alert(status + " - Error Occured!");
                  }
              );
            ngProgress.complete();
            $scope.show = true;

        }


        $scope.Buy = function (pid, channelId) {
            store.order(pid);
        }

        $scope.onPageShow = function onPageShow() {

            angular.forEach($scope.channelList, function (value, key) {
                store.register({
                    id: "io.cordova.PrviiMobile.NR." + value.ID,
                    alias: value.Name,
                    type: store.CONSUMABLE
                });
            });


           // store.refresh();
            store.when("product").updated(function (p) {
                $scope.render(p);
            });
          //  store.refresh();
            // store.validator = "http://apiuat.prvii.org/api/InAppPurchase/ValidateReceipt";
            store.validator = function (product, callback) {
                debugger
                product.finish();
                var ss = angular.toJson(product);
                console.log(angular.toJson(product));

                var dataToPost1 = { receiptData: product.transaction.transactionReceipt, receiptToken: product.transaction.appStoreReceipt };
                $http.post($rootScope.SERVICE_URL + "InAppPurchase/ValidateReceipt", dataToPost1)
                            .success(function (serverResponse, status, headers, config) {
                                debugger
                                alert(serverResponse);
                                var pp = angular.toJson(serverResponse);
                                var obj = JSON.parse(serverResponse);
                                var ProductID = obj.receipt.product_id;
                                alert(ProductID.replace('io.cordova.PrviiMobile.NR.', ''));
                                var channelID = ProductID.replace('io.cordova.PrviiMobile.NR.', '');
                                var subscriberId = $rootScope.USER_ID;
                                var dataToPost = { UserID: subscriberId, ChannelID: channelID };

                                $http.post($rootScope.SERVICE_URL + "ShoppingCart/subscribedByIos", dataToPost)
                                      .success(function (serverResponse, status, headers, config) {
                                          debugger
                                          $scope.cartID = serverResponse;
                                          var dataToPost2 = { ID: subscriberId };
                                          $http.post($rootScope.SERVICE_URL + "channel/GetUnSubscribChannelList", dataToPost2)
                                                   .success(function (serverResponse, status, headers, config) {
                                                       debugger
                                                       //$route.reload();
                                                       $scope.channelList = serverResponse;
                                                       if (device.platform == "iOS") {
                                                           $scope.newChannelList = [];
                                                           onPageShow();

                                                       }
                                                   }).error(function (serverResponse, status, headers, config) {
                                                       alert(status + " - Error Occured!");
                                                   }
                                               );
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
                order.verify();
                // order.finish();                
            });

             store.refresh();
        }

        $scope.render1 = function render1(product) {
            debugger
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
                    $scope[elId] = true;

                    var IsValidData = true;
                    angular.forEach($scope.newChannelList, function (v, k) {
                        if (v.ProductID == elId) {
                            IsValidData = false;
                        }
                    });
                    if (IsValidData) {
                        angular.forEach($scope.channelList, function (value, key) {
                            if ("io.cordova.PrviiMobile.NR." + value.ID == elId) {
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
                                    ProductID: "io.cordova.PrviiMobile.NR." + value.ID
                                });
                            }
                        });
                    }
                }

            }
        }


        $scope.render= function render(product) {

            var elId = product.id;

           debugger
            if (!product.loaded) {
                return;
            }
            else if (!product.valid) {
               // $("#L1").append("<li><h3>" + product.title + " non valido</h3></li>");
            }
            else if (product.valid) {

                if (product.canPurchase) {
                    var ProductID = product.id;
                    var channelID = ProductID.replace('io.cordova.PrviiMobile.NR.', '');
                    angular.element(document.getElementById('addCelebrities')).append($compile("<div  class='row><div class='col-sm-3'><div class='celebrity-img'> <a ng-click='includeSubscriberChannelFullDetails('subscriberchannelFullDetails.html'," + channelID + ")'> <img ng-src='{{SERVICE_URL}}channel/GetMedia?channelID=" + channelID + "&mediaTypeID=1' /></a>  <div class='clearfix'></div></div></div>  <div class='col-sm-3'><div class='celebrity-name'><a ng-click='includeSubscriberChannelFullDetails('subscriberchannelFullDetails.html',channel.ID)'>" + product.title + " </a></div></div> <div class='col-sm-3'><div class='celebrity-name'>" + product.price + "</div></div>    <div class='col-sm-3' ><button class='unsubscribe-btn' data-alert=" + product.id + ">Buy</button> </div>      </div>")($scope));


                  //  $("#L1").append("<li><a href='#'><h3>" + product.title + "</h3><p>" + product.description + "</p><p class='ui-li-aside'><strong>" + product.price + "</strong></p></a><a href='#' id=" + product.id + " data-icon='gear' data-iconpos='notext'></a></li>");
                }
               
            }
        }            

    }]
    );


    prviiAppnewChannelsList.directive("addCelebrities", function ($compile) {
        return function (scope, element, attrs) {
            element.bind("click", function () {
                scope.count++;
                angular.element(document.getElementById('space-for-buttons')).append($compile("<div><button class='btn btn-default' data-alert=" + scope.count + ">Show alert #" + scope.count + "</button></div>")(scope));
            });
        };
    });

    //prviiAppnewChannelsList.directive("buyButton", function () {
    //    return function (scope, element, attrs) {
    //        element.bind("click", function () {
    //            console.log(attrs);
    //            alert("This is alert #" + attrs.alert);
    //        });
    //    };
    //});



    
    //prviiAppnewChannelsList.directive("addbuttonsbutton", function () {
    //    return {
    //        restrict: "E",
    //        template: "<button addbuttons>Click to add buttons</button>"
    //    }
    //});

   
    //prviiAppnewChannelsList.directive("addbuttons", function ($compile) {
    //    return function (scope, element, attrs) {
    //        element.bind("click", function () {
    //            scope.count++;
    //            angular.element(document.getElementById('space-for-buttons')).append($compile("<div><button class='btn btn-default' data-alert=" + scope.count + ">Show alert #" + scope.count + "</button></div>")(scope));
    //        });
    //    };
    //});

    
    prviiAppnewChannelsList.directive("alert", function () {
        return function (scope, element, attrs) {
            element.bind("click", function () {
                console.log(attrs);
                alert("This is alert #" + attrs.alert);
                store.order(attrs.alert);

            });
        };
    });




})();