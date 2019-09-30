angular.module('ngApp.warehouse').controller('WarehouseAddEditController', function ($scope, $state, $location, $stateParams, $translate, $filter, SessionService, WarehouseService, toaster, UserService) {

    //Set Multilingual for Modal Popup
    var setModalOptions = function () {
        $translate(['Warehouse', 'records', 'SuccessfullySavedInformation', 'PleaseCorrectValidationErrors', 'detail', 'FrayteError','FrayteValidation', 'ErrorGetting', 'FrayteInformation', 'SuccessfullyDelete', 'The', 'ErrorDeletingRecord']).then(function (translations) {
            
            $scope.TitleFrayteError = translations.FrayteError;
            $scope.TitleFrayteInformation = translations.FrayteInformation;
            $scope.TitleFrayteValidation = translations.FrayteValidation;            

            $scope.TextValidation = translations.PleaseCorrectValidationErrors;            
            $scope.TextSuccessfullySavedInformation = translations.SuccessfullySavedInformation;
            $scope.TextErrorSavingRecord = translations.ErrorSavingRecord;
            $scope.TextErrorGettingWarehouseRecord = translations.ErrorGetting + " " + translations.Warehouse + " " + translations.records;
        });
    };

    // Set Default WorkingStartTime and WorkingEndTime
    var setWorkingStartTime = function () {
        var h = "09";
        var m = "00";
        return h.toString() + m.toString();
    };
    var setWorkingEndTime = function () {
        var h = "17";
        var m = "00";
        return h.toString() + m.toString();
    };
    $scope.NewWarehouse = function () {
        $scope.warehouseDetail = {
            Email: '',
            TelephoneNo: '',
            MobileNo: '',
            Fax: '',
            Manager: {
                UserId: 0,
                ContactName: ''
            },
            Timezone: null,
            "WorkingWeekDay": null,
            WorkingStartTime: setWorkingStartTime(),
            WorkingEndTime: setWorkingEndTime(),
            LocationName: '',
            MapDetail: {
                latitude: $scope.details.geometry.viewport.O.O,
                longitude: $scope.details.geometry.viewport.j.j
            },
            Zoom: 8,
            LocationMapImage: '',
            MarkerDetail: {
                latitude: $scope.details.geometry.viewport.O.O,
                longitude: $scope.details.geometry.viewport.j.j
            },
            Country: null,
            Address: '',
            Address2: '',
            Address3: '',
            City: '',
            Suburb: '',
            State: '',
            Zip: ''
        };
    };

    $scope.EditLocation = function () {
        $scope.map.options = { scrollwheel: true };
        $scope.marker.options = { draggable: true };
    };

    $scope.SetLocation = function () {
        $scope.map.options = { scrollwheel: false };
        $scope.marker.options = { draggable: false };
    };

    //Google map
    $scope.SetMarkerValues = function (markerDetail) {
        if (markerDetail === undefined || markerDetail === null) {
            $scope.marker = {
                id: 0,
                coords: {
                    latitude: $scope.details.geometry.viewport.O.O,
                    longitude: $scope.details.geometry.viewport.j.j
                },
                options: { draggable: true },
                events: {
                    dragend: function (marker, eventName, args) {
                        $scope.marker.options = {
                            draggable: true,
                            labelContent: "lat: " + $scope.details.geometry.viewport.O.O + ' ' + 'lon: ' + $scope.details.geometry.viewport.j.j,
                            labelAnchor: "100 0",
                            labelClass: "marker-labels"
                        };
                    }
                }
            };
        }
        else {
            $scope.marker = {
                id: 0,
                coords: {
                    latitude: markerDetail.latitude,
                    longitude: markerDetail.longitude
                },
                options: { draggable: true },
                events: {
                    dragend: function (marker, eventName, args) {
                        $scope.marker.options = {
                            draggable: true,
                            labelContent: "lat: " + markerDetail.latitude + ' ' + 'lon: ' + markerDetail.longitude,
                            labelAnchor: "100 0",
                            labelClass: "marker-labels"
                        };
                    }
                }
            };
        }

    };

    $scope.GoBack = function () {
        $state.go('admin.warehouse');
    };

    $scope.submit = function (isValid, warehouseDetail) {
        if (isValid) {

            if (warehouseDetail.TelephoneNo !== undefined && warehouseDetail.TelephoneNo !== '') {
                warehouseDetail.TelephoneNo = '(+' + $scope.warehouseDetail.TelephoneCode.PhoneCode + ') ' + warehouseDetail.TelephoneNo;
            }

            if (warehouseDetail.MobileNo !== undefined && warehouseDetail.MobileNo !== '') {
                warehouseDetail.MobileNo = '(+' + $scope.warehouseDetail.MobileCode.PhoneCode + ') ' + warehouseDetail.MobileNo;
            }

            if (warehouseDetail.Fax !== undefined && warehouseDetail.Fax !== '') {
                warehouseDetail.Fax = '(+' + $scope.warehouseDetail.FaxCode.PhoneCode + ') ' + warehouseDetail.Fax;
            }

            //Get Google Map Image and pass it to Server
            html2canvas(document.getElementById("dvWarehouseMap"), {
                useCORS: true,
                onrendered: function (canvas) {
                    var img = canvas.toDataURL("image/png");
                    img = img.replace('data:image/png;base64,', '');
                    var finalImageSrc = 'data:image/png;base64,' + img;
                    $('#googlemapbinary').attr('src', finalImageSrc);

                    warehouseDetail.LocationMapImage = img;

                    WarehouseService.SaveWarehouse(warehouseDetail).then(function (response) {
                        if (response.data.WarehouseId > 0) {
                            toaster.pop({
                                type: 'success',
                                title: $scope.TitleFrayteInformation,
                                body: $scope.TextSuccessfullySavedInformation,
                                showCloseButton: true
                            });
                            $scope.GoBack();
                        }
                    }, function () {
                        toaster.pop({
                            type: 'error',
                            title: $scope.TitleFrayteError,
                            body: $scope.TextErrorSavingRecord,
                            showCloseButton: true
                        });
                    });
                }
            });
        }
        else {
            toaster.pop({
                type: 'warning',
                title: $scope.TitleFrayteValidation,
                body: $scope.TextValidation,
                showCloseButton: true
            });
        }
    };

    $scope.GetWarehouseDetailsInitials = function () {
        UserService.GetInitials().then(function (response) {
            $scope.WorkingWeekDays = response.data.WorkingWeekDays;
            $scope.countries = response.data.Countries;
            $scope.timezones = response.data.TimeZones;
            $scope.countryPhoneCodes = response.data.CountryPhoneCodes;
            //After successfull initials get the warehouse detail
            if ($scope.warehouseId === undefined || $scope.warehouseId === null || $scope.warehouseId === "0") {
                $scope.SetMarkerValues();
                //Step 1: Set warehouse record
                $scope.NewWarehouse();
                // Set Default WorkingWeek Day
                if ($scope.WorkingWeekDays !== null && $scope.WorkingWeekDays !== undefined && $scope.WorkingWeekDays.length > 0) {
                    var weekDays = $scope.WorkingWeekDays;
                    for (var n = 0; n < weekDays.length; n++) {
                        if (weekDays[n].IsDefault) {
                            $scope.warehouseDetail.WorkingWeekDay = weekDays[n];
                            break;
                        }
                    }
                }
            }
            else {
                //Get warehouse details 
                WarehouseService.GetWarehouseDetail($scope.warehouseId).then(function (response) {
                    //Step 1: Get warehouse record
                    $scope.warehouseDetail = response.data;

                    $scope.SetPhoneCodeAndNumber('TelephoneNo', $scope.warehouseDetail.TelephoneNo);
                    $scope.SetPhoneCodeAndNumber('MobileNo', $scope.warehouseDetail.MobileNo);
                    $scope.SetPhoneCodeAndNumber('Fax', $scope.warehouseDetail.Fax);

                    $scope.SetMarkerValues($scope.warehouseDetail.MarkerDetail);

                    // set state and zip
                    if ($scope.warehouseDetail && $scope.warehouseDetail.Country != null && $scope.warehouseDetail.Country.Code === "HKG") {
                        $scope.setStateDisable = true;
                        $scope.setZipDisable = true;
                        $scope.warehouseDetail.Zip= null;
                        $scope.warehouseDetail.State = null;
                    }
                    if ($scope.warehouseDetail && $scope.warehouseDetail.Country != null && $scope.warehouseDetail.Country.Code === "GBR") {
                        $scope.setStateDisable = true;
                        $scope.warehouseDetail.State = null;
                    }

                }, function () {
                    toaster.pop({
                        type: 'error',
                        title: $scope.TitleFrayteError,
                        body: $scope.TextErrorGettingWarehouseRecord,
                        showCloseButton: true
                    });
                });
            }
        });
    };

    //This will hide Frayteuser Panel by default.    
    $scope.ShowHideFrayteuserPanel = function () {
        $scope.ShowFrayteuserPanel = !$scope.ShowFrayteuserPanel;
    };

    $scope.toggleShow = function () {
        $scope.show = !$scope.show;
    };

    $scope.GetUsers = function (query) {
        return UserService.GetAssociatedUsers(query).then(function (response) {
            return response.data;
        });
    };

    $scope.SetPhoneCodeAndNumber = function (userType, telephoneNumber) {
        if (telephoneNumber !== undefined && telephoneNumber !== null && telephoneNumber !== '') {
            var n = telephoneNumber.indexOf(")");
            var code = telephoneNumber.substring(0, n + 1);

            if (userType === 'TelephoneNo') {
                $scope.warehouseDetail.TelephoneNo = telephoneNumber.replace(code, "").trim();
            }
            else if (userType === 'MobileNo') {
                $scope.warehouseDetail.MobileNo = telephoneNumber.replace(code, "").trim();
            }
            else if (userType === 'Fax') {
                $scope.warehouseDetail.Fax = telephoneNumber.replace(code, "").trim();
            }

            var countryCode = telephoneNumber.substring(2, n);
            var objects = $scope.countryPhoneCodes;

            for (var i = 0; i < objects.length; i++) {
                if (objects[i].PhoneCode === countryCode) {
                    if (userType === 'TelephoneNo') {
                        $scope.warehouseDetail.TelephoneCode = objects[i];
                    }
                    else if (userType === 'MobileNo') {
                        $scope.warehouseDetail.MobileCode = objects[i];
                    }
                    else if (userType === 'Fax') {
                        $scope.warehouseDetail.FaxCode = objects[i];
                    }
                    break;
                }
            }
        }
    };

    $scope.InitailDetailOfCountry = function (data, showData, countries) {
        if (!data) {
            $scope.warehouseDetail.FaxCode = showData;
            $scope.warehouseDetail.TelephoneCode = showData;
            $scope.warehouseDetail.MobileCode = showData;
            for (var i = 0; i < countries.length; i++) {
                if (countries[i].Name == showData.Name) {
                    $scope.warehouseDetail.Country = countries[i];
                    break;
                }
            }
            $scope.data = true;
        }
    };
    $scope.InitailDetailOfCountryCodes = function (data, countryPhoneFaxCode, country) {
        // Set Customer State and zip
        if (country.Code !== null && country.Code !== '' && country.Code !== undefined) {
            if (country.Code === "HKG") {
                $scope.setStateDisable = true;
                $scope.setZipDisable = true;
                $scope.warehouseDetail.Zip = null;
                $scope.warehouseDetail.State = null;
            }
            else if (country.Code === "GBR") {
                $scope.setStateDisable = true;
                $scope.setZipDisable = false;
                $scope.warehouseDetail.State = null;
            }
            else {
                $scope.setZipDisable = false;
                $scope.setStateDisable = false;
            }
        }
        if (!data) {
            for (var i = 0; i < countryPhoneFaxCode.length; i++) {
                if (countryPhoneFaxCode[i].Name == country.Name) {
                    $scope.warehouseDetail.FaxCode = countryPhoneFaxCode[i];
                    $scope.warehouseDetail.TelephoneCode = countryPhoneFaxCode[i];
                    $scope.warehouseDetail.MobileCode = countryPhoneFaxCode[i];
                    break;
                }
            }
            $scope.data = true;
        }
    };
    // Set State And Zip for "HKG" and "GBR"
    $scope.setStateAndZip = function (Code, stateZip) {
        if (Code !== null && Code !== '' && Code !== undefined) {
            if (Code === "HKG" && (stateZip === 'zip' || stateZip === 'state')) {
                return false;
            }
            else if (Code === "GBR" && stateZip === 'state') {
                return false;
            }
            else {
                return true;
            }
        }
        else {
            return true;
        }
    };

    function init() {
        // set Multilingual Modal Popup Options
        setModalOptions();

        $scope.result = '';
        $scope.details = {
            "geometry": {
                "viewport":
                    {
                        "O": { "O": 22.45640823181909, "j": 22.45640823181909 },
                        "j": { "j": 114.11009788513184, "O": 114.11009788513184 }
                    }
            }
        };
        //Need the propery to freez the mouse scrool
        $scope.map = {
            options: { scrollwheel: true }
        };
        $scope.warehouseId = $stateParams.warehouseId;
        $scope.GetWarehouseDetailsInitials();
        $scope.ShowFrayteuserPanel = true;
        $scope.data = false;
    }
    init();

});

