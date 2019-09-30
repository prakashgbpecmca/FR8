angular.module('ngApp.customerStaff').controller('CustomerStaffController', function ($scope, $uibModal, ModalService, toaster, SessionService, CustomerStaffService, AppSpinner, $translate) {

    //add customer Staff code here
    $scope.addCustomerStaff = function (UserId) {
        ModalInstance = $uibModal.open({
            Animation: true,
            controller: 'AddCustomerStaffController',
            templateUrl: 'customerStaff/customerStaffAddEdit/addCustomerStaff.tpl.html',
            keyboard: true,
            windowClass: 'CustomerAddress-Edit',
            size: 'lg',
            backdrop: 'static',
            resolve: {
                UserId: function () {
                    return UserId;
                }
            }
        });

        ModalInstance.result.then(function () {
            $scope.GetInitials();
        });
    };
    //end

    var setModalOptions = function () {
        $translate(['user', 'detail', 'customer', 'ErrorGetting', 'FrayteInformation', 'Branch', 'FrayteValidation', 'FrayteError',
                'SuccessfullySavedInformation', 'PleaseCorrectValidationErrors', 'ErrorSavingRecord', 'Cancel_Validation', 'Cancel', 'Confirmation',
                'Successfully_Saved_User_Information', 'SavingUserDetail', 'Loading_Customer_Staff_Detail', 'TimeValidation', 'Add_Customer_Staff',
                'Delete_Customer_Staff_Detail', 'Customer_Staff_Remove_Success', 'Error_While_Removing']).then(function (translations) {
                $scope.TitleFrayteError = translations.FrayteError;
                $scope.TitleFrayteInformation = translations.FrayteInformation;
                $scope.TitleFrayteValidation = translations.FrayteValidation;
                $scope.Successfully_Saved_User_Informations = translations.Successfully_Saved_User_Information;
                $scope.TextErrorSavingRecord = translations.ErrorSavingRecord;
                $scope.TextValidation = translations.PleaseCorrectValidationErrors;
                $scope.TextErrorGettingUserDetail = translations.ErrorGetting + " " + translations.user + " " + translations.detail;
                $scope.TextErrorGettingCustomerRecord = translations.ErrorGetting + " " + translations.customer + " " + translations.Branch + " " + translations.detail + " " + translations.records;
                $scope.CancelConfirmation = translations.Cancel + " " + translations.Confirmation;
                $scope.CancelValidation = translations.Cancel_Validation;
                $scope.LoadingCustomerStaffDetail = translations.Loading_Customer_Staff_Detail;
                $scope.SavingUserDetail = translations.SavingUserDetail;
                $scope.ValidTime = translations.TimeValidation;
                $scope.AddCustomerStaff = translations.Add_Customer_Staff;
                $scope.RemoveCustomerStaffDetail = translations.Delete_Customer_Staff_Detail;
                $scope.CustomerStaffRemoveSuccess = translations.Customer_Staff_Remove_Success;
                $scope.ErrorWhileRemoving = translations.Error_While_Removing;

                $scope.GetInitials();
            });
    };

    $scope.GetInitials = function () {
        AppSpinner.showSpinnerTemplate($scope.LoadingCustomerStaffDetail, $scope.Template);
        CustomerStaffService.GetInitials($scope.CustomerStaffTrack).then(function (response) {
            if (response.data !== undefined && response.data !== '' && response.data !== null && response.data.length > 0) {
                $scope.customerstaffrecord = response.data;
                $scope.totalItemCount = response.data[0].TotalRows;
            }
            else {
                $scope.totalItemCount = 0;
            }
            AppSpinner.hideSpinnerTemplate();
        },
        function () {
            AppSpinner.hideSpinnerTemplate();
            //if (response.status !== 401) {
            //    toaster.pop({
            //        type: 'error',
            //        title: $scope.TitleFrayteError,
            //        body: $scope.InitialDataValidation,
            //        showCloseButton: true
            //    });
            //}
        });
    };

    $scope.removeCustomerStaff = function (UserId) {
        if (UserId !== undefined && UserId !== null && UserId !== '' && UserId > 0) {
            AppSpinner.showSpinnerTemplate($scope.RemoveCustomerStaffDetail, $scope.Template);
            CustomerStaffService.RemoveCustomerStaff(UserId).then(function (response) {
                if (response.data !== undefined && response.data !== null && response.data !== '' && response.data.Status === true) {
                    toaster.pop({
                        type: 'success',
                        title: $scope.TitleFrayteInformation,
                        body: $scope.CustomerStaffRemoveSuccess,
                        showCloseButton: true
                    });
                    $scope.GetInitials();
                    AppSpinner.hideSpinnerTemplate();
                }
                else {
                    AppSpinner.hideSpinnerTemplate();
                    toaster.pop({
                        type: 'error',
                        title: $scope.TitleFrayteError,
                        body: $scope.ErrorWhileRemoving,
                        showCloseButton: true
                    });
                }
            },
            function () {
                AppSpinner.hideSpinnerTemplate();
                //if (response.status !== 401) {
                //    toaster.pop({
                //        type: 'error',
                //        title: $scope.TitleFrayteError,
                //        body: $scope.InitialDataValidation,
                //        showCloseButton: true
                //    });
                //}
            });
        }
    };

    $scope.pageChanged = function (CustomerStaffTrack) {
        $scope.GetInitials();
    };

    function init() {

        $scope.Template = 'directBooking/ajaxLoader.tpl.html';
        var userInfo = SessionService.getUser();
        $scope.CreatedBy = userInfo.EmployeeId;
        $scope.RoleId = userInfo.RoleId;
        $scope.viewby = 50;
        $scope.currentPage = 1;
        $scope.itemsPerPage = $scope.viewby;
        $scope.maxSize = 2;

        $scope.pageChangedObj = [{
            SpecialSearchId: 1,
            pageChangedValue: 20,
            pageChangedValueDisplay: 20
         },
         {
             SpecialSearchId: 2,
             pageChangedValue: 50,
             pageChangedValueDisplay: 50
         },
         {
             SpecialSearchId: 3,
             pageChangedValue: 100,
             pageChangedValueDisplay: 100
         },
         {
             SpecialSearchId: 4,
             pageChangedValue: 200,
             pageChangedValueDisplay: 200
         }];

        $scope.CustomerStaffTrack = {
            CurrentPage: $scope.currentPage,
            TakeRows: $scope.itemsPerPage,
            RoleId: $scope.RoleId,
            CreatedBy: $scope.CreatedBy
        };

        setModalOptions();
    }

    init();

});