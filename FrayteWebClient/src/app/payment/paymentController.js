angular.module('ngApp.payment').controller('PaymentController', function ($scope, AppSpinner, ModalService, $state, config, PaymentService, $filter, $log, ShipmentService, TopCountryService, HomeService, SystemAlertService, $uibModal, $translate, $anchorScroll, $location) {
    //start angular carousel
    $scope.myInterval = 5000;
    $scope.noWrapSlides = false;
    $scope.active = 0;
    var slides = $scope.slides = [];
    var currIndex = 0;

    $scope.addSlide = function () {
        //var newWidth = 600 + slides.length + 1;
        slides.push({
            image: $scope.ImagePath + 'bray.png',
            id: currIndex++
        });
        slides.push({
            image: $scope.ImagePath + 'Serious_Stuff.png',
            id: currIndex++
        });
        slides.push({
            image: $scope.ImagePath + 'Client__2.png',
            id: currIndex++
        });
        slides.push({
            image: $scope.ImagePath + 'Client_3.png',
            id: currIndex++
        });
        slides.push({
            image: $scope.ImagePath + 'Client__5.png',
            id: currIndex++
        });
        slides.push({
            image: $scope.ImagePath + 'Client__6.png',
            id: currIndex++
        });
    };

    $scope.randomize = function () {
        var indexes = generateIndexesArray();
        assignNewIndexesToSlides(indexes);
    };

    for (var i = 0; i < 1; i++) {
        $scope.addSlide();
    }

    //end

    $scope.setScroll = function (printSectionId) {
        $location.hash(printSectionId);
        $anchorScroll();
    };
    $scope.paypalbutton = {
        hide: true
    };

    var setMultilingualOptions = function () {
        $translate(['Success', 'Record_Saved', 'FrayteError_Validation', 'ErrorSavingRecord', 'GettingDetails_Error', 'CancelShipmentError_Validation',
            'GeneratePdfError_Validation', 'GeneratePdfError_Validation', 'SuccessfullySendlLabel_Validation', 'SendingMailError_Validation',
        'EnterValidEmailAdd', 'TrackShipmentNotTrack_Error', 'FrayteWarning', 'PleaseCorrectValidationErrors', 'SuccessfullySavedInformation', 'ErrorSavingRecord', 'FrayteError', 'FrayteSuccess']).then(function (translations) {
            $scope.Success = translations.Success;
            $scope.RecordSaved = translations.Record_Saved;
            $scope.FrayteErrorValidation = translations.FrayteError_Validation;
            $scope.ErrorSavingRecord = translations.ErrorSavingRecord;
            $scope.GettingDetailsError = translations.GettingDetails_Error;
            $scope.CancelShipmentErrorValidation = translations.CancelShipmentError_Validation;
            $scope.GeneratePdfErrorValidation = translations.GeneratePdfError_Validation;
            $scope.SuccessfullySendlLabelValidation = translations.SuccessfullySendlLabel_Validation;
            $scope.SendingMailErrorValidation = translations.SendingMailError_Validation;
            $scope.EnterValidEmailAdd = translations.EnterValidEmailAdd;
            $scope.TrackShipmentNotTrackError = translations.TrackShipmentNotTrack_Error;
            $scope.Frayte_Warning = translations.FrayteWarning;
            $scope.PleaseCorrect_ValidationErrors = translations.PleaseCorrectValidationErrors;
            $scope.Successfully_SavedInformation = translations.SuccessfullySavedInformation;
            $scope.ErrorSaving_Record = translations.ErrorSavingRecord;
            $scope.Frayte_Error = translations.FrayteError;
            $scope.Frayte_Success = translations.FrayteSuccess;


        });
    };

    //state to go Home Page
    $scope.goToHome = function () {
        $state.go('home.welcome');
    };
    //Login popup
    $scope.open = function (size) {
        var modalInstance = $uibModal.open({
            animation: true,
            templateUrl: 'logon/logon.tpl.html',
            controller: 'LogonController',
            windowClass: 'Logon-Modal',
            size: 'sm'
        });
    };

    $scope.isCurrentPath = function (path) {
        return $state.is(path);
    };

    $scope.SetCountries = function () {

        ShipmentService.GetInitials().then(function (response) {
            var Country = {};
            $scope.countries = response.data.Countries;

            var curr = response.data.CurrencyTypes;
            // Set countries
            $scope.Countries = TopCountryService.TopCountryList($scope.countries);
            for (var i = 0 ; i < $scope.Countries.length; i++) {
                if ($scope.Countries[i].Code === 'CH2') {
                    $scope.Countries.splice(i, 1);
                }
                else if ($scope.Countries[i].Code === 'CHN') {
                    $scope.Countries[i].Name = "China";
                }
            }
            if (config.SITE_COUNTRY === 'COM') {
                Country = $filter('filter')($scope.Countries, { Code: 'HKG' });
                $scope.senddata.selectedcountry = Country[0];
            }
            else if (config.SITE_COUNTRY === 'CO.UK') {
                Country = $filter('filter')($scope.Countries, { Code: 'GBR' });
                $scope.senddata.selectedcountry = Country[0];
            }
        });
    };

    $scope.SetCurrency = function () {
        $scope.currency = [
   { CurrencyName: "Australian Dollar", CurrencyCode: "AUD" },

   { CurrencyName: "British Pound", CurrencyCode: "GBP" },

   { CurrencyName: "Hong Kong Dollar", CurrencyCode: "HKD" },

   { CurrencyName: "US Dollar", CurrencyCode: "USD" }
        ];
    };
    $scope.getvalue = function (Pv_Amount) {
        if (Pv_Amount === undefined) {
            $scope.usd = '';
        }
        if (Pv_Amount !== undefined && !isNaN(Pv_Amount)) {
            var amount = Pv_Amount * 3.5 / 100;
            var amountData = parseFloat(Pv_Amount).toFixed(2) + parseFloat(amount).toFixed(2);

            $scope.senddata.Pv_Amount = parseFloat(Pv_Amount) + parseFloat(amount);
            if (Pv_Amount > 0) {
                if ($scope.PaymentCurrency !== null) {
                    $scope.usd = 'Toatal Paid Amount : ' + $scope.senddata.Pv_Amount.toFixed(2) + ' ' + $scope.PaymentCurrency.CurrencyCode;
                }

            }
            else {
                $scope.usd = '';
            }
            $scope.senddata.Amout = Pv_Amount;
        }
    };

    $scope.SetInitialValues = function () {
        $scope.senddata = {
            selectedcountry:
                null,
            Pv_CurrencyId: '840',
            Pv_CountryId: '',
            Pv_Amount: '',
            Pv_ErrorResponseUrl: config.SERVICE_URL + '/PayVisionCheckout/ErrorUrl',
            Pv_SuccessResponseUrl: config.SERVICE_URL + '/PayVisionCheckout/SuccessUrl',
            CompanyName: '',
            FirstName: '',
            LastName: '',
            Email: '',
            Address1: '',
            Address2: '',
            City: '',
            State: '',
            ZipCode: '',
            CardNumber: '',
            cvc: '',
            ExpDate: '',
            Invoice_no: '',
            Country: '',
            Currency: '',
            Amout: '',
            ClientPaymentSuccessUrl: config.CLIENT_SUCCESS_URL,
            ClientPaymentErrorUrl: config.CLIENT_ERROR_URL,
            Items: [{
                ItemCode: 'IT-0001',
                Itemname: 'Paid Invoice Amount',
                Itemdescription: 'Paid Invoice Amount',
                quantity: '1'
            }]
        };
    };


    var myfunction = function stripeResponseHandler(status, response) {
        // Grab the form:
        var $form = $('#payment');
        var myJson = {
            PaymentInfo: $scope.senddata,
            CardNo: '',
            ExpiryDate: '',
            CVC: '',
            EmailId: '',
            EmailCC: $scope.ccMail,
            Amount: 0,
            Currency: 'USD',
            Description: 'Example Charge',
            Token: ''
        };

        //Set Value on this JSON
        myJson.Token = response.id;
        myJson.CardNo = $scope.senddata.CardNumber;
        myJson.ExpiryDate = $scope.senddata.ExpDate + '/' + $scope.senddata.ExpYear;
        myJson.CVC = $scope.senddata.cvc;
        myJson.EmailId = $scope.senddata.CardNumber;
        myJson.Amount = $scope.senddata.Pv_Amount.toFixed(2);
        $scope.senddata.Currency = $scope.PaymentCurrency;
        myJson.Currency = $scope.PaymentCurrency.CurrencyCode;
        myJson.Description = 'Example Charge';

        //Post the data
        PaymentService.StripeInitiatePayment(myJson).then(function (response) {
            if (response.data === 'Succeeded' || response.data === 'succeeded') {
                AppSpinner.hideSpinnerTemplate();
                $state.go('payment-success');
            }
            else {
                AppSpinner.hideSpinnerTemplate();
                $state.go('payment-error');
            }
        }, function () {
            AppSpinner.hideSpinnerTemplate();
            $state.go('payment-error');
        });

        //if (response.error) { // Problem!

        //    // Show the errors on the form:
        //    $form.find('.payment-errors').text(response.error.message);
        //    $form.find('.submit').prop('disabled', false); // Re-enable submission

        //} else { // Token was created!

        //    // Get the token ID:
        //    var token = response.id;

        //    // Insert the token ID into the form so it gets submitted to the server:
        //    $form.append($('<input type="hidden" name="stripeToken">').val(token));
        //    //$('#txtToken').text = $('stripeToken').val;
        //    // Submit the form:
        //    $form.get(0).submit();
        //}
    };

    $scope.sendrequest = function (isValid, senddata, PaymentType) {
        if (isValid) {

            if ($scope.paypalValidate) {
                $scope.paypalbutton.hide = false;
            }
            else {
                if (PaymentType === "Stripe") {
                    senddata.Pv_CountryId = $scope.senddata.selectedcountry.Code;
                    senddata.Country = $scope.senddata.selectedcountry.Name;

                    //Here we need to call Stripe code
                    var $form = $('#PaymentPayvisionForm');

                    $scope.Template = 'directBooking/ajaxLoader.tpl.html';
                    AppSpinner.showSpinnerTemplate('Processing payment', $scope.Template);
                    // Request a token from Stripe:
                    //Stripe.card.createToken($form, stripeResponseHandler);
                    Stripe.card.createToken($form, myfunction);

                }
            }

        }
        else {

        }
    };

    $scope.paypalPaymentCheckOut = function () {

        $scope.PaymentPaypal.Amount = $scope.PaymentPaypal.TotalAmount;

        PaymentService.PayWithPaypal($scope.PaymentPaypal).then(function (response) {
            debugger;

        }, function () {
            debugger;

        });


    };
    $scope.changePaymentType = function (tab) {
        if (tab !== undefined && tab !== null && tab === 'Stripe') {
            $scope.PaymentType = "Stripe";
        }
        else if (tab !== undefined && tab !== null && tab === 'PayPal') {
            $scope.PaymentType = "Paypal";
        }
    };

    // Pay pal integration 
    $scope.sendMailToCustomer = function (data) {

        $scope.Template = 'directBooking/ajaxLoader.tpl.html';
        AppSpinner.showSpinnerTemplate('Sending Payment Mail', $scope.Template);
        var myJson = {
            PaymentInfo: $scope.senddata,
            BalanceTransactionId: '',
            CardNo: '',
            ExpiryDate: '',
            CVC: '',
            EmailId: '',
            EmailCC: $scope.ccMail,
            Amount: 0,
            Currency: 'USD',
            Description: 'Example Charge',
            Token: '',
            PaymentCompany: ''
        };


        //Set Value on this JSON
        //myJson.Token = response.id;
        //   myJson.CardNo = $scope.senddata.CardNumber;
        //  myJson.ExpiryDate = $scope.senddata.ExpDate + '/' + $scope.senddata.ExpYear;
        // myJson.CVC = $scope.senddata.cvc;
        if (data !== undefined && data !== null) {
            myJson.BalanceTransactionId = data.paymentID;
        }
        myJson.EmailId = $scope.senddata.CardNumber;
        myJson.Amount = $scope.senddata.Pv_Amount;
        $scope.senddata.Currency = $scope.PaymentCurrency;
        myJson.Currency = $scope.PaymentCurrency.CurrencyCode;
        myJson.Description = 'Example Charge';

        if ($scope.clientCompany === 'vytalSupport') {
            myJson.PaymentCompany = "VytalSupport";

        }
        else if ($scope.clientCompany === 'whytecliff') {
            myJson.PaymentCompany = "Whytecliff";
        }
        else if ($scope.clientCompany === 'cliffPremiums') {
            myJson.PaymentCompany = "CliffPremiums";
        }

        //Post the data
        PaymentService.SendPaypalMail(myJson).then(function (response) {
            if (response.data === 'success') {
                AppSpinner.hideSpinnerTemplate();
                $state.go('payment-success');
            }
            else {
                AppSpinner.hideSpinnerTemplate();
                $state.go('payment-error');
            }
        }, function () {
            AppSpinner.hideSpinnerTemplate();
            $state.go('payment-error');
        });
    };

    $scope.PayPalConfirmation = function (paymentDetails, data, actions) {
        var modalOptions = {
            headerText: 'PayPal Payment Confirmation',
            bodyText: 'Please confirm the PayPal payment confirmation.'
        };

        ModalService.Confirm({}, modalOptions).then(function (result) {
            return actions.payment.execute().then(function () {
                debugger;
                // Show a success page to the buyer
                $scope.sendMailToCustomer(data);
                toaster.pop({
                    type: 'success',
                    title: $scope.Frayte_Success,
                    body: 'Payment processed successfully',
                    showCloseButton: true
                });


                //   alert('Payment processed successfully');
            });
        });
    };

    $scope.setFormScope = function (scope) {
        debugger;
        console.log(scope.myForm);
        this.formScope = scope.myForm;
    };

    (function ($) {
        $(document).ready(function () {
            //$(".small-menu").hide();
            $(".large-menu").show();
            $(function () {
                $(window).scroll(function () {
                    if ($(this).scrollTop() > 150) {
                        $(".small-menu").show();
                        $(".large-menu").hide();
                    }
                    else {
                        $(".small-menu").hide();
                        $(".large-menu").show();
                    }
                });
            });
            $('.menu-button').click(function () {
                $(".small-menu").hide();
                $(".large-menu").show();
            });
        });
    }(jQuery));

    function init() {
        $scope.paypalValidate = false;
        $scope.payPal = false;

        $scope.PaymentType = "Stripe";
        $scope.PaymentPaypal = {
            Quantity: 1,
            Amount: '',
            TotalAmount: '',
            Currency: 'USD',
            PaymentType: 'Sale'
        };


        $scope.tabs = [
    {
        Id: 1,
        heading: "Credit Card"
    }, {
        Id: 2,
        heading: "Pay By Paypal"
    }];
        $scope.Pv_Amount = '';
        $scope.SetCountries();
        $scope.SetCurrency();
        $scope.SetInitialValues();

        if (config.SITE_COUNTRY === 'CO.UK') {

            $scope.PaymentCurrency = { CurrencyName: "British Pound", CurrencyCode: "GBP" };
            $scope.senddata.Currency = $scope.PaymentCurrency;
        }
        else {
            $scope.PaymentCurrency = { CurrencyName: "US Dollar", CurrencyCode: "USD" };
            $scope.senddata.Currency = $scope.PaymentCurrency;
        }
        //Stripe.setPublishableKey('pk_test_Oq0qIoEduarpwEEizP4SVFDN');
        //Live Striple Key = "pk_live_5sqdhHj6ioDoN8Ts6oNErfn3"
        var key = config.Stripe_PaymentKey;
        Stripe.setPublishableKey(key);


        // Pay pal integration 
        paypal.Button.render({

            env: config.Paypal_PaymentMode, // Optional: specify 'sandbox' or 'production' environment

            client: {
                sandbox: config.Paypal_PaymentKey_Sandbox,
                production: config.Paypal_PaymentKey_Production
            },

            payment: function () {
                debugger;
                //$scope.submitted = true;
                //console.log($scope.myForm);
                //var $form = $('#myForm.PaymentPayvisionForm');
                //$form.submit();
                //$scope.sendrequest($scope.myForm.PaymentPayvisionForm.$valid, $scope.senddata);
                $scope.senddata.Currency = $scope.PaymentCurrency;
                var env = this.props.env;
                var client = this.props.client;

                return paypal.rest.payment.create(env, client, {
                    transactions: [
                        {
                            amount: { total: $scope.senddata.Pv_Amount, currency: $scope.senddata.Currency.CurrencyCode }
                        }
                    ]
                });
            },

            commit: true, // Optional: show a 'Pay Now' button in the checkout flow

            onAuthorize: function (data, actions) {
                debugger;
                console.log(data);
                // Optional: display a confirmation page here
                var paymentDetails = actions.payment.get();
                console.log(paymentDetails);
                if (data.paymentID !== '' && data.paymentID !== null) {
                    $scope.PayPalConfirmation(paymentDetails, data, actions);
                }
            },
            onError: function (err) {
                // Show an error page here, when an error occurs
                debugger;
                $state.go('payment-error');
            },
            onCancel: function (data, actions) {
                debugger;
                $state.go('payment-error');
            }


        }, '#paypal-button');

        HomeService.GetCurrentOperationZone().then(function (response) {
            if (response.data) {
                var OperationZoneId = response.data.OperationZoneId;

                if (OperationZoneId === 1) {
                    $scope.ccMail = 'accounts@frayte.com';
                    //$scope.ccMail = 'vikshit9292@gmail.com';
                }
                else if (OperationZoneId === 2) {
                    $scope.ccMail = 'accounts@frayte.co.uk';
                    
                }

                var CurrentDate = new Date();
                SystemAlertService.GetPublicSystemAlert(OperationZoneId, CurrentDate).then(function (response) {
                    $scope.result = response.data;
                    if (response.data.length > 0) {
                        $scope.cssshow = false;
                    }
                    else {
                        $scope.cssshow = true;
                    }
                    $scope.result.reverse();
                    if ($scope.result.length > 0) {
                        $scope.Month = ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul', 'Aug', 'Sep', 'Oct', 'Nov', 'Dec'];
                        for (i = 0; i < $scope.result.length; i++) {
                            var Newdate = $scope.result[i].FromDate;

                            var newdate = [];
                            var Date = [];
                            var Time = [];
                            var gtMonth1 = [];
                            newdate = Newdate.split('T');
                            Date = newdate[0].split('-');
                            var gtDate = Date[2];
                            gtMonth1 = Date[1].split('');
                            var gtMonth2 = Date[1];
                            var gtMonth3 = gtMonth1[0] === "0" ? gtMonth1[1] : gtMonth2;
                            var gtMonth4 = gtMonth3--;
                            var gtMonth = $scope.Month[gtMonth3];
                            var gtYear = Date[0];
                            Time = $scope.result[i].FromTime.split(':');
                            var gtHour = Time[0];
                            var gtMin = Time[1];


                            $scope.result[i].Date = gtDate + "-" + gtMonth + "-" + gtYear + " - " + gtHour + ":" + gtMin;

                        }
                    }
                    else {
                        $scope.reslength = true;
                    }
                    if ($scope.result.length > 0) {
                        $scope.reslength = false;
                        $scope.Count = $scope.result.length;
                        $scope.PublicPageDate = $scope.result[0].Date;
                        $scope.GMT = $scope.result[0].TimeZoneDetail.OffsetShort;
                    }
                    else {
                        $scope.reslength = true;
                    }

                });
            }
        });
        $scope.val = false;
        setMultilingualOptions();
        //$scope.cssshow = false;
        $anchorScroll.yOffset = 700;
        $scope.ImagePath = config.BUILD_URL;
        $scope.FrayteWebSite = config.SITE_COUNTRY;

    }

    init();

});
