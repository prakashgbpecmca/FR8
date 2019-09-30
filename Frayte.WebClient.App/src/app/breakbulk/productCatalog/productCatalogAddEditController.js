angular.module('ngApp.breakBulk').controller("ProductCatalogAddEditController", function ($scope, $translate, $uibModal, ModalService, config, CustomerId, HubDetail, BreakBulkService, toaster, $uibModalInstance, TopCurrencyService, ProductcatalogId, AppSpinner) {

    $scope.ShowCustomManifestFields = function (DestinationHub) {
        if (DestinationHub !== undefined && DestinationHub !== null && DestinationHub !== '') {
            $scope.HubCode = DestinationHub.Code;
            $scope.HubId = DestinationHub.HubId;
        }
    };

    //Set Multilingual for Modal Popup
    var setMultilingualOptions = function () {
        $translate(['FrayteError', 'FrayteWarning', 'FrayteSuccess', 'Confirmation', 'SomeErrorOccuredTryAgain', 'InitialDataValidation', 'SelectCustomerAddressBookValidation',
                    'BookingSave_Validation', 'SelectCustomer_Validation', 'Select_Address_Form_First', 'Select_Address_To_First', 'Fill_Package_Information', 'SelectCustomer_Validation',
                    'SelectCurrency_Validation', 'Sure_Clear_The_Form', 'SomeErrorOccuredTryAgain', 'Enter_Valid_AWB_Number_First', 'Select_From_Contry_Save_Draft', 'Select_To_Contry_Save_Draft',
                    'Fill_AWB_To_Save_Shipment_Draft', 'Error_While_Placing_Booking_Try_Again', 'While_We_Placing_Booking', 'CorrectValidationErrorFirst', 'Please_Select_Hub', 'Product_Catalog_Save_Success',
                    'Enter_Product_Description', 'Loading_Product_Catalog', 'Saving_Product_Catalog']).then(function (translations) {
                        $scope.FrayteError = translations.FrayteError;
                        $scope.FrayteWarning = translations.FrayteWarning;
                        $scope.FrayteSuccess = translations.FrayteSuccess;
                        $scope.Confirmation = translations.Confirmation;
                        $scope.SomeErrorOccuredTryAgain = translations.SomeErrorOccuredTryAgain;
                        $scope.InitialDataValidation = translations.InitialDataValidation;
                        $scope.SelectCustomerAddressBookValidation = translations.SelectCustomerAddressBook_Validation;
                        $scope.BookingSaveValidation = translations.BookingSave_Validation;
                        $scope.SelectCustomer_Validation = translations.SelectCustomer_Validation;
                        $scope.Select_Address_Form_First = translations.Select_Address_Form_First;
                        $scope.Select_Address_To_First = translations.Select_Address_To_First;
                        $scope.Fill_Package_Information = translations.Fill_Package_Information;
                        $scope.SelectCustomerValidation = translations.SelectCustomer_Validation;
                        $scope.SelectCurrencyValidation = translations.SelectCurrency_Validation;
                        $scope.Sure_Clear_The_Form = translations.Sure_Clear_The_Form;
                        $scope.SomeErrorOccuredTryAgain = translations.SomeErrorOccuredTryAgain;
                        $scope.Enter_Valid_AWB_Number_First = translations.Enter_Valid_AWB_Number_First;
                        $scope.Select_From_Contry_Save_Draft = translations.Select_From_Contry_Save_Draft;
                        $scope.Select_To_Contry_Save_Draft = translations.Select_To_Contry_Save_Draft;
                        $scope.Fill_AWB_To_Save_Shipment_Draft = translations.Fill_AWB_To_Save_Shipment_Draft;
                        $scope.Error_While_Placing_Booking_Try_Again = translations.Error_While_Placing_Booking_Try_Again;
                        $scope.While_We_Placing_Booking = translations.While_We_Placing_Booking;
                        $scope.CorrectValidationErrorFirst = translations.CorrectValidationErrorFirst;
                        $scope.SelectHub = translations.Please_Select_Hub;
                        $scope.ProductCatalog_SaveSuccess = translations.Product_Catalog_Save_Success;
                        $scope.Enter_Product_Description = translations.Enter_Product_Description;
                        $scope.Loading_Product_Catalog = translations.Loading_Product_Catalog;
                        $scope.Saving_Product_Catalog = translations.Saving_Product_Catalog;
                    });
    };

    $scope.CanadaProductCatalog = {
        ProductCatalogId: 0,
        CustomerId: 0,
        FactoryUserId: 0,
        HubId: 0,
        Length: 0.0,
        Width: 0.0,
        Height: 0.0,
        Weight: 0.0,
        DeclaredValue: 0.0,
        ProductDescription: null,
        Reference: null,
        InternalAccountnumber: null,
        WeightUOM: null,
        TotalValue: null,
        Currency: {
            CurrencyCode: '',
            Description: ''
        },
        ItemHSCode: null,
        Pieces: null,
        SKU: null,
        ProvinceCode: null
    };

    $scope.SwissProductCatalog = {
        ProductCatalogId: 0,
        CustomerId: 0,
        FactoryUserId: 0,
        HubId: 0,
        Length: 0.0,
        Width: 0.0,
        Height: 0.0,
        Weight: 0.0,
        DeclaredValue: 0.0,
        ProductDescription: null,
        ReferenceInvoiceNR: null,
        Currency: {
            CurrencyCode: '',
            Description: ''
        },
        VersendLand: null,
        Anzahl: null,
        Netto: null,
        Brutto: null,
        TarifCode: null,
        Value: null,
        OriginCountry: null,
        PacType: null,
        PacQty: null,
        ZeichenPackstucke: null,
        ArtVorpapier: null,
        ZeichenVorpapier: null
    };

    $scope.USAProductCatalog = {
        ProductCatalogId: 0,
        CustomerId: 0,
        FactoryUserId: 0,
        HubId: 0,
        Length: 0.0,
        Width: 0.0,
        Height: 0.0,
        Weight: 0.0,
        DeclaredValue: 0.0,
        ProductDescription: null,
        Reference: null,
        InternalAccountnumber: null,
        WeightUOM: null,
        TotalValue: null,
        ItemHSCode: null,
        CustomEntryType: null,
        CustomTotalValue: null,
        CustomTotalVAT: null,
        CustomDuty: null,
        Pieces: null,
        ItemValue: null,
        CustomCommodityMap: null,
        ECommerceShipmentId: null
    };

    $scope.UKProductCatalog = {
        ProductCatalogId: 0,
        CustomerId: 0,
        FactoryUserId: 0,
        HubId: 0,
        Length: 0.0,
        Width: 0.0,
        Height: 0.0,
        Weight: 0.0,
        DeclaredValue: 0.0,
        ProductDescription: null,
        Vat: null,
        Pce: null,
        Number: null,
        TCode: null,
        Currency: {
            CurrencyCode: '',
            Description: ''
        },
        Value: null,
        Weblink: null
    };

    $scope.JapanProductCatalog = {
        ProductCatalogId: 0,
        CustomerId: 0,
        FactoryUserId: 0,
        HubId: 0,
        Length: 0.0,
        Width: 0.0,
        Height: 0.0,
        Weight: 0.0,
        DeclaredValue: 0.0,
        ProductDescription: null,
        Number: null,
        Code: null,
        Currency: {
            CurrencyCode: '',
            Description: ''
        },
        DeclaredValueOfCustom: null,
        Origin: null,
        OriginCode: null
    };

    $scope.productcatalog = {
        ProductCatalogId: 0,
        CustomerId: 0,
        DestinationHub: {},
        Length: '',
        Width: '',
        Height: '',
        Weight: '',
        DeclareValue: '',
        ProductDescription: ''
    };

    $scope.SaveProductCatalog = function (DestinationHub) {
        AppSpinner.showSpinnerTemplate($scope.Saving_Product_Catalog, $scope.Template);
        if (DestinationHub !== undefined && DestinationHub !== null && DestinationHub !== '') {
            if (DestinationHub.Code === 'YVR' || DestinationHub.Code === 'YYZ') {
                if ($scope.productcatalog !== undefined && $scope.productcatalog != null && $scope.productcatalog !== '') {
                    $scope.CanadaProductCatalog.ProductCatalogId = $scope.productcatalog.ProductCatalogId;
                    $scope.CanadaProductCatalog.CustomerId = $scope.productcatalog.CustomerId;
                    $scope.CanadaProductCatalog.HubId = $scope.productcatalog.DestinationHub.HubId;
                    $scope.CanadaProductCatalog.Length = $scope.productcatalog.Length;
                    $scope.CanadaProductCatalog.Width = $scope.productcatalog.Width;
                    $scope.CanadaProductCatalog.Height = $scope.productcatalog.Height;
                    $scope.CanadaProductCatalog.Weight = $scope.productcatalog.Weight;
                    $scope.CanadaProductCatalog.DeclaredValue = $scope.productcatalog.DeclareValue;
                    $scope.CanadaProductCatalog.ProductDescription = $scope.productcatalog.ProductDescription;
                    BreakBulkService.Canadaproductcatalog($scope.CanadaProductCatalog).then(function (response) {
                        if (response.data !== undefined && response.data !== null && response.data !== '' && response.data === true) {
                            toaster.pop({
                                type: 'success',
                                title: $scope.FrayteSuccess,
                                body: $scope.ProductCatalog_SaveSuccess,
                                showCloseButton: true
                            });
                            $uibModalInstance.close(true);
                            AppSpinner.hideSpinnerTemplate();
                        }
                        else {
                            toaster.pop({
                                type: 'error',
                                title: $scope.FrayteError,
                                body: $scope.SomeErrorOccuredTryAgain,
                                showCloseButton: true
                            });
                            AppSpinner.hideSpinnerTemplate();
                        }
                    });
                }
                else {
                    toaster.pop({
                        type: 'warning',
                        title: $scope.FrayteWarning,
                        body: $scope.Enter_Product_Description,
                        showCloseButton: true
                    });
                    AppSpinner.hideSpinnerTemplate();
                }
            }
            else if (DestinationHub.Code === 'ZRH') {
                if ($scope.productcatalog !== undefined && $scope.productcatalog != null && $scope.productcatalog !== '') {
                    $scope.SwissProductCatalog.ProductCatalogId = $scope.productcatalog.ProductCatalogId;
                    $scope.SwissProductCatalog.CustomerId = $scope.productcatalog.CustomerId;
                    $scope.SwissProductCatalog.HubId = $scope.productcatalog.DestinationHub.HubId;
                    $scope.SwissProductCatalog.Length = $scope.productcatalog.Length;
                    $scope.SwissProductCatalog.Width = $scope.productcatalog.Width;
                    $scope.SwissProductCatalog.Height = $scope.productcatalog.Height;
                    $scope.SwissProductCatalog.Weight = $scope.productcatalog.Weight;
                    $scope.SwissProductCatalog.DeclaredValue = $scope.productcatalog.DeclareValue;
                    $scope.SwissProductCatalog.ProductDescription = $scope.productcatalog.ProductDescription;
                    BreakBulkService.Swissaproductcatalog($scope.SwissProductCatalog).then(function (response) {
                        if (response.data !== undefined && response.data !== null && response.data !== '' && response.data === true) {
                            toaster.pop({
                                type: 'success',
                                title: $scope.FrayteSuccess,
                                body: $scope.ProductCatalog_SaveSuccess,
                                showCloseButton: true
                            });
                            $uibModalInstance.close(true);
                            AppSpinner.hideSpinnerTemplate();
                        }
                        else {
                            toaster.pop({
                                type: 'error',
                                title: $scope.FrayteError,
                                body: $scope.SomeErrorOccuredTryAgain,
                                showCloseButton: true
                            });
                            AppSpinner.hideSpinnerTemplate();
                        }
                    });
                }
                else {
                    toaster.pop({
                        type: 'warning',
                        title: $scope.FrayteWarning,
                        body: $scope.Enter_Product_Description,
                        showCloseButton: true
                    });
                    AppSpinner.hideSpinnerTemplate();
                }
            }
            else if (DestinationHub.Code === 'JFK' || DestinationHub.Code === 'SFO' || DestinationHub.Code === 'ORD') {
                if ($scope.productcatalog !== undefined && $scope.productcatalog != null && $scope.productcatalog !== '') {
                    $scope.USAProductCatalog.ProductCatalogId = $scope.productcatalog.ProductCatalogId;
                    $scope.USAProductCatalog.CustomerId = $scope.productcatalog.CustomerId;
                    $scope.USAProductCatalog.HubId = $scope.productcatalog.DestinationHub.HubId;
                    $scope.USAProductCatalog.Length = $scope.productcatalog.Length;
                    $scope.USAProductCatalog.Width = $scope.productcatalog.Width;
                    $scope.USAProductCatalog.Height = $scope.productcatalog.Height;
                    $scope.USAProductCatalog.Weight = $scope.productcatalog.Weight;
                    $scope.USAProductCatalog.DeclaredValue = $scope.productcatalog.DeclareValue;
                    $scope.USAProductCatalog.ProductDescription = $scope.productcatalog.ProductDescription;
                    $scope.USAProductCatalog.customCommodityMap = $scope.productcatalog.USAProductCatalog;
                    BreakBulkService.USAproductcatalog($scope.USAProductCatalog).then(function (response) {
                        if (response.data !== undefined && response.data !== null && response.data !== '' && response.data === true) {
                            toaster.pop({
                                type: 'success',
                                title: $scope.FrayteSuccess,
                                body: $scope.ProductCatalog_SaveSuccess,
                                showCloseButton: true
                            });
                            $uibModalInstance.close(true);
                            AppSpinner.hideSpinnerTemplate();
                        }
                        else {
                            toaster.pop({
                                type: 'error',
                                title: $scope.FrayteError,
                                body: $scope.SomeErrorOccuredTryAgain,
                                showCloseButton: true
                            });
                            AppSpinner.hideSpinnerTemplate();
                        }
                    });
                }
                else {
                    toaster.pop({
                        type: 'warning',
                        title: $scope.FrayteWarning,
                        body: $scope.Enter_Product_Description,
                        showCloseButton: true
                    });
                    AppSpinner.hideSpinnerTemplate();
                }
            }
            else if (DestinationHub.Code === 'LHR') {
                if ($scope.productcatalog !== undefined && $scope.productcatalog != null && $scope.productcatalog !== '') {
                    $scope.UKProductCatalog.ProductCatalogId = $scope.productcatalog.ProductCatalogId;
                    $scope.UKProductCatalog.CustomerId = $scope.productcatalog.CustomerId;
                    $scope.UKProductCatalog.HubId = $scope.productcatalog.DestinationHub.HubId;
                    $scope.UKProductCatalog.Length = $scope.productcatalog.Length;
                    $scope.UKProductCatalog.Width = $scope.productcatalog.Width;
                    $scope.UKProductCatalog.Height = $scope.productcatalog.Height;
                    $scope.UKProductCatalog.Weight = $scope.productcatalog.Weight;
                    $scope.UKProductCatalog.DeclaredValue = $scope.productcatalog.DeclareValue;
                    $scope.UKProductCatalog.ProductDescription = $scope.productcatalog.ProductDescription;
                    BreakBulkService.UKproductcatalog($scope.UKProductCatalog).then(function (response) {
                        if (response.data !== undefined && response.data !== null && response.data !== '' && response.data === true) {
                            toaster.pop({
                                type: 'success',
                                title: $scope.FrayteSuccess,
                                body: $scope.ProductCatalog_SaveSuccess,
                                showCloseButton: true
                            });
                            $uibModalInstance.close(true);
                            AppSpinner.hideSpinnerTemplate();
                        }
                        else {
                            toaster.pop({
                                type: 'error',
                                title: $scope.FrayteError,
                                body: $scope.SomeErrorOccuredTryAgain,
                                showCloseButton: true
                            });
                            AppSpinner.hideSpinnerTemplate();
                        }
                    });
                }
                else {
                    toaster.pop({
                        type: 'warning',
                        title: $scope.FrayteWarning,
                        body: $scope.Enter_Product_Description,
                        showCloseButton: true
                    });
                    AppSpinner.hideSpinnerTemplate();
                }
            }
            else if (DestinationHub.Code === 'NRT') {
                if ($scope.productcatalog !== undefined && $scope.productcatalog != null && $scope.productcatalog !== '') {
                    $scope.JapanProductCatalog.ProductCatalogId = $scope.productcatalog.ProductCatalogId;
                    $scope.JapanProductCatalog.CustomerId = $scope.productcatalog.CustomerId;
                    $scope.JapanProductCatalog.HubId = $scope.productcatalog.DestinationHub.HubId;
                    $scope.JapanProductCatalog.Length = $scope.productcatalog.Length;
                    $scope.JapanProductCatalog.Width = $scope.productcatalog.Width;
                    $scope.JapanProductCatalog.Height = $scope.productcatalog.Height;
                    $scope.JapanProductCatalog.Weight = $scope.productcatalog.Weight;
                    $scope.JapanProductCatalog.DeclaredValue = $scope.productcatalog.DeclareValue;
                    $scope.JapanProductCatalog.ProductDescription = $scope.productcatalog.ProductDescription;
                    BreakBulkService.Japanproductcatalog($scope.JapanProductCatalog).then(function (response) {
                        if (response.data !== undefined && response.data !== null && response.data !== '' && response.data === true) {
                            toaster.pop({
                                type: 'success',
                                title: $scope.FrayteSuccess,
                                body: $scope.ProductCatalog_SaveSuccess,
                                showCloseButton: true
                            });
                            $uibModalInstance.close(true);
                            AppSpinner.hideSpinnerTemplate();
                        }
                        else {
                            toaster.pop({
                                type: 'error',
                                title: $scope.FrayteError,
                                body: $scope.SomeErrorOccuredTryAgain,
                                showCloseButton: true
                            });
                            AppSpinner.hideSpinnerTemplate();
                        }
                    });
                }
                else {
                    toaster.pop({
                        type: 'warning',
                        title: $scope.FrayteWarning,
                        body: $scope.Enter_Product_Description,
                        showCloseButton: true
                    });
                    AppSpinner.hideSpinnerTemplate();
                }
            }
            else if (DestinationHub.Code === 'OSL') {
                if ($scope.productcatalog !== undefined && $scope.productcatalog != null && $scope.productcatalog !== '') {
                    $scope.UKProductCatalog.ProductCatalogId = $scope.productcatalog.ProductCatalogId;
                    $scope.UKProductCatalog.CustomerId = $scope.productcatalog.CustomerId;
                    $scope.UKProductCatalog.HubId = $scope.productcatalog.DestinationHub.HubId;
                    $scope.UKProductCatalog.Length = $scope.productcatalog.Length;
                    $scope.UKProductCatalog.Width = $scope.productcatalog.Width;
                    $scope.UKProductCatalog.Height = $scope.productcatalog.Height;
                    $scope.UKProductCatalog.Weight = $scope.productcatalog.Weight;
                    $scope.UKProductCatalog.DeclaredValue = $scope.productcatalog.DeclareValue;
                    $scope.UKProductCatalog.ProductDescription = $scope.productcatalog.ProductDescription;
                    BreakBulkService.Norwayproductcatalog($scope.UKProductCatalog).then(function (response) {
                        if (response.data !== undefined && response.data !== null && response.data !== '' && response.data === true) {
                            toaster.pop({
                                type: 'success',
                                title: $scope.FrayteSuccess,
                                body: $scope.ProductCatalog_SaveSuccess,
                                showCloseButton: true
                            });
                            $uibModalInstance.close(true);
                            AppSpinner.hideSpinnerTemplate();
                        }
                        else {
                            toaster.pop({
                                type: 'error',
                                title: $scope.FrayteError,
                                body: $scope.SomeErrorOccuredTryAgain,
                                showCloseButton: true
                            });
                            AppSpinner.hideSpinnerTemplate();
                        }
                    });
                }
                else {
                    toaster.pop({
                        type: 'warning',
                        title: $scope.FrayteWarning,
                        body: $scope.Enter_Product_Description,
                        showCloseButton: true
                    });
                    AppSpinner.hideSpinnerTemplate();
                }
            }
            else if (DestinationHub.Code === 'SIN') {
                if ($scope.productcatalog !== undefined && $scope.productcatalog != null && $scope.productcatalog !== '') {
                    $scope.UKProductCatalog.ProductCatalogId = $scope.productcatalog.ProductCatalogId;
                    $scope.UKProductCatalog.CustomerId = $scope.productcatalog.CustomerId;
                    $scope.UKProductCatalog.HubId = $scope.productcatalog.DestinationHub.HubId;
                    $scope.UKProductCatalog.Length = $scope.productcatalog.Length;
                    $scope.UKProductCatalog.Width = $scope.productcatalog.Width;
                    $scope.UKProductCatalog.Height = $scope.productcatalog.Height;
                    $scope.UKProductCatalog.Weight = $scope.productcatalog.Weight;
                    $scope.UKProductCatalog.DeclaredValue = $scope.productcatalog.DeclareValue;
                    $scope.UKProductCatalog.ProductDescription = $scope.productcatalog.ProductDescription;
                    BreakBulkService.Singaporeproductcatalog($scope.UKProductCatalog).then(function (response) {
                        if (response.data !== undefined && response.data !== null && response.data !== '' && response.data === true) {
                            toaster.pop({
                                type: 'success',
                                title: $scope.FrayteSuccess,
                                body: $scope.ProductCatalog_SaveSuccess,
                                showCloseButton: true
                            });
                            $uibModalInstance.close(true);
                            AppSpinner.hideSpinnerTemplate();
                        }
                        else {
                            toaster.pop({
                                type: 'error',
                                title: $scope.FrayteError,
                                body: $scope.SomeErrorOccuredTryAgain,
                                showCloseButton: true
                            });
                            AppSpinner.hideSpinnerTemplate();
                        }
                    });
                }
                else {
                    toaster.pop({
                        type: 'warning',
                        title: $scope.FrayteWarning,
                        body: $scope.Enter_Product_Description,
                        showCloseButton: true
                    });
                    AppSpinner.hideSpinnerTemplate();
                }
            }
            else {
                toaster.pop({
                    type: 'warning',
                    title: $scope.FrayteWarning,
                    body: $scope.SelectHub,
                    showCloseButton: true
                });
            }
        }
    };

    var editproductcatalog = function (ProductcatalogId) {
        cuurency();
        setMultilingualOptions();
        if (ProductcatalogId !== undefined && ProductcatalogId !== null && ProductcatalogId !== '' && ProductcatalogId > 0) {
            AppSpinner.showSpinnerTemplate($scope.Loading_Product_Catalog, $scope.Template);
            BreakBulkService.editCatalog(ProductcatalogId).then(function (response) {
                if (response.data !== undefined && response.data !== null && response.data !== '') {
                    if (response.data.m_Rest.m_Item1 !== undefined && response.data.m_Rest.m_Item1 !== null && response.data.m_Rest.m_Item1 !== '') {
                        var sp = response.data.m_Rest.m_Item1.split(";");
                        if (sp !== undefined && sp !== null && sp !== '' && sp.length > 0) {
                            if (sp[0] === 'YVR' || sp[0] === 'YYZ') {
                                $scope.productcatalog.Length = response.data.m_Item7.Length;
                                $scope.productcatalog.Width = response.data.m_Item7.Width;
                                $scope.productcatalog.Height = response.data.m_Item7.Height;
                                $scope.productcatalog.Weight = response.data.m_Item7.Weight;
                                $scope.productcatalog.DeclareValue = response.data.m_Item7.DeclaredValue;
                                $scope.productcatalog.ProductDescription = response.data.m_Item7.ProductDescription;

                                if (response.data.m_Item7 !== undefined && response.data.m_Item7 !== null && response.data.m_Item7 !== '') {

                                    $scope.CanadaProductCatalog = response.data.m_Item7;

                                    if ($scope.CurrencyTypes !== undefined && $scope.CurrencyTypes !== null && $scope.CurrencyTypes !== '' && $scope.CurrencyTypes.length > 0 && response.data.m_Item7.Currency !== undefined && response.data.m_Item7.Currency !== null && response.data.m_Item7.Currency !== '') {
                                        for (i = 0; i < $scope.CurrencyTypes.length; i++) {
                                            if ($scope.CurrencyTypes[i].CurrencyCode === response.data.m_Item7.Currency.CurrencyCode) {
                                                $scope.CanadaProductCatalog.Currency = $scope.CurrencyTypes[i];
                                            }
                                        }
                                    }

                                    if ($scope.Units !== undefined && $scope.Units !== null && $scope.Units !== '' && response.data.m_Item7.WeightUOM !== undefined && response.data.m_Item7.WeightUOM !== null && response.data.m_Item7.WeightUOM !== '') {
                                        for (j = 0; j < $scope.Units.length; j++) {
                                            if ($scope.Units[j].UnitValue === response.data.m_Item7.WeightUOM.UnitValue) {
                                                $scope.CanadaProductCatalog.WeightUOM = $scope.Units[j];
                                            }
                                        }
                                    }
                                }
                                AppSpinner.hideSpinnerTemplate();
                            }
                            else if (sp[0] === 'ZRH') {
                                $scope.productcatalog.Length = response.data.m_Item4.Length;
                                $scope.productcatalog.Width = response.data.m_Item4.Width;
                                $scope.productcatalog.Height = response.data.m_Item4.Height;
                                $scope.productcatalog.Weight = response.data.m_Item4.Weight;
                                $scope.productcatalog.DeclareValue = response.data.m_Item4.DeclaredValue;
                                $scope.productcatalog.ProductDescription = response.data.m_Item4.ProductDescription;

                                if (response.data.m_Item4 !== undefined && response.data.m_Item4 !== null && response.data.m_Item4 !== '') {

                                    $scope.SwissProductCatalog = response.data.m_Item4;

                                    if ($scope.CurrencyTypes !== undefined && $scope.CurrencyTypes !== null && $scope.CurrencyTypes !== '' && $scope.CurrencyTypes.length > 0 && response.data.m_Item4.Currency !== undefined && response.data.m_Item4.Currency !== null && response.data.m_Item4.Currency !== '') {
                                        for (i = 0; i < $scope.CurrencyTypes.length; i++) {
                                            if ($scope.CurrencyTypes[i].CurrencyCode === response.data.m_Item4.Currency.CurrencyCode) {
                                                $scope.SwissProductCatalog.Currency = $scope.CurrencyTypes[i];
                                            }
                                        }
                                    }

                                    if ($scope.Units !== undefined && $scope.Units !== null && $scope.Units !== '' && response.data.m_Item4.WeightUOM !== undefined && response.data.m_Item4.WeightUOM !== null && response.data.m_Item4.WeightUOM !== '') {
                                        for (j = 0; j < $scope.Units.length; j++) {
                                            if ($scope.Units[j].UnitValue === response.data.m_Item4.WeightUOM.UnitValue) {
                                                $scope.SwissProductCatalog.WeightUOM = $scope.Units[j];
                                            }
                                        }
                                    }
                                }
                                AppSpinner.hideSpinnerTemplate();
                            }
                            else if (sp[0] === 'JFK' || sp[0] === 'SFO' || sp[0] === 'ORD') {
                                $scope.productcatalog.Length = response.data.m_Item2.Length;
                                $scope.productcatalog.Width = response.data.m_Item2.Width;
                                $scope.productcatalog.Height = response.data.m_Item2.Height;
                                $scope.productcatalog.Weight = response.data.m_Item2.Weight;
                                $scope.productcatalog.DeclareValue = response.data.m_Item2.DeclaredValue;
                                $scope.productcatalog.ProductDescription = response.data.m_Item2.ProductDescription;

                                if (response.data.m_Item2 !== undefined && response.data.m_Item2 !== null && response.data.m_Item2 !== '') {

                                    $scope.USAProductCatalog = response.data.m_Item2;

                                    if ($scope.CurrencyTypes !== undefined && $scope.CurrencyTypes !== null && $scope.CurrencyTypes !== '' && $scope.CurrencyTypes.length > 0 && response.data.m_Item2.Currency !== undefined && response.data.m_Item2.Currency !== null && response.data.m_Item2.Currency !== '') {
                                        for (i = 0; i < $scope.CurrencyTypes.length; i++) {
                                            if ($scope.CurrencyTypes[i].CurrencyCode === response.data.m_Item2.Currency.CurrencyCode) {
                                                $scope.USAProductCatalog.Currency = $scope.CurrencyTypes[i];
                                            }
                                        }
                                    }

                                    if ($scope.Units !== undefined && $scope.Units !== null && $scope.Units !== '' && response.data.m_Item2.WeightUOM !== undefined && response.data.m_Item2.WeightUOM !== null && response.data.m_Item2.WeightUOM !== '') {
                                        for (j = 0; j < $scope.Units.length; j++) {
                                            if ($scope.Units[j].UnitValue === response.data.m_Item2.WeightUOM.UnitValue) {
                                                $scope.USAProductCatalog.WeightUOM = $scope.Units[j];
                                            }
                                        }
                                    }
                                }
                                AppSpinner.hideSpinnerTemplate();
                            }
                            else if (sp[0] === 'LHR') {
                                $scope.productcatalog.Length = response.data.m_Item1.Length;
                                $scope.productcatalog.Width = response.data.m_Item1.Width;
                                $scope.productcatalog.Height = response.data.m_Item1.Height;
                                $scope.productcatalog.Weight = response.data.m_Item1.Weight;
                                $scope.productcatalog.DeclareValue = response.data.m_Item1.DeclaredValue;
                                $scope.productcatalog.ProductDescription = response.data.m_Item1.ProductDescription;

                                if (response.data.m_Item1 !== undefined && response.data.m_Item1 !== null && response.data.m_Item1 !== '') {

                                    $scope.UKProductCatalog = response.data.m_Item1;

                                    if ($scope.CurrencyTypes !== undefined && $scope.CurrencyTypes !== null && $scope.CurrencyTypes !== '' && $scope.CurrencyTypes.length > 0 && response.data.m_Item1.Currency !== undefined && response.data.m_Item1.Currency !== null && response.data.m_Item1.Currency !== '') {
                                        for (i = 0; i < $scope.CurrencyTypes.length; i++) {
                                            if ($scope.CurrencyTypes[i].CurrencyCode === response.data.m_Item1.Currency.CurrencyCode) {
                                                $scope.UKProductCatalog.Currency = $scope.CurrencyTypes[i];
                                            }
                                        }
                                    }

                                    if ($scope.Units !== undefined && $scope.Units !== null && $scope.Units !== '' && response.data.m_Item1.WeightUOM !== undefined && response.data.m_Item1.WeightUOM !== null && response.data.m_Item1.WeightUOM !== '') {
                                        for (j = 0; j < $scope.Units.length; j++) {
                                            if ($scope.Units[j].UnitValue === response.data.m_Item1.WeightUOM.UnitValue) {
                                                $scope.UKProductCatalog.WeightUOM = $scope.Units[j];
                                            }
                                        }
                                    }
                                }
                                AppSpinner.hideSpinnerTemplate();
                            }
                            else if (sp[0] === 'NRT') {
                                $scope.productcatalog.Length = response.data.m_Item3.Length;
                                $scope.productcatalog.Width = response.data.m_Item3.Width;
                                $scope.productcatalog.Height = response.data.m_Item3.Height;
                                $scope.productcatalog.Weight = response.data.m_Item3.Weight;
                                $scope.productcatalog.DeclareValue = response.data.m_Item3.DeclaredValue;
                                $scope.productcatalog.ProductDescription = response.data.m_Item3.ProductDescription;

                                if (response.data.m_Item3 !== undefined && response.data.m_Item3 !== null && response.data.m_Item3 !== '') {

                                    $scope.JapanProductCatalog = response.data.m_Item3;

                                    if ($scope.CurrencyTypes !== undefined && $scope.CurrencyTypes !== null && $scope.CurrencyTypes !== '' && $scope.CurrencyTypes.length > 0 && response.data.m_Item3.Currency !== undefined && response.data.m_Item3.Currency !== null && response.data.m_Item3.Currency !== '') {
                                        for (i = 0; i < $scope.CurrencyTypes.length; i++) {
                                            if ($scope.CurrencyTypes[i].CurrencyCode === response.data.m_Item3.Currency.CurrencyCode) {
                                                $scope.JapanProductCatalog.Currency = $scope.CurrencyTypes[i];
                                            }
                                        }
                                    }

                                    if ($scope.Units !== undefined && $scope.Units !== null && $scope.Units !== '' && response.data.m_Item3.WeightUOM !== undefined && response.data.m_Item3.WeightUOM !== null && response.data.m_Item3.WeightUOM !== '') {
                                        for (j = 0; j < $scope.Units.length; j++) {
                                            if ($scope.Units[j].UnitValue === response.data.m_Item3.WeightUOM.UnitValue) {
                                                $scope.JapanProductCatalog.WeightUOM = $scope.Units[j];
                                            }
                                        }
                                    }
                                }
                                AppSpinner.hideSpinnerTemplate();
                            }
                            else if (sp[0] === 'OSL') {
                                $scope.productcatalog.Length = response.data.m_Item5.Length;
                                $scope.productcatalog.Width = response.data.m_Item5.Width;
                                $scope.productcatalog.Height = response.data.m_Item5.Height;
                                $scope.productcatalog.Weight = response.data.m_Item5.Weight;
                                $scope.productcatalog.DeclareValue = response.data.m_Item5.DeclaredValue;
                                $scope.productcatalog.ProductDescription = response.data.m_Item5.ProductDescription;

                                if (response.data.m_Item5 !== undefined && response.data.m_Item5 !== null && response.data.m_Item5 !== '') {

                                    $scope.UKProductCatalog = response.data.m_Item5;

                                    if ($scope.CurrencyTypes !== undefined && $scope.CurrencyTypes !== null && $scope.CurrencyTypes !== '' && $scope.CurrencyTypes.length > 0 && response.data.m_Item5.Currency !== undefined && response.data.m_Item5.Currency !== null && response.data.m_Item5.Currency !== '') {
                                        for (i = 0; i < $scope.CurrencyTypes.length; i++) {
                                            if ($scope.CurrencyTypes[i].CurrencyCode === response.data.m_Item5.Currency.CurrencyCode) {
                                                $scope.UKProductCatalog.Currency = $scope.CurrencyTypes[i];
                                            }
                                        }
                                    }

                                    if ($scope.Units !== undefined && $scope.Units !== null && $scope.Units !== '' && response.data.m_Item5.WeightUOM !== undefined && response.data.m_Item5.WeightUOM !== null && response.data.m_Item5.WeightUOM !== '') {
                                        for (j = 0; j < $scope.Units.length; j++) {
                                            if ($scope.Units[j].UnitValue === response.data.m_Item5.WeightUOM.UnitValue) {
                                                $scope.UKProductCatalog.WeightUOM = $scope.Units[j];
                                            }
                                        }
                                    }
                                }
                                AppSpinner.hideSpinnerTemplate();
                            }
                            else if (sp[0] === 'SIN') {
                                $scope.productcatalog.Length = response.data.m_Item6.Length;
                                $scope.productcatalog.Width = response.data.m_Item6.Width;
                                $scope.productcatalog.Height = response.data.m_Item6.Height;
                                $scope.productcatalog.Weight = response.data.m_Item6.Weight;
                                $scope.productcatalog.DeclareValue = response.data.m_Item6.DeclaredValue;
                                $scope.productcatalog.ProductDescription = response.data.m_Item6.ProductDescription;

                                if (response.data.m_Item6 !== undefined && response.data.m_Item6 !== null && response.data.m_Item6 !== '') {

                                    $scope.UKProductCatalog = response.data.m_Item6;

                                    if ($scope.CurrencyTypes !== undefined && $scope.CurrencyTypes !== null && $scope.CurrencyTypes !== '' && $scope.CurrencyTypes.length > 0 && response.data.m_Item6.Currency !== undefined && response.data.m_Item6.Currency !== null && response.data.m_Item6.Currency !== '') {
                                        for (i = 0; i < $scope.CurrencyTypes.length; i++) {
                                            if ($scope.CurrencyTypes[i].CurrencyCode === response.data.m_Item6.Currency.CurrencyCode) {
                                                $scope.UKProductCatalog.Currency = $scope.CurrencyTypes[i];
                                            }
                                        }
                                    }

                                    if ($scope.Units !== undefined && $scope.Units !== null && $scope.Units !== '' && response.data.m_Item6.WeightUOM !== undefined && response.data.m_Item6.WeightUOM !== null && response.data.m_Item6.WeightUOM !== '') {
                                        for (j = 0; j < $scope.Units.length; j++) {
                                            if ($scope.Units[j].UnitValue === response.data.m_Item6.WeightUOM.UnitValue) {
                                                $scope.UKProductCatalog.WeightUOM = $scope.Units[j];
                                            }
                                        }
                                    }
                                }
                                AppSpinner.hideSpinnerTemplate();
                            }
                        }
                        else {
                            AppSpinner.hideSpinnerTemplate();
                        }
                        AppSpinner.hideSpinnerTemplate();
                    }
                    else {
                        AppSpinner.hideSpinnerTemplate();
                    }
                }
                else {
                    AppSpinner.hideSpinnerTemplate();
                }
            }, function () {
                AppSpinner.hideSpinnerTemplate();
            });
        }
        else {
            AppSpinner.hideSpinnerTemplate();
        }
    };

    var cuurency = function () {
        BreakBulkService.getCurrency().then(function (response) {
            if (response.data !== undefined && response.data !== null && response.data !== '') {
                $scope.CurrencyTypes = TopCurrencyService.TopCurrencyList(response.data);
                hubdetail();
            }
        });
    };

    var hubdetail = function () {
        BreakBulkService.Gethubs().then(function (response) {
            if (response.data !== undefined && response.data !== null && response.data !== '') {
                $scope.hubs = response.data;

                for (i = 0; i < $scope.hubs.length; i++) {
                    if ($scope.hubs[i].HubId == $scope.hubdetail.HubId) {
                        var value = $scope.CurrencyTypes;
                        if ($scope.hubdetail.HubCode === 'LHR' || $scope.hubdetail.HubCode === 'OSL' || $scope.hubdetail.HubCode === 'SIN') {
                            for (j = 0; j < $scope.CurrencyTypes.length; j++) {
                                if ($scope.hubs[i].DefaultCurrency === $scope.CurrencyTypes[j].CurrencyCode) {
                                    $scope.UKProductCatalog.Currency = $scope.CurrencyTypes[j];
                                }
                            }
                        }
                        else if ($scope.hubdetail.HubCode === 'YVR' || $scope.hubdetail.HubCode === 'YYZ') {
                            for (j = 0; j < $scope.CurrencyTypes.length; j++) {
                                if ($scope.hubs[i].DefaultCurrency === $scope.CurrencyTypes[j].CurrencyCode) {
                                    $scope.CanadaProductCatalog.Currency = $scope.CurrencyTypes[j];
                                }
                            }
                        }
                        else if ($scope.hubdetail.HubCode === 'ZRH') {
                            for (j = 0; j < $scope.CurrencyTypes.length; j++) {
                                if ($scope.hubs[i].DefaultCurrency === $scope.CurrencyTypes[j].CurrencyCode) {
                                    $scope.SwissProductCatalog.Currency = $scope.CurrencyTypes[j];
                                }
                            }
                        }
                        else if ($scope.hubdetail.HubCode === 'NRT') {
                            for (j = 0; j < $scope.CurrencyTypes.length; j++) {
                                if ($scope.hubs[i].DefaultCurrency === $scope.CurrencyTypes[j].CurrencyCode) {
                                    $scope.JapanProductCatalog.Currency = $scope.CurrencyTypes[j];
                                }
                            }
                        }
                        $scope.productcatalog.DestinationHub = $scope.hubs[i];
                        $scope.ShowCustomManifestFields($scope.productcatalog.DestinationHub);
                    }
                }
            }
        });
    };

    function init() {

        cuurency();
        setMultilingualOptions();
        $scope.CustomerId = CustomerId;
        $scope.hubdetail = HubDetail;
        $scope.Template = 'directBooking/ajaxLoader.tpl.html';

        $scope.Units = [
        {
            UnitValue: 'kgToCms',
            UnitDisplay: 'kGS/CMS'
        },
        {
            UnitValue: 'lbToInchs',
            UnitDisplay: 'LB/INCHS'
        }];

        $scope.submitted = true;
        $scope.destinationHub = true;
        $scope.currencyRequired = true;

        if (ProductcatalogId > 0) {
            $scope.productcatalog.ProductCatalogId = ProductcatalogId;
            $scope.productcatalog.CustomerId = $scope.CustomerId;
            editproductcatalog($scope.productcatalog.ProductCatalogId);
        }
        else {
            $scope.productcatalog.ProductCatalogId = 0;
            $scope.productcatalog.CustomerId = $scope.CustomerId;
        }
    }

    init();
});